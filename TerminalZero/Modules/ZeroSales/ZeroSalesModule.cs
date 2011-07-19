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
        public const int Code = 7;

        public ZeroSalesModule()
            : base(Code, Resources.SalesModuleDescription)
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
            var PackReceived = new ZeroSalesPackManager();
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

        public override void Initialize()
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
            BusinessContext.Instance.BeginOperation();
            SaleReportViewModel viewModel = new SaleReportViewModel();
            Terminal.Instance.CurrentClient.ShowView(viewModel.View);
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
                new ZeroSalesPackManager().Export(WorkingDirectory);
            }
            catch (Exception ex)
            {
                Terminal.Instance.CurrentClient.Notifier.SetUserMessage(true, ex.ToString());
            }
        }

        #endregion


    }
}
