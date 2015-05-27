﻿using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace ContosoMobile.Authentication
{    
    /// <summary>
    /// This class is used to fetch the SAML and SWT tokens required to authenticate the user.
    /// </summary>
    /// <remarks>
    /// The application consuming this API may want to cache these tokens. 
    /// These tokens are wrapped into a SOAP header and sent along with the message going through the Service Bus
    /// The creation of the service payload (tokens + data) is currently left to the consuming App to perform, when it decides to call its service operations
    /// </remarks>
    public class AuthenticationProvider
    {
        /// <summary>
        /// This is the actual payload of the SOAP message that will be sent to the STS
        /// </summary>
        /// <remarks>
        /// It is a Request for Security Token (RST) as per the WS-Trust specification, and can alternatively be generated by 
        /// appropriate library support, e.g. Windows Identity Foundation (WIF)
        /// </remarks>
        private string StsUrlPayload = @"<s:Envelope xmlns:s=""http://www.w3.org/2003/05/soap-envelope"" 
xmlns:a=""http://www.w3.org/2005/08/addressing"" 
xmlns:u=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd"">
	<s:Header>
		<a:Action s:mustUnderstand=""1"">http://docs.oasis-open.org/ws-sx/ws-trust/200512/RST/Issue</a:Action>		
		<a:To s:mustUnderstand=""1"">{0}</a:To>
		<o:Security s:mustUnderstand=""1"" xmlns:o=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd"">
			<u:Timestamp u:Id=""_0"">
				<u:Created>{1}Z</u:Created>
				<u:Expires>{2}Z</u:Expires>
			</u:Timestamp>
			<o:UsernameToken u:Id=""uuid-ea4cdc24-712d-4af7-9128-acb3db03b55f-1"">
				<o:Username>{3}</o:Username>
				<o:Password o:Type=""http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-username-token-profile-1.0#PasswordText"">{4}</o:Password>
			</o:UsernameToken>
		</o:Security>
	</s:Header>

	<s:Body>
		<trust:RequestSecurityToken xmlns:trust=""http://docs.oasis-open.org/ws-sx/ws-trust/200512"">
			<wsp:AppliesTo xmlns:wsp=""http://schemas.xmlsoap.org/ws/2004/09/policy"">
				<a:EndpointReference>
					<a:Address>{5}</a:Address>
				</a:EndpointReference>
			</wsp:AppliesTo>
			<trust:KeyType>http://docs.oasis-open.org/ws-sx/ws-trust/200512/Bearer</trust:KeyType>
			<trust:RequestType>http://docs.oasis-open.org/ws-sx/ws-trust/200512/Issue</trust:RequestType>
			<trust:TokenType>urn:oasis:names:tc:SAML:1.0:assertion</trust:TokenType>
		</trust:RequestSecurityToken>
	</s:Body>
</s:Envelope>";

        #region private fields

        // User alias and password
        private UserCredentials credentials;

        // The Windows Azure namespace of the Service Bus
        private string azureNamespace = String.Empty;
        
        // The AD FS endpoint URL
        private string stsEndpoint = String.Empty;
        
        // Stores a request for security token (RST) payload sent to AD FS
        private string requestSecurityTokenPayload = String.Empty;

        // Stores a request for security token response (RSTR) payload received from AD FS
        private string requestSecurityTokenResponse = String.Empty;
        
        // Stores the payload containing the SAML claims sent to ACS, requesting a SWT 
        private string acsPayload = String.Empty;
      
        #endregion

        #region properties

        /// <summary>
        /// This read-only property is the relying party that is configured in AD FS. This value tells the STS how
        /// to respond (which token format, whether it's encrypted, which claims to include, token lifetime, etc.)
        /// </summary>
        public string AppliesTo
        {
            get
            {
                return "https://" + azureNamespace + "-sb.accesscontrol.windows.net";
            }           
        }

        /// <summary>
        /// This read-only property is the actual ACS endpoint we post the SAML token to in order to acquire the SWT
        /// </summary>
        public string AcsEndPoint
        {
            get
            {
                return "https://" + azureNamespace + "-sb.accesscontrol.windows.net/WRAPv0.9/";
            }
        }

        /// <summary>
        /// This read-only property is the relying party that is configured in SB's ACS.
        /// </summary>
        /// <remarks>
        /// The value is simply a hint to the STS as to which rules to perform.
        /// By default, it's simply http://[namespace].servicebus.windows.net/, but more specific endpoint configurations are possible if needed.
        /// If we send http://[namespace].servicebus.windows.net/Expense, it will look for that and if it doesn't find it, will look for the former.
        /// DO NOTE the absence of the 's' in http:// here...this needs to match the configuration in ACS or the request will be rejected.
        /// </remarks>
        public string RelyingParty
        {
            get
            {
                return "http://" + azureNamespace + ".servicebus.windows.net/";
            }
        }

        /// <summary>
        /// Stores SAML claims extracted from the RSTR from AD FS
        /// </summary>
        public string SamlAssertionToken
        {
            get;
            private set;
        }

        /// <summary>
        /// Stores a Simple Web Token(SWT) received from ACS
        /// </summary>
        public string SwtAcsToken
        {
            get;
            private set;
        }

        #endregion

        /// <summary>
        /// Constructor to instantiate the AuthenticationProvider properties
        /// </summary>
        /// <param name="userAlias">User alias in the form domain\useralias</param>
        /// <param name="password">User password</param>
        /// <param name="windowsAzureNamespace">Windows Azure namespace of the Service Bus</param>
        /// <param name="stsEndpointUrl">AD FS Endpoint Url, eg: 'https://contosoadfs.com/adfs/services/trust/13/usernamemixed'</param>
        public AuthenticationProvider(string userAlias, string password, string windowsAzureNamespace, string stsEndpointUrl)
        {
            credentials = new UserCredentials(userAlias, password);
            azureNamespace = windowsAzureNamespace;
            stsEndpoint = stsEndpointUrl;
        }
       
        #region Request Security Token from STS

        /// <summary>
        /// Method exposed to kick off a bunch of asynchronous for performing the authentication.
        /// Once the authentication has successfully completed, the SamlAssertionToken & SwtAcsToken are ready to be consumed.
        /// </summary>
        public void IssueToken()
        {
            SendRequestSecurityToken();
        }
        
        /// <summary>
        /// Create the RST payload containing the user's credentials
        /// Send the RST to the STS in a SOAP message using HttpWebRequest
        /// </summary>
        private void SendRequestSecurityToken()
        {
            DateTime stsAuthStartTime = DateTime.UtcNow;

            //Insert the sts endpoint, user credential and relying party values in the payload. Also setting the '<u:Expires>' time value to UtcNow + 5 minutes
            requestSecurityTokenPayload = String.Format(this.StsUrlPayload, this.stsEndpoint, stsAuthStartTime.ToString("s") + ".000",
                stsAuthStartTime.AddMinutes(5).ToString("s") + ".000", credentials.Alias, credentials.Password, this.AppliesTo);

            try
            {
                HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(stsEndpoint);
                httpWebRequest.ContentType = "application/soap+xml; charset=utf-8";
                httpWebRequest.Method = "POST";
                httpWebRequest.BeginGetRequestStream(new AsyncCallback(RequestSecurityTokenStreamCallback), httpWebRequest);
            }
            catch (Exception e)
            {
                OnAuthenticationCompleted(new AuthenticationMessageArgs(new AuthenticationMessage(e.Message, false)));
            }
        }

        /// <summary>
        /// Writes the RST data bytes into the System.IO.Stream object
        /// </summary>
        /// <param name="asynchronousResult">The asynchronous result from the callback of the request</param>
        private void RequestSecurityTokenStreamCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)asynchronousResult.AsyncState;
            Stream requestStream = null;

            try
            {
                using (requestStream = httpWebRequest.EndGetRequestStream(asynchronousResult))
                {
                    if (requestStream != null)
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(requestSecurityTokenPayload);
                        requestStream.Write(bytes, 0, bytes.Length);                        
                    }
                }
                httpWebRequest.BeginGetResponse(new AsyncCallback(RequestSecurityTokenResponseStreamCallback), httpWebRequest);
            }
            catch (Exception e)
            {
                OnAuthenticationCompleted(new AuthenticationMessageArgs(new AuthenticationMessage(e.Message, false)));
            }
        }

        /// <summary>
        /// Callback method to fetch the request for security token response
        /// </summary>
        /// <param name="asynchronousResult">The asynchronous result from the callback of the request</param>
        private void RequestSecurityTokenResponseStreamCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest httpWebRequest = (HttpWebRequest)asynchronousResult.AsyncState;

            try
            {
                HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.EndGetResponse(asynchronousResult);
                requestSecurityTokenResponse = Utilities.GetResponseStringFromResponse(httpWebResponse);
                
                SendTokenToAcs();
            }
            catch (WebException e)
            {                
                string responseString = Utilities.GetResponseStringFromResponse((HttpWebResponse)e.Response);
                string errorStr = Utilities.GetErrorFromStsResponseString(responseString);
                if (errorStr.Contains("FailedAuthentication"))
                {
                    errorStr = String.Format("Username and/or password was not accepted. Please correct your credentials and try again");
                }
                OnAuthenticationCompleted(new AuthenticationMessageArgs(new AuthenticationMessage(errorStr, false)));
            }
        }

        #endregion

        #region Request for SWT token from ACS

        /// <summary>
        /// Extracts the SAML assertions from the RSTR and sends a request to the ACS for a SWT token
        /// </summary>
        private void SendTokenToAcs()
        {
            const string acsPayloadFormat = @"wrap_scope={0}&wrap_assertion={1}&wrap_assertion_format=SAML";
            
            SamlAssertionToken = Utilities.GetSamlTokenFromResponseString(requestSecurityTokenResponse);

            if (!String.IsNullOrEmpty(SamlAssertionToken))
            {
                acsPayload = String.Format(acsPayloadFormat, Uri.EscapeDataString(RelyingParty), Uri.EscapeDataString(SamlAssertionToken));
                
                try
                {
                    HttpWebRequest request = (HttpWebRequest)WebRequest.Create(AcsEndPoint);
                    request.ContentType = @"application/x-www-form-urlencoded";
                    request.Method = "POST";
                    request.BeginGetRequestStream(new AsyncCallback(RequestAcsTokenStreamCallback), request);
                }
                catch (WebException e)
                {
                    OnAuthenticationCompleted(new AuthenticationMessageArgs(new AuthenticationMessage(e.Message, false)));
                }
            }
            else
            {
                OnAuthenticationCompleted(new AuthenticationMessageArgs(new AuthenticationMessage("SAML Token from STS is empty", false)));
            }           
        }

        /// <summary>
        /// Writes the ACS payload data bytes into the System.IO.Stream object
        /// </summary>
        /// <param name="asynchronousResult">The asynchronous result from the callback of the request</param>
        private void RequestAcsTokenStreamCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
            Stream requestStream = null;
            
            try
            {
                using (requestStream = request.EndGetRequestStream(asynchronousResult))
                {
                    if (requestStream != null)
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(acsPayload);
                        requestStream.Write(bytes, 0, bytes.Length);                        
                    }
                }
                request.BeginGetResponse(new AsyncCallback(RequestAcsTokenResponseCallback), request);
            }
            catch (WebException e)
            {
                OnAuthenticationCompleted(new AuthenticationMessageArgs(new AuthenticationMessage(e.Message, false)));
            }          
        }

        /// <summary>
        /// Callback method to fetch the SWT token from the response
        /// </summary>
        /// <param name="asynchronousResult"></param>
        private void RequestAcsTokenResponseCallback(IAsyncResult asynchronousResult)
        {
            HttpWebRequest request = (HttpWebRequest)asynchronousResult.AsyncState;
            
            try
            {
                HttpWebResponse response = (HttpWebResponse)request.EndGetResponse(asynchronousResult);
                string rawAcsResponse = Utilities.GetResponseStringFromResponse(response);

                SwtAcsToken = Uri.UnescapeDataString(rawAcsResponse.Split('&').Single(value => value.StartsWith("wrap_access_token=", StringComparison.OrdinalIgnoreCase)).Split('=')[1]);

                if (!String.IsNullOrEmpty(this.SwtAcsToken))
                {
                    OnAuthenticationCompleted(new AuthenticationMessageArgs(new AuthenticationMessage("Security Token from ACS issued successfully", true))); 
                }
            }
            catch (WebException e)
            {
                // The ACS server has likely sent back an error response
                string responseString = Utilities.GetResponseStringFromResponse((HttpWebResponse)e.Response);
                OnAuthenticationCompleted(new AuthenticationMessageArgs(new AuthenticationMessage(responseString, false)));
            }           
            catch (Exception e)
            {
                OnAuthenticationCompleted(new AuthenticationMessageArgs(new AuthenticationMessage(e.Message, false)));
            }
        }        

        #endregion

        #region AuthenticationCompleted EventHandler

        /// <summary>
        /// Subscribe to this event to be notified of authentication completion messages, both succeeded & failed.
        /// </summary>
        public event AuthenticationCompletedEventHandler AuthenticationCompleted;

        /// <summary>
        /// The On Completed method to be called
        /// </summary>
        /// <param name="args"></param>
        private void OnAuthenticationCompleted(AuthenticationMessageArgs args)
        {
            AuthenticationCompletedEventHandler handler = AuthenticationCompleted;

            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion
    }
}