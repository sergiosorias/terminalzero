using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses;
using System.Windows.Controls;
using ZeroCommonClasses.GlobalObjects;

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

        private void OpenLog(ZeroRule rule)
        {

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
