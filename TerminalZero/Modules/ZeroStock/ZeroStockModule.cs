using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces;
using ZeroStock.Pages;

namespace ZeroStock
{
    public class ZeroStockModule : ZeroModule
    {
        public ZeroStockModule(ITerminal currentTerminal)
            :base(currentTerminal,3,"Operaciones referentes al stock de productos")
        {

        }

        public override void BuildPosibleActions(List<ZeroAction> actions)
        {
            actions.Add(new ZeroAction(ActionType.MenuItem,"Operaciones@Stock@Actual",openStockView));
            actions.Add(new ZeroAction(ActionType.MainViewButton, "Operaciones@Stock@Nuevo", openNewStockView));
        }

        public override void BuildRulesActions(List<ZeroRule> rules)
        {
            
        }

        public override string[] GetFilesToSend()
        {
            return new string[] { };
        }

        public override void Init()
        {
            
        }

        #region Handlers

        private void openStockView(ZeroRule rule)
        {
            
        }

        private void openNewStockView(ZeroRule rule)
        {
            NewStockView view = new NewStockView();
            OnNotifing(new ModuleNotificationEventArgs { ControlToShow = view });
        }

        
        #endregion
    }
}
