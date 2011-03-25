using System;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces;

namespace ZeroLogHandle
{
    public class ZeroLogHandleModule : ZeroModule, ILogBuilder
    {
        public ZeroLogHandleModule(ITerminal iCurrentTerminal)
            : base(iCurrentTerminal, 1, "Guarda un log detallado de las operaciones")
        {
            
        }

        public override string[] GetFilesToSend()
        {
            return new string[] { };
        }

        #region IModule Members

        public override void Init()
        {
            
        }
        #endregion

        #region ILogBuilder Members

        public void Add(string log)
        {

        }

        public void Add(Exception ex)
        {

        }

        #endregion

    }

    
}
