using System;
using System.Diagnostics;
using System.Linq;
using ZeroBusiness;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.Pack;
using ZeroSales.Pages;
using ZeroSales.Presentation;
using ZeroSales.Properties;
using System.IO;

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

        public override void NewPackReceived(string path)
        {
            base.NewPackReceived(path);
            var PackReceived = new ZeroSalesPackManager(Terminal.Instance);
            PackReceived.Imported += (o, e) =>
            {
                try
                {
                    File.Delete(path);
                }
                catch
                {
                    Terminal.Instance.CurrentClient.Notifier.Log(TraceLevel.Verbose, string.Format("Error deleting pack imported. Module = {0}, Path = {1}", ModuleCode, path));
                }
            };
            PackReceived.Imported += (o,e)=> Terminal.Instance.CurrentClient.Notifier.Log(TraceLevel.Info,
                                                                                          string.Format(
                                                                                              "Import Finished: Status = {3}, ConnID = {0}, DB Pack = {1}, Pack Module = {2}",
                                                                                              e.ConnectionID, e.Pack.Code,
                                                                                              e.PackInfo != null
                                                                                                  ? e.PackInfo.ModuleCode
                                                                                                  : -1,
                                                                                              e.Pack.PackStatusCode));
            PackReceived.Error += (o, e) => Terminal.Instance.CurrentClient.Notifier.Log(TraceLevel.Error, e.GetException().ToString());
            PackReceived.Import(path);
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
            Terminal.Instance.Session.Actions.Add(new ZeroAction(Actions.OpenSaleStatictics, OpenSaleStatictics));
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

        private void OpenSaleStatictics(object parameter)
        {
            BusinessContext.Instance.BeginOperation();
            var viewModel = new SaleStatisticsViewModel();
            Terminal.Instance.CurrentClient.ShowView(viewModel.View);
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

                    info.AddTable(modelManager.SaleHeaders);
                    info.AddTable(modelManager.SaleItems);
                    info.AddTable(modelManager.SalePaymentHeaders);
                    info.AddTable(modelManager.SalePaymentItems);
                    
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

        #endregion


    }
}
