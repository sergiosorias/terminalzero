        using ZeroBusiness;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces;
using ZeroSales.Pages;

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
            var action = new ZeroAction( ActionType.MenuItem, Actions.OpenNewSaleView, openSaleView);
            OwnerTerminal.Session.AddAction(action);
        }

        private void openSaleView()
        {
            var args = new ModuleNotificationEventArgs
                           {
                               ControlToShow = new CreateSaleView(OwnerTerminal, 0)
                           };
            OnModuleNotifing(args);
        }

        #endregion


    }
}
