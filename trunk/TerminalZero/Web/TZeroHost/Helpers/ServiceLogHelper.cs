using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ZeroCommonClasses.GlobalObjects;
using System.Text;

namespace TZeroHost.Helpers
{
    public class ServiceLogHelper : IDisposable
    {
        private object[] Parameters;
        private string ConnID;
        private string Method;
        public int TerminalCode;

        public string StatusMessage;
        public bool IsValid { get; private set; }

        public ServiceLogHelper(string method, string connID, params object[] parameters)
        {
            TerminalCode = -1;
            ConnID = connID;
            Method = method;
            Parameters = parameters;
        }

        public void Handle(Action action)
        {
            System.Diagnostics.Trace.Indent();
            try
            {
                DateTime stamp = DateTime.Now;
                action.Invoke();
                IsValid = true;
                System.Diagnostics.Trace.Unindent();
                System.Diagnostics.Trace.WriteLineIf(ZeroCommonClasses.Context.ContextBuilder.LogLevel.TraceInfo,  string.Format("Terminal: {0}, Service Method -> {1}, Duration: {2}", TerminalCode, Method, DateTime.Now-stamp), "Information");
            }
            catch (Exception ex)
            {
                IsValid = false;
                StatusMessage = string.Format("{1} ERROR, ",Method,ex);
                StringBuilder messageFormat = new StringBuilder();
                if (Parameters != null)
                {
                    
                    for (int i = 1; i < Parameters.Length + 1; i++)
                    {
                        messageFormat.AppendFormat("{0} - {1}",i, Parameters[i-1]).AppendLine();
                    }
                }

                System.Diagnostics.Trace.WriteLineIf(ZeroCommonClasses.Context.ContextBuilder.LogLevel.TraceError,string.Format("Terminal: {0}, Service Method -> {1}, Parameters: {2}, throw {3}", TerminalCode, Method, messageFormat, ex), "Error");
            }

            System.Diagnostics.Trace.Unindent();

        }

        #region IDisposable Members

        public void Dispose()
        {
            
        }

        #endregion
    }
}