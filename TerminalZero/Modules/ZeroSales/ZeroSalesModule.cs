using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces;

namespace ZeroSales
{
    public class ZeroSalesModule : ZeroModule
    {
        public ZeroSalesModule(ITerminal terminal)
            :base(terminal,7,"Operaciones referentes a ventas")
        {
            BuildPosibleActions();
        }

        #region Overrrides

        public override string[] GetFilesToSend()
        {
            return new string[] { };
        }

        public override void Init()
        {
            
        }

        #endregion

        #region Private Methods

        private void BuildPosibleActions()
        {
            var action = new ZeroAction(null, ActionType.MenuItem, "Operaciones@Ventas@Nueva", openSaleView);
            OwnerTerminal.Session.AddAction(action);
        }

        private void openSaleView()
        {
            ModuleNotificationEventArgs args = new ModuleNotificationEventArgs();
            args.ControlToShow = new Pages.CreateSaleView(OwnerTerminal,0);
            OnModuleNotifing(args);
        }

        #endregion


    }
}
