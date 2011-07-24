using System;
using System.Diagnostics;
using System.IO;
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
        public const int Code = 7;

        public ZeroSalesModule()
            : base(Code, Resources.SalesModuleDescription)
        {
            
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
                    Terminal.Instance.Client.Notifier.Log(TraceLevel.Verbose, string.Format("Error deleting pack imported. Module = {0}, Path = {1}", ModuleCode, path));
                }
            };
            PackReceived.Imported += (o,e)=> Terminal.Instance.Client.Notifier.Log(TraceLevel.Info,
                                                                                          string.Format(
                                                                                              "Import Finished: Status = {3}, ConnID = {0}, DB Pack = {1}, Pack Module = {2}",
                                                                                              e.ConnectionID, e.Pack.Code,
                                                                                              e.PackInfo != null
                                                                                                  ? e.PackInfo.ModuleCode
                                                                                                  : -1,
                                                                                              e.Pack.PackStatusCode));
            PackReceived.Error += (o, e) => Terminal.Instance.Client.Notifier.Log(TraceLevel.Error, e.GetException().ToString());
            PackReceived.Import(path);
        }

        public override void Initialize()
        {
           
        }

        #endregion

        #region Private Methods

        [ZeroAction(Actions.OpenNewSaleView)]
        private void OpenSaleView(object parameter)
        {
            BusinessContext.Instance.BeginOperation();
            var viewModel = new CreateSaleViewModel(new CreateSaleView(), 0);
            Terminal.Instance.Client.ShowView(viewModel.View);
        }

        [ZeroAction(Actions.OpenCurrentSalesView)]
        private void OpenCurrentSalesView(object parameter)
        {
            BusinessContext.Instance.BeginOperation();
            var viewModel = new SaleReportViewModel();
            Terminal.Instance.Client.ShowView(viewModel.View);
        }
        
        [ZeroAction(Actions.OpenSaleStatictics,Rules.IsTerminalZero)]
        private void OpenSaleStatictics(object parameter)
        {
            BusinessContext.Instance.BeginOperation();
            var viewModel = new SaleStatisticsViewModel();
            Terminal.Instance.Client.ShowView(viewModel.View);
        }

        private void TryExportSaleDataPack()
        {
            try
            {
                new ZeroSalesPackManager().Export(WorkingDirectory);
            }
            catch (Exception ex)
            {
                Terminal.Instance.Client.Notifier.SetUserMessage(true, ex.ToString());
            }
        }

        #endregion


    }
}
