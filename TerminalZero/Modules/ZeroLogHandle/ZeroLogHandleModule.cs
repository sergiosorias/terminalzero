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

        private bool _TryUseSameContext = false;
        public bool TryUseSameContext
        {
            get
            {
                return _TryUseSameContext;

            }
            set
            {
                _TryUseSameContext = true;
            }
        }

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
