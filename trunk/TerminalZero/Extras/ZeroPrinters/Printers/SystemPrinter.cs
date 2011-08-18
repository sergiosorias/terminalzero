using System;
using System.Collections.Generic;

namespace ZeroPrinters.Printers
{
    public enum PrinterType
    {
        General = 1,
        Legal = 2,
        TextOnly = 4
    }

    public abstract class SystemPrinter : IDisposable
    {
        public string Name { get; protected set; }
        public abstract bool IsOnLine { get; protected set; }
        
        private string lastError;
        public string LastError
        {
            get
            {
                return lastError;
            }
            protected set
            {
                lastError = value;
            }
        }

        public bool HasError 
        { 
            get { return !string.IsNullOrWhiteSpace(LastError); } 
        }

        protected SystemPrinter(PrinterInfo info)
        {
            Name = info.Name;
        }

        public virtual void Print()
        {
            
        }

        public virtual void Clear()
        {
            LastError = string.Empty;
        }
        
        #region IDisposable Members

        public virtual void Dispose()
        {
            
        }

        #endregion
        
    }
}