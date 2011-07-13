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
        protected SystemPrinter(PrinterInfo info)
        {
            Name = info.Name;
            Parameters = new Dictionary<string, string>();
        }

        public string Name { get; internal set; }
        public abstract bool IsOnLine { get; protected set; }
        public bool IsExistanceMandatory { get; protected set; }
        public Dictionary<string,string> Parameters { get; private set; }

        public virtual void Print()
        {
            
        }

        public virtual void CancelPrint()
        {
            
        }
        
        #region IDisposable Members

        public virtual void Dispose()
        {
            
        }

        #endregion
        
    }
}