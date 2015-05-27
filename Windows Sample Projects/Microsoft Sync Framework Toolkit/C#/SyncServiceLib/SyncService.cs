﻿// Copyright © Microsoft Corporation. All rights reserved.

// Microsoft Limited Permissive License (Ms-LPL)

// This license governs use of the accompanying software. If you use the software, you accept this license. If you do not accept the license, do not use the software.

// 1. Definitions
// The terms “reproduce,” “reproduction,” “derivative works,” and “distribution” have the same meaning here as under U.S. copyright law.
// A “contribution” is the original software, or any additions or changes to the software.
// A “contributor” is any person that distributes its contribution under this license.
// “Licensed patents” are a contributor’s patent claims that read directly on its contribution.

// 2. Grant of Rights
// (A) Copyright Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free copyright license to reproduce its contribution, prepare derivative works of its contribution, and distribute its contribution or any derivative works that you create.
// (B) Patent Grant- Subject to the terms of this license, including the license conditions and limitations in section 3, each contributor grants you a non-exclusive, worldwide, royalty-free license under its licensed patents to make, have made, use, sell, offer for sale, import, and/or otherwise dispose of its contribution in the software or derivative works of the contribution in the software.

// 3. Conditions and Limitations
// (A) No Trademark License- This license does not grant you rights to use any contributors’ name, logo, or trademarks.
// (B) If you bring a patent claim against any contributor over patents that you claim are infringed by the software, your patent license from such contributor to the software ends automatically.
// (C) If you distribute any portion of the software, you must retain all copyright, patent, trademark, and attribution notices that are present in the software.
// (D) If you distribute any portion of the software in source code form, you may do so only under this license by including a complete copy of this license with your distribution. If you distribute any portion of the software in compiled or object code form, you may only do so under a license that complies with this license.
// (E) The software is licensed “as-is.” You bear the risk of using it. The contributors give no express warranties, guarantees or conditions. You may have additional consumer rights under your local laws which this license cannot change. To the extent permitted under your local laws, the contributors exclude the implied warranties of merchantability, fitness for a particular purpose and non-infringement.
// (F) Platform Limitation- The licenses granted in sections 2(A) & 2(B) extend only to the software or derivative works that you create that run on a Microsoft Windows operating system product.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.ServiceModel.Channels;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Activation;
using Microsoft.Synchronization.Data;
using Microsoft.Synchronization.Services.SqlProvider;

namespace Microsoft.Synchronization.Services
{
    /// <summary>
    /// Arguments for OnEndSyncRequest event
    /// </summary>
    public class OnEndSyncRequestEventArgs : EventArgs 
    {
        private Message msg;
        
        /// <summary>
        /// Constructor of the event that is raised before processing a sync request.
        /// </summary>
        public OnEndSyncRequestEventArgs(Message msg)
        {
            this.msg = msg;
        }
        
        /// <summary>
        /// Access message argument
        /// </summary>
        public Message Msg 
        { 
            get { return msg;} 
        }
    }
    
