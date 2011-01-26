using System;
using System.Diagnostics;
using System.Text;
using ZeroCommonClasses.Context;

namespace TZeroHost.Helpers
{
    public class ServiceLogHelper : IDisposable
    {
        private object[] Parameters;
        private string _connId;
        private string Method;
        public int TerminalCode;

        public string StatusMessage;
        public bool IsValid { get; private set; }

        public ServiceLogHelper(string method, string connID, params object[] parameters)
        {
            TerminalCode = -1;
            _connId = connID;
            Method = method;
            Parameters = parameters;
        }

        public void Handle(Action action)
        {
            Trace.Indent();
            try
            {
                DateTime stamp = DateTime.Now;
                action.Invoke();
                IsValid = true;
                Trace.Unindent();
                Trace.WriteLineIf(ContextBuilder.LogLevel.TraceInfo,  string.Format("Terminal: {0}, Service Method -> {1}, Duration: {2}", TerminalCode, Method, DateTime.Now-stamp), "Information");
            }
            catch (Exception ex)
            {
                IsValid = false;
                StatusMessage = string.Format("{0} ERROR {1}, ",Method,ex);
                var messageFormat = new StringBuilder();
                if (Parameters != null)
                {
                    
                    for (int i = 1; i < Parameters.Length + 1; i++)
                    {
                        messageFormat.AppendFormat("{0} - {1}",i, Parameters[i-1]).AppendLine();
                    }
                }

                Trace.WriteLineIf(ContextBuilder.LogLevel.TraceError,string.Format("Terminal: {0}, Service Method -> {1}, Parameters: {2}, throw {3}", TerminalCode, Method, messageFormat, ex), "Error");
            }

            Trace.Unindent();

        }

        #region IDisposable Members

        public void Dispose()
        {
            
        }

        #endregion
    }
}