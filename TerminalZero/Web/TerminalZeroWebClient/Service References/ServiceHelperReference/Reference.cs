﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// 
// This code was auto-generated by Microsoft.Silverlight.ServiceReference, version 3.0.40818.0
// 
namespace TerminalZeroWebClient.ServiceHelperReference {
    using System.Runtime.Serialization;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="VirtualLogEntry", Namespace="http://schemas.datacontract.org/2004/07/ZeroLogHandle.Classes")]
    public partial class VirtualLogEntry : object, System.ComponentModel.INotifyPropertyChanged {
        
        private int IndentLevelField;
        
        private string MessageField;
        
        private System.DateTime StampField;
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public int IndentLevel {
            get {
                return this.IndentLevelField;
            }
            set {
                if ((this.IndentLevelField.Equals(value) != true)) {
                    this.IndentLevelField = value;
                    this.RaisePropertyChanged("IndentLevel");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string Message {
            get {
                return this.MessageField;
            }
            set {
                if ((object.ReferenceEquals(this.MessageField, value) != true)) {
                    this.MessageField = value;
                    this.RaisePropertyChanged("Message");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public System.DateTime Stamp {
            get {
                return this.StampField;
            }
            set {
                if ((this.StampField.Equals(value) != true)) {
                    this.StampField = value;
                    this.RaisePropertyChanged("Stamp");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceHelperReference.ServiceHelper")]
    public interface ServiceHelper {
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/ServiceHelper/GetLogsStr", ReplyAction="http://tempuri.org/ServiceHelper/GetLogsStrResponse")]
        System.IAsyncResult BeginGetLogsStr(System.DateTime lastStamp, System.AsyncCallback callback, object asyncState);
        
        System.Collections.ObjectModel.ObservableCollection<string> EndGetLogsStr(System.IAsyncResult result);
        
        [System.ServiceModel.OperationContractAttribute(AsyncPattern=true, Action="http://tempuri.org/ServiceHelper/GetLogs", ReplyAction="http://tempuri.org/ServiceHelper/GetLogsResponse")]
        System.IAsyncResult BeginGetLogs(System.DateTime lastStamp, System.AsyncCallback callback, object asyncState);
        
        System.Collections.ObjectModel.ObservableCollection<TerminalZeroWebClient.ServiceHelperReference.VirtualLogEntry> EndGetLogs(System.IAsyncResult result);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ServiceHelperChannel : TerminalZeroWebClient.ServiceHelperReference.ServiceHelper, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetLogsStrCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public GetLogsStrCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public System.Collections.ObjectModel.ObservableCollection<string> Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((System.Collections.ObjectModel.ObservableCollection<string>)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class GetLogsCompletedEventArgs : System.ComponentModel.AsyncCompletedEventArgs {
        
        private object[] results;
        
        public GetLogsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) : 
                base(exception, cancelled, userState) {
            this.results = results;
        }
        
        public System.Collections.ObjectModel.ObservableCollection<TerminalZeroWebClient.ServiceHelperReference.VirtualLogEntry> Result {
            get {
                base.RaiseExceptionIfNecessary();
                return ((System.Collections.ObjectModel.ObservableCollection<TerminalZeroWebClient.ServiceHelperReference.VirtualLogEntry>)(this.results[0]));
            }
        }
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class ServiceHelperClient : System.ServiceModel.ClientBase<TerminalZeroWebClient.ServiceHelperReference.ServiceHelper>, TerminalZeroWebClient.ServiceHelperReference.ServiceHelper {
        
        private BeginOperationDelegate onBeginGetLogsStrDelegate;
        
        private EndOperationDelegate onEndGetLogsStrDelegate;
        
        private System.Threading.SendOrPostCallback onGetLogsStrCompletedDelegate;
        
        private BeginOperationDelegate onBeginGetLogsDelegate;
        
        private EndOperationDelegate onEndGetLogsDelegate;
        
        private System.Threading.SendOrPostCallback onGetLogsCompletedDelegate;
        
        private BeginOperationDelegate onBeginOpenDelegate;
        
        private EndOperationDelegate onEndOpenDelegate;
        
        private System.Threading.SendOrPostCallback onOpenCompletedDelegate;
        
        private BeginOperationDelegate onBeginCloseDelegate;
        
        private EndOperationDelegate onEndCloseDelegate;
        
        private System.Threading.SendOrPostCallback onCloseCompletedDelegate;
        
        public ServiceHelperClient() {
        }
        
        public ServiceHelperClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public ServiceHelperClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceHelperClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public ServiceHelperClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
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
        
        public event System.EventHandler<GetLogsStrCompletedEventArgs> GetLogsStrCompleted;
        
        public event System.EventHandler<GetLogsCompletedEventArgs> GetLogsCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> OpenCompleted;
        
        public event System.EventHandler<System.ComponentModel.AsyncCompletedEventArgs> CloseCompleted;
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult TerminalZeroWebClient.ServiceHelperReference.ServiceHelper.BeginGetLogsStr(System.DateTime lastStamp, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginGetLogsStr(lastStamp, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Collections.ObjectModel.ObservableCollection<string> TerminalZeroWebClient.ServiceHelperReference.ServiceHelper.EndGetLogsStr(System.IAsyncResult result) {
            return base.Channel.EndGetLogsStr(result);
        }
        
        private System.IAsyncResult OnBeginGetLogsStr(object[] inValues, System.AsyncCallback callback, object asyncState) {
            System.DateTime lastStamp = ((System.DateTime)(inValues[0]));
            return ((TerminalZeroWebClient.ServiceHelperReference.ServiceHelper)(this)).BeginGetLogsStr(lastStamp, callback, asyncState);
        }
        
        private object[] OnEndGetLogsStr(System.IAsyncResult result) {
            System.Collections.ObjectModel.ObservableCollection<string> retVal = ((TerminalZeroWebClient.ServiceHelperReference.ServiceHelper)(this)).EndGetLogsStr(result);
            return new object[] {
                    retVal};
        }
        
        private void OnGetLogsStrCompleted(object state) {
            if ((this.GetLogsStrCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetLogsStrCompleted(this, new GetLogsStrCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GetLogsStrAsync(System.DateTime lastStamp) {
            this.GetLogsStrAsync(lastStamp, null);
        }
        
        public void GetLogsStrAsync(System.DateTime lastStamp, object userState) {
            if ((this.onBeginGetLogsStrDelegate == null)) {
                this.onBeginGetLogsStrDelegate = new BeginOperationDelegate(this.OnBeginGetLogsStr);
            }
            if ((this.onEndGetLogsStrDelegate == null)) {
                this.onEndGetLogsStrDelegate = new EndOperationDelegate(this.OnEndGetLogsStr);
            }
            if ((this.onGetLogsStrCompletedDelegate == null)) {
                this.onGetLogsStrCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetLogsStrCompleted);
            }
            base.InvokeAsync(this.onBeginGetLogsStrDelegate, new object[] {
                        lastStamp}, this.onEndGetLogsStrDelegate, this.onGetLogsStrCompletedDelegate, userState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.IAsyncResult TerminalZeroWebClient.ServiceHelperReference.ServiceHelper.BeginGetLogs(System.DateTime lastStamp, System.AsyncCallback callback, object asyncState) {
            return base.Channel.BeginGetLogs(lastStamp, callback, asyncState);
        }
        
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        System.Collections.ObjectModel.ObservableCollection<TerminalZeroWebClient.ServiceHelperReference.VirtualLogEntry> TerminalZeroWebClient.ServiceHelperReference.ServiceHelper.EndGetLogs(System.IAsyncResult result) {
            return base.Channel.EndGetLogs(result);
        }
        
        private System.IAsyncResult OnBeginGetLogs(object[] inValues, System.AsyncCallback callback, object asyncState) {
            System.DateTime lastStamp = ((System.DateTime)(inValues[0]));
            return ((TerminalZeroWebClient.ServiceHelperReference.ServiceHelper)(this)).BeginGetLogs(lastStamp, callback, asyncState);
        }
        
        private object[] OnEndGetLogs(System.IAsyncResult result) {
            System.Collections.ObjectModel.ObservableCollection<TerminalZeroWebClient.ServiceHelperReference.VirtualLogEntry> retVal = ((TerminalZeroWebClient.ServiceHelperReference.ServiceHelper)(this)).EndGetLogs(result);
            return new object[] {
                    retVal};
        }
        
        private void OnGetLogsCompleted(object state) {
            if ((this.GetLogsCompleted != null)) {
                InvokeAsyncCompletedEventArgs e = ((InvokeAsyncCompletedEventArgs)(state));
                this.GetLogsCompleted(this, new GetLogsCompletedEventArgs(e.Results, e.Error, e.Cancelled, e.UserState));
            }
        }
        
        public void GetLogsAsync(System.DateTime lastStamp) {
            this.GetLogsAsync(lastStamp, null);
        }
        
        public void GetLogsAsync(System.DateTime lastStamp, object userState) {
            if ((this.onBeginGetLogsDelegate == null)) {
                this.onBeginGetLogsDelegate = new BeginOperationDelegate(this.OnBeginGetLogs);
            }
            if ((this.onEndGetLogsDelegate == null)) {
                this.onEndGetLogsDelegate = new EndOperationDelegate(this.OnEndGetLogs);
            }
            if ((this.onGetLogsCompletedDelegate == null)) {
                this.onGetLogsCompletedDelegate = new System.Threading.SendOrPostCallback(this.OnGetLogsCompleted);
            }
            base.InvokeAsync(this.onBeginGetLogsDelegate, new object[] {
                        lastStamp}, this.onEndGetLogsDelegate, this.onGetLogsCompletedDelegate, userState);
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
        
        protected override TerminalZeroWebClient.ServiceHelperReference.ServiceHelper CreateChannel() {
            return new ServiceHelperClientChannel(this);
        }
        
        private class ServiceHelperClientChannel : ChannelBase<TerminalZeroWebClient.ServiceHelperReference.ServiceHelper>, TerminalZeroWebClient.ServiceHelperReference.ServiceHelper {
            
            public ServiceHelperClientChannel(System.ServiceModel.ClientBase<TerminalZeroWebClient.ServiceHelperReference.ServiceHelper> client) : 
                    base(client) {
            }
            
            public System.IAsyncResult BeginGetLogsStr(System.DateTime lastStamp, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[1];
                _args[0] = lastStamp;
                System.IAsyncResult _result = base.BeginInvoke("GetLogsStr", _args, callback, asyncState);
                return _result;
            }
            
            public System.Collections.ObjectModel.ObservableCollection<string> EndGetLogsStr(System.IAsyncResult result) {
                object[] _args = new object[0];
                System.Collections.ObjectModel.ObservableCollection<string> _result = ((System.Collections.ObjectModel.ObservableCollection<string>)(base.EndInvoke("GetLogsStr", _args, result)));
                return _result;
            }
            
            public System.IAsyncResult BeginGetLogs(System.DateTime lastStamp, System.AsyncCallback callback, object asyncState) {
                object[] _args = new object[1];
                _args[0] = lastStamp;
                System.IAsyncResult _result = base.BeginInvoke("GetLogs", _args, callback, asyncState);
                return _result;
            }
            
            public System.Collections.ObjectModel.ObservableCollection<TerminalZeroWebClient.ServiceHelperReference.VirtualLogEntry> EndGetLogs(System.IAsyncResult result) {
                object[] _args = new object[0];
                System.Collections.ObjectModel.ObservableCollection<TerminalZeroWebClient.ServiceHelperReference.VirtualLogEntry> _result = ((System.Collections.ObjectModel.ObservableCollection<TerminalZeroWebClient.ServiceHelperReference.VirtualLogEntry>)(base.EndInvoke("GetLogs", _args, result)));
                return _result;
            }
        }
    }
}