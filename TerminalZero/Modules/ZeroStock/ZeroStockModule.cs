using System;
using System.Diagnostics;
using System.IO;
using ZeroBusiness;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.Pack;
using ZeroStock.Pages;
using ZeroStock.Presentation;
using ZeroStock.Properties;

namespace ZeroStock
{
    public class ZeroStockModule : ZeroModule
    {
        public const int Code = 4;

        public ZeroStockModule()
            : base(Code, Resources.StockModuleDescription)
        {
            BuildActions();
        }

        private void BuildActions()
        {
            var createStockFromSale = new ZeroTriggerAction(Actions.ExecCreateStockFromLastSale, CreateStockFromSale);
            createStockFromSale.AddParam(typeof(SaleHeader), true);
            Terminal.Instance.Session.Actions.Add(createStockFromSale);
        }

        public override void Initialize()
        {

        }

        public override string[] GetFilesToSend()
        {
            TryExportStockDataPack();
            return PackManager.GetPacks(ModuleCode, WorkingDirectory);
        }

        public override void NewPackReceived(string path)
        {
            base.NewPackReceived(path);
            var PackReceived = new ZeroStockPackManager();
            PackReceived.Imported += (o, e) =>
            {
                try
                {
                    File.Delete(path);
                }
                catch
                {
                    Terminal.Instance.Client.Notifier.Log(TraceLevel.Verbose, string.Format(
                                                    "Error deleting pack imported. Module = {0}, Path = {1}",
                                                    ModuleCode, path));
                }
            };
            PackReceived.Imported += PackReceived_Imported;
            PackReceived.Error += (o, e) => Terminal.Instance.Client.Notifier.Log(TraceLevel.Error, e.GetException().ToString());
            PackReceived.Import(path);

        }

        private void PackReceived_Imported(object sender, PackProcessEventArgs e)
        {
            Terminal.Instance.Client.Notifier.Log(TraceLevel.Info,
                                          string.Format(
                                              "Import Finished: Status = {3}, ConnID = {0}, DB Pack = {1}, Pack Module = {2}",
                                              e.ConnectionID, e.Pack.Code,
                                              e.PackInfo != null
                                                  ? e.PackInfo.ModuleCode
                                                  : -1,
                                              e.Pack.PackStatusCode));

        }

        private void TryExportStockDataPack()
        {
            try
            {
                new ZeroStockPackManager().Export(WorkingDirectory);
            }
            catch (Exception ex)
            {
                Terminal.Instance.Client.Notifier.SetUserMessage(true, ex.ToString());
            }
        }

        #region Handlers

        [ZeroAction(Actions.OpenCurrentStockView)]
        private void OpenStockView(object parameter)
        {
            BusinessContext.Instance.BeginOperation();
            var view = new CurrentStockView();
            Terminal.Instance.Client.ShowView(view);
        }

        [ZeroAction(Actions.OpenNewStockView, Rules.IsTerminalZero)]
        private void OpenNewStockView(object parameter)
        {
            BusinessContext.Instance.BeginOperation();
            var view = new CreateStockViewModel(StockType.Types.New);
            Terminal.Instance.Client.ShowView(view.View);
        }

        [ZeroAction(Actions.OpenModifyStockView, Rules.IsTerminalZero)]
        private void OpenModifyStockView(object parameter)
        {
            BusinessContext.Instance.BeginOperation();
            var view = new CreateStockViewModel(StockType.Types.Modify);
            Terminal.Instance.Client.ShowView(view.View);
        }

        [ZeroAction(Actions.OpenDeliveryNoteView, Rules.IsTerminalZero)]
        private void OpenDeliveryNoteView(object parameter)
        {
            BusinessContext.Instance.BeginOperation();
            var view = new DeliveryDocumentView();
            Terminal.Instance.Client.ShowView(view);
        }
        
        private void CreateStockFromSale(object parameter)
        {
            var header =  Terminal.Instance.Session[typeof (SaleHeader)].Value as SaleHeader;

            var stockNew = new StockHeader(StockType.Types.Modify,Terminal.Instance.Code);

            foreach (SaleItem item in header.SaleItems)
            {
                stockNew.AddNewStockItem(item.Product,item.Quantity,item.Batch);    
            }

            //BusinessContext.Instance.Model.AddToStockHeaders(stockNew);

        }

        #endregion
    }
}
