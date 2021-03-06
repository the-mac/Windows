﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18033
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This code was auto-generated by Microsoft.Silverlight.Phone.ServiceReference, version 3.7.0.0
// 
namespace ContosoMobile.ExpenseServiceReference
{
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(Namespace="http://samples.microsoft.com/DynamicsAx/", ConfigurationName="ExpenseServiceReference.ExpenseServiceContract")]
    public interface ExpenseServiceContract {
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://samples.microsoft.com/DynamicsAx/ExpenseServiceContract/CreateExpense", ReplyAction="http://samples.microsoft.com/DynamicsAx/ExpenseServiceContract/CreateExpenseRespo" +
            "nse")]
        System.IAsyncResult BeginCreateExpense(System.DateTime dateTime, decimal amount, string currency, string comments, System.AsyncCallback callback, object asyncState);
        
        long EndCreateExpense(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ExpenseServiceContractChannel : ContosoMobile.ExpenseServiceReference.ExpenseServiceContract, System.ServiceModel.IClientChannel
    {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class CreateExpenseCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public CreateExpenseCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public long Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((long)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ExpenseServiceContractClient : System.ServiceModel.ClientBase<ContosoMobile.ExpenseServiceReference.ExpenseServiceContract>, ContosoMobile.ExpenseServiceReference.ExpenseServiceContract
    {
        
        private BeginOperationDelegate onBeginCreateExpenseDelegate;
        
        private EndOperationDelegate onEndCreateExpenseDelegate;
        
        private System.Threading.SendOrPostCallback onCreateExpenseCompletedDelegate;
        
        private BeginOperationDelegate onBeginOpenDelegate;
        
        private EndOperationDelegate onEndOpenDelegate;
        
        private System.Threading.SendOrPostCallback onOpenCompletedDelegate;
        
        private BeginOperationDelegate onBeginCloseDelegate;
        
        private EndOperationDelegate onEndCloseDelegate;
        
        private System.Threading.SendOrPostCallback onCloseCompletedDelegate;
        
        public ExpenseServiceContractClient() {
        }
        
        public ExpenseServiceContractClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ExpenseServiceContractClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ExpenseServiceContractClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ExpenseServiceContractClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public System.Net.CookieContainer CookieContainer {
            get {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    return httpCookieContainerManager.CookieContainer;
                }
                else {
                    return null;
                }
            }
            set {
                System.ServiceModel.Channels.IHttpCookieContainerManager httpCookieContainerManager = this.InnerChannel.GetProperty<System.ServiceModel.Channels.IHttpCookieContainerManager>();
                if ((httpCookieContainerManager != null)) {
                    httpCookieContainerManager.CookieContainer = value;
                }
                else {
                    throw new System.InvalidOperationException("Unable to set the CookieContainer. Please make sure the binding contains an HttpC" +
                            "ookieContainerBindingElement.");
                }
            }
        }
        
        public event System.EventHandler<CreateExpenseCompletedEventArgs> CreateExpenseCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> OpenCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> CloseCompleted;
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult ContosoMobile.ExpenseServiceReference.ExpenseServiceContract.BeginCreateExpense(System.DateTime dateTime, decimal amount, string currency, string comments, System.AsyncCallback callback, object asyncState)
        {
            return base.Channel.BeginCreateExpense(dateTime, amount, currency, comments, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        long ContosoMobile.ExpenseServiceReference.ExpenseServiceContract.EndCreateExpense(System.IAsyncResult result)
        {
            return base.Channel.EndCreateExpense(result);
        }
        
        private System.IAsyncResult OnBeginCreateExpense(object[] inValues, System.AsyncCallback callback, object asyncState) {
            System.DateTime dateTime = ((System.DateTime)(inValues[0]));
            decimal amount = ((decimal)(inValues[1]));
            string currency = ((string)(inValues[2]));
            string comments = ((string)(inValues[3]));
            return ((ContosoMobile.ExpenseServiceReference.ExpenseServiceContract)(this)).BeginCreateExpense(dateTime, amount, currency, comments, callback, asyncState);
        }
        
        private object[] OnEndCreateExpense(System.IAsyncResult result) {
            long retVal = ((ContosoMobile.ExpenseServiceReference.ExpenseServiceContract)(this)).EndCreateExpense(result);
            return new object[] {
                    retVal};
        }
        
        private void OnCreateExpenseCompleted(object state) {
            if ((this.CreateExpenseCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.CreateExpenseCompleted(this, new CreateExpenseCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void CreateExpenseAsync(System.DateTime dateTime, decimal amount, string currency, string comments) {
            this.CreateExpenseAsync(dateTime, amount, currency, comments, null);
        }
        
        public void CreateExpenseAsync(System.DateTime dateTime, decimal amount, string currency, string comments, object userState) {
            if ((this.onBeginCreateExpenseDelegate == null)) {
                this.onBeginCreateExpenseDelegate = new BeginOperationDelegate(this.OnBeginCreateExpense);
            }
            if ((this.onEndCreateExpenseDelegate == null)) {
                this.onEndCreateExpenseDelegate = new EndOperationDelegate(this.OnEndCreateExpense);
            }
            if ((this.onCreateExpenseCompletedDelegate == null)) {
                this.onCreateExpenseCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnCreateExpenseCompleted);
            }
            base.InvokeAsync(this.onBeginCreateExpenseDelegate, new object[] {
                        dateTime,
                        amount,
                        currency,
                        comments}, this.onEndCreateExpenseDelegate, this.onCreateExpenseCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginOpen(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginOpen(callback, asyncState);
        }
        
        private object[] OnEndOpen(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndOpen(result);
            return null;
        }
        
        private void OnOpenCompleted(object state) {
            if ((this.OpenCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.OpenCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void OpenAsync() {
            this.OpenAsync(null);
        }
        
        public void OpenAsync(object userState) {
            if ((this.onBeginOpenDelegate == null)) {
                this.onBeginOpenDelegate = new BeginOperationDelegate(this.OnBeginOpen);
            }
            if ((this.onEndOpenDelegate == null)) {
                this.onEndOpenDelegate = new EndOperationDelegate(this.OnEndOpen);
            }
            if ((this.onOpenCompletedDelegate == null)) {
                this.onOpenCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnOpenCompleted);
            }
            base.InvokeAsync(this.onBeginOpenDelegate, null, this.onEndOpenDelegate, this.onOpenCompletedDelegate, userState);
        }
        
        private System.IAsyncResult OnBeginClose(object[] inValues, System.AsyncCallback callback, object asyncState) {
            return ((System.ServiceModel.ICommunicationObject)(this)).BeginClose(callback, asyncState);
        }
        
        private object[] OnEndClose(System.IAsyncResult result) {
            ((System.ServiceModel.ICommunicationObject)(this)).EndClose(result);
            return null;
        }
        
        private void OnCloseCompleted(object state) {
            if ((this.CloseCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.CloseCompleted(this, new System.ComponentModel.AsyncCompletedEventArgs(e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void CloseAsync() {
            this.CloseAsync(null);
        }
        
        public void CloseAsync(object userState) {
            if ((this.onBeginCloseDelegate == null)) {
                this.onBeginCloseDelegate = new BeginOperationDelegate(this.OnBeginClose);
            }
            if ((this.onEndCloseDelegate == null)) {
                this.onEndCloseDelegate = new EndOperationDelegate(this.OnEndClose);
            }
            if ((this.onCloseCompletedDelegate == null)) {
                this.onCloseCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnCloseCompleted);
            }
            base.InvokeAsync(this.onBeginCloseDelegate, null, this.onEndCloseDelegate, this.onCloseCompletedDelegate, userState);
        }

        protected override ContosoMobile.ExpenseServiceReference.ExpenseServiceContract CreateChannel()
        {
            return new ExpenseServiceContractClientChannel(this);
        }

        private class ExpenseServiceContractClientChannel : ChannelBase<ContosoMobile.ExpenseServiceReference.ExpenseServiceContract>, ContosoMobile.ExpenseServiceReference.ExpenseServiceContract
        {

            public ExpenseServiceContractClientChannel(System.ServiceModel.ClientBase<ContosoMobile.ExpenseServiceReference.ExpenseServiceContract> client) : 
                    base(client) {
            }
            
            public System.IAsyncResult BeginCreateExpense(System.DateTime dateTime, decimal amount, string currency, string comments, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[4];
                _args[0] = dateTime;
                _args[1] = amount;
                _args[2] = currency;
                _args[3] = comments;
                System.IAsyncResult _result = base.BeginInvoke("CreateExpense", _args, callback, asyncState);
                return _result;
            }
            
            public long EndCreateExpense(System.IAsyncResult result) {
                object[] _args = new object[0];
                long _result = ((long)(base.EndInvoke("CreateExpense", _args, result)));
                return _result;
            }
        }
    }
}
