using System;
using System.Linq;
using ZeroBusiness;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.Pack;
using ZeroSales.Pages;
using ZeroSales.Presentation;
using ZeroSales.Properties;

namespace ZeroSales
{
    public class ZeroSalesModule : ZeroModule
    {
        public ZeroSalesModule()
            : base(7, Resources.SalesModuleDescription)
        {
            BuildPosibleActions();
        }

        #region Overrides

        public override string[] GetFilesToSend()
        {
            TryExportSaleDataPack();
            return PackManager.GetPacks(ModuleCode, WorkingDirectory);
        }

        private void TryExportSaleDataPack()
        {
            try
            {
                var manager = new ZeroSalesPackManager(Terminal.Instance);
                using (var modelManager = BusinessContext.CreateTemporaryModelManager(manager))
                {
                    var info = new ExportEntitiesPackInfo(ModuleCode, WorkingDirectory);
                    info.TerminalToCodes.AddRange(
                        modelManager.GetExportTerminal(Terminal.Instance.TerminalCode).Where(
                            t => t.IsTerminalZero && t.Code != Terminal.Instance.TerminalCode).Select(t => t.Code));

                    info.AddExportableEntities(modelManager.SaleHeaders);
                    info.AddExportableEntities(modelManager.SaleItems);
                    info.AddExportableEntities(modelManager.SalePaymentHeaders);
                    info.AddExportableEntities(modelManager.SalePaymentItems);

                    if (info.SomeEntityHasRows)
                    {
                        using (manager)
                        {
                            if (manager.Export(info))
                            {
                                modelManager.SaveChanges();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Terminal.Instance.CurrentClient.Notifier.SetUserMessage(true, ex.ToString());
            }
        }

        public override void Init()
        {
           
        }

        #endregion

        #region Private Methods

        private void BuildPosibleActions()
        {
            Terminal.Instance.Session.Actions.Add(new ZeroAction(Actions.OpenCurrentSalesView, OpenCurrentSalesView));
            Terminal.Instance.Session.Actions.Add(new ZeroAction(Actions.OpenNewSaleView, OpenSaleView));
        }

        private void OpenSaleView(object parameter)
        {
            BusinessContext.Instance.BeginOperation();
            var viewModel = new CreateSaleViewModel(new CreateSaleView(), 0);
            Terminal.Instance.CurrentClient.ShowView(viewModel.View);
        }

        private void OpenCurrentSalesView(object parameter)
        {
            ///TODO:
            //BusinessContext.Instance.BeginOperation();
            //var view = new CreateSaleView();
            //Terminal.Instance.CurrentClient.ShowView(view);
        }

        #endregion


    }
}
