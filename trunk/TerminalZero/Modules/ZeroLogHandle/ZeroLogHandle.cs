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
    public class ZeroLogHandle : ZeroModule, ILogBuilder
    {
        public ZeroLogHandle(ITerminal iCurrentTerminal)
            : base(iCurrentTerminal, 1, "Guarda un log detallado de las operaciones")
        {
            
        }

        public override void BuildPosibleActions(List<ZeroAction> actions)
        {
            //ZeroAction openLog = new ZeroCommonClasses.ZeroAction(ZeroCommonClasses.ActionType.MenuItem, "Log@Abrir..",OpenLog, "ValidateUser");
            //actions.Add(openLog);
        }

        public override void BuildRulesActions(List<ZeroRule> rules)
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

        public void SetState()
        {

        }

        public void Add(string log)
        {

        }

        public void Add(Exception ex)
        {

        }

        #endregion

    }
}