    // InstanceContextMode => Specifies the number of service instances available for handling calls that are contained in incoming messages.
    //
    // AspNetCompatibilityRequirements => 
    //      ASP.NET compatibility mode allows WCF services to use features such as identity impersonation.
    /// <summary>
    /// SyncService implementation. This class handles the incoming requests for sync.
    /// </summary>
    /// <typeparam name="T">Entities class that is auto-generated by the SyncSvcUtil.exe tool.</typeparam>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall), 
                     AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public abstract class SyncService<T> : IRequestHandler where T : class, new()
    {
        #region Private Members

        private HttpContextServiceHost _serviceHost;
        private Request _requestDescription;
        private IRequestProcessor _requestProcessor;
        private Message _outgoingMessage;

        private SyncServiceConfiguration _syncConfiguration;

        #endregion

        #region Constructor

        /// <summary>
        /// Static constructor.
        /// </summary>
        static SyncService()
        {
            // Ensure that <T> has the SyncScope attribute on it.
            //Note: Early validation check to fail quickly if things are not correct.
            if (!Attribute.IsDefined(typeof(T), typeof(SyncScopeAttribute)))
            {
                throw SyncServiceException.CreateInternalServerError(Strings.TemplateClassNotMarkedWithSyncScopeAttribute);
            }

            SyncServiceTracer.TraceVerbose("SyncService initialized!");
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Method that can be overridden in a derived class to execute custom code before
        /// performing diagnostic checks. If the method returns true, the diagnostics checks will be
        /// done else a HTTP 404 response is sent back to the caller.
        /// </summary>
        /// <returns><see cref="bool" /> indicating whether diagnostics checks should be performed or not.</returns>
        public virtual bool OnBeginDiagnosticRequest()
        {
            // Return true here. The overridden method (if it exists) will be called.
            // Otherwise continue with processing the request.
            return true;
        }

        #endregion

        #region IRequestHandler Members

        /// <summary>
        /// Process an incoming request and return a WCF Message class instance.
        /// </summary>
        /// <param name="messageBody">Message body</param>
        /// <returns>Instance of the WCF Message class</returns>
        public Message ProcessRequestForMessage(Stream messageBody)
        {
            try
            {
                // Intialize the service host first, since most of the logic depends on it.
                _serviceHost = new HttpContextServiceHost(messageBody);
                _serviceHost.ValidateRequestHttpVerbAndSegments();

                // Create the configuration. The first call will initialize the configuration
                // and all other calls will be a no-op. 
                CreateConfiguration();

                // Raise event for user code
                InvokeOnSyncRequestStart();

                _requestDescription = new RequestParser(_serviceHost, _syncConfiguration).ParseIncomingRequest();

                if (null == _requestDescription.SyncBlob || 0 == _requestDescription.SyncBlob.Length)
                {
                    InitializeNewClient();
                }

                _requestProcessor = RequestProcessorFactory.GetRequestProcessorInstance(_requestDescription.RequestCommand, _syncConfiguration, _serviceHost);

                _outgoingMessage = _requestProcessor.ProcessRequest(_requestDescription);

                // Add sync properties
                var responseProperties = _outgoingMessage.Properties[HttpResponseMessageProperty.Name] as HttpResponseMessageProperty;
                if (null != responseProperties)
                {
                    responseProperties.Headers[SyncServiceConstants.SYNC_SERVICE_VERSION_KEY] = SyncServiceConstants.SYNC_SERVICE_VERSION_VALUE;
                }

                // Raise event for user code
                InvokeOnEndSyncRequest(_outgoingMessage);
            }
            catch (SyncServiceException syncServiceException)
            {
                ProcessSyncServiceException(syncServiceException);
            }
            catch (Exception exception)
            {
                if (WebUtil.IsFatalException(exception))
                {
                    throw;
                }

                _outgoingMessage = CreateMessageFromUnhandledException(exception);
            }

            return _outgoingMessage;
        }

        /// <summary>
        /// This method is invoked when a GET request is made to the service root.
        /// We will redirect this request to the /$syncscopes URL which will return a list of scopes.
        /// Without this, the service returns a missing operation error.
        /// </summary>
        public void ProcessRequestToServiceRoot()
        {
            _serviceHost = new HttpContextServiceHost(null);

            // Look at the host header to determine the domain to which the request was made.
            // In Windows Azure, the Uri class does not give accurate information about the request host.
            string requestHost = !String.IsNullOrEmpty(_serviceHost.HostHeader) ? _serviceHost.HostHeader : _serviceHost.RequestUri.Authority;

            // Create the URL corresponding to $syncscopes.
            // SchemeName is "http" or "https"
            // requestHost will contain domain and port number (if any)
            // AbsolutePath for this operation contract will be "/<servicename>.svc/"
            _serviceHost.OutgoingResponse.Location =
                String.Format("{0}://{1}{2}{3}", _serviceHost.RequestUri.Scheme, requestHost, 
                                                   _serviceHost.RequestUri.AbsolutePath, 
                                                   SyncServiceConstants.SYNC_SCOPES_URL_SEGMENT);

            _serviceHost.OutgoingResponse.StatusCode = HttpStatusCode.Redirect;
        }

        /// <summary>Processes the diagnostics page request.</summary>
        /// <returns>The response <see cref="Message"/>.</returns>
        public Message ProcessRequestForDiagnostics()
        {
            try
            {
                // Intialize the service host.
                _serviceHost = new HttpContextServiceHost();

                // Ensure configuration.
                CreateConfiguration();

                Debug.Assert(_syncConfiguration != null, "_syncConfiguration != null");

                // Check the EnableDiagPage property and invoke the callback if necessary. 
                // Return 404-Not Found if not enabled.
                if (!_syncConfiguration.EnableDiagnosticPage || !OnBeginDiagnosticRequest())
                {
                    throw SyncServiceException.CreateResourceNotFound();
                }

                // Perform diagnostic checks and prepare outgoing message.
                _outgoingMessage = DiagHelper.CreateDiagResponseMessage(_syncConfiguration, _serviceHost);
            }
            catch (SyncServiceException syncServiceException)
            {
                ProcessSyncServiceException(syncServiceException);
            }
            catch (Exception exception)
            {
                if (WebUtil.IsFatalException(exception))
                {
                    throw;
                }

                _outgoingMessage = CreateMessageFromUnhandledException(exception);
            }

            return _outgoingMessage;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Call the initialize client user code and create a new sync blob with the clientId.
        /// </summary>
        private void InitializeNewClient()
        {
            _requestDescription.ProviderParams = new List<SqlSyncProviderFilterParameterInfo>(_syncConfiguration.FilterParameters.Count);

            // Loop through the filter parameter configuration and create a list of parameters in the request description 
            // that also contains the value.
            //Note: A single query string parameter can be assigned to multiple tables.
            foreach (var filterParameter in _syncConfiguration.FilterParameters)
            {
                string value = _serviceHost.GetQueryStringItem(filterParameter.QueryStringKey);
                _requestDescription.ProviderParams.Add(new SqlSyncProviderFilterParameterInfo
                                                           {
                                                               SqlParameterName = filterParameter.SqlParameterName,
                                                               QueryStringKey = filterParameter.QueryStringKey,
                                                               TableName = filterParameter.TableName,
                                                               Value = WebUtil.ChangeType(value, filterParameter.ValueType)
                                                           });
            }
        }

        /// <summary>
        /// This method tries to look up the service configuration from the MetadataCache.
        /// For the first request, there will be no item in the cache and so, an object of type
        /// SyncServiceConfiguration is created, initialized and added to the MetadataCache.
        /// </summary>
        private void CreateConfiguration()
        {
            Type serviceType = base.GetType();

            // Check if we already have a configuration for the service in the metadata cache.
            MetadataCacheItem item = MetadataCache.TryLookup(serviceType);

            if (null == item)
            {
                SyncTracer.Info("Creating SyncServiceConfiguration for service type {0}", serviceType);

                item = new MetadataCacheItem(serviceType);

                // Initialize the private member since it will then have default values.
                // In case of an error in the static initializer, we can refer to the default values
                // of configuration.
                _syncConfiguration = new SyncServiceConfiguration(typeof(T));

                // This will invoke the static initialization method.
                _syncConfiguration.Initialize(serviceType);
                
                item.Configuration = _syncConfiguration;

                MetadataCache.AddCacheItem(serviceType, item);

                SyncTracer.Info("SyncServiceConfiguration for service type {0} created successfully!", serviceType);
            }
            else
            {
                _syncConfiguration = item.Configuration;
            }
            
            // Invoke the testhook Initialize method. 
            // Note: This needs to be called regardless of whether we find the configuration in the 
            // cache or not because this is on a per request basis.
            _syncConfiguration.InvokeTestHookInitializeMethod(serviceType);
        }

        /// <summary>
        /// Create a new instance of the WCF Message class that has the correct content type (xml/json) and has a serialized
        /// instance of the SyncError class which is a representation of the error message.
        /// </summary>
        /// <param name="httpStatusCode">Status code to be used for the outgoing response.</param>
        /// <param name="errorDescription">A description of the error.</param>
        /// <returns>An instance of the WCF Message class that is sent as response.</returns>
        private Message CreateExceptionMessage(HttpStatusCode httpStatusCode, string errorDescription)
        {
            var error = new ServiceError { ErrorDescription = errorDescription };

            Message message;
            if (_syncConfiguration.SerializationFormat == SyncSerializationFormat.ODataJson)
            {
                message = Message.CreateMessage(MessageVersion.None, String.Empty, error, new DataContractJsonSerializer(typeof(ServiceError)));
                message.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Json));
            }
            else
            {
                message = Message.CreateMessage(MessageVersion.None, String.Empty, error, new DataContractSerializer(typeof(ServiceError)));
                message.Properties.Add(WebBodyFormatMessageProperty.Name, new WebBodyFormatMessageProperty(WebContentFormat.Xml));
            }

            var property = new HttpResponseMessageProperty { StatusCode = httpStatusCode };
            property.Headers.Add(HttpResponseHeader.ContentType, WebUtil.GetContentType(_serviceHost.GetOutputSerializationFormat(_syncConfiguration.SerializationFormat)));

            message.Properties.Add(HttpResponseMessageProperty.Name, property);

            return message;
        }

        /// <summary>Create a instance of <see cref="Message" /> from an exception. This is sent as the response.</summary>
        /// <param name="exception">Exception to process</param>
        /// <returns>An instance of <see cref="Message" /> class.</returns>
        private Message CreateMessageFromUnhandledException(Exception exception)
        {
            string exceptionMessage = WebUtil.GetExceptionMessage(exception);

            SyncServiceTracer.TraceError(exceptionMessage);

            return _syncConfiguration.UseVerboseErrors
                                   ? CreateExceptionMessage(HttpStatusCode.InternalServerError, exceptionMessage)
                                   : CreateExceptionMessage(HttpStatusCode.InternalServerError, Strings.InternalServerError);
        }

        private void ProcessSyncServiceException(SyncServiceException syncServiceException)
        {
            string exceptionMessage = WebUtil.GetExceptionMessage(syncServiceException);

            SyncServiceTracer.TraceWarning(exceptionMessage);

            _outgoingMessage = _syncConfiguration.UseVerboseErrors
                                   ? CreateExceptionMessage((HttpStatusCode)syncServiceException.StatusCode, exceptionMessage)
                                   : CreateExceptionMessage((HttpStatusCode)syncServiceException.StatusCode, syncServiceException.Message);

            // Add the "Allow" HTTP header if present._outgoingMessage.Properties[HttpResponseMessageProperty.Name].
            if (!String.IsNullOrEmpty(syncServiceException.ResponseAllowHeader) &&
                null != _outgoingMessage.Properties[HttpResponseMessageProperty.Name])
            {
                ((HttpResponseMessageProperty)_outgoingMessage.Properties[HttpResponseMessageProperty.Name]).
                    Headers.Add("Allow", syncServiceException.ResponseAllowHeader);
            }
        }

        #endregion

        #region Events

        /// <summary>
        /// Event that is raised before processing a sync request.
        /// </summary>
        public event EventHandler OnBeginSyncRequest;

        private void InvokeOnSyncRequestStart()
        {
            EventHandler request = OnBeginSyncRequest;
            if (request != null)
            {
                SyncTracer.Verbose("Raising OnBeginSyncRequest event");
                request(this, new EventArgs());
                SyncTracer.Verbose("Done: Raising OnBeginSyncRequest event");
            }
        }
        
        /// <summary>
        /// Event that is raised before writing the response to the caller.
        /// </summary>
        public event EventHandler<OnEndSyncRequestEventArgs> OnEndSyncRequest;

        private void InvokeOnEndSyncRequest(Message message)
        {
            EventHandler<OnEndSyncRequestEventArgs> request = OnEndSyncRequest;
            if (request != null)
            {
                SyncTracer.Verbose("Raising OnEndSyncRequest event");
                request(this, new OnEndSyncRequestEventArgs(message));
                SyncTracer.Verbose("Done: Raising OnEndSyncRequest event");
            }
        }

        #endregion
    }
}
