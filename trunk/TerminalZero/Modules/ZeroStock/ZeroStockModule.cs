using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ZeroBusiness;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.Pack;
using ZeroStock.Pages;
using ZeroStock.Properties;

namespace ZeroStock
{
    public class ZeroStockModule : ZeroModule
    {
        public ZeroStockModule()
            : base(4, Resources.StockModuleDescription)
        {
            BuildPosibleActions();
        }

        private void BuildPosibleActions()
        {
            Terminal.Instance.Session.AddAction(new ZeroAction( Actions.OpenCurrentStockView, OpenStockView, null, true));
            Terminal.Instance.Session.AddAction(new ZeroAction( Actions.OpenNewStockView, OpenNewStockView, Rules.IsTerminalZero,true));
            Terminal.Instance.Session.AddAction(new ZeroAction( Actions.OpenModifyStockView, OpenModifyStockView, Rules.IsTerminalZero,true));
            Terminal.Instance.Session.AddAction(new ZeroAction( Actions.OpenDeliveryNoteView, OpenDeliveryNoteView, Rules.IsTerminalZero,true));
            var createStockFromSale = new ZeroTriggerAction(Actions.ExecCreateStockFromLastSale, CreateStockFromSale,null);
            createStockFromSale.AddParam(typeof(SaleHeader),true);
            Terminal.Instance.Session.AddAction(createStockFromSale);
        }

        public override void Init()
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
            var PackReceived = new ZeroStockPackManager(Terminal.Instance);
            PackReceived.Imported += (o, e) =>
            {
                try
                {
                    File.Delete(path);
                }
                catch
                {
                    Terminal.Instance.CurrentClient.Notifier.Log(TraceLevel.Verbose, string.Format(
                                                    "Error deleting pack imported. Module = {0}, Path = {1}",
                                                    ModuleCode, path));
                }
            };
            PackReceived.Imported += PackReceived_Imported;
            PackReceived.Error += (o, e) => Terminal.Instance.CurrentClient.Notifier.Log(TraceLevel.Error, e.GetException().ToString());
            PackReceived.Import(path);

        }

        private void PackReceived_Imported(object sender, PackProcessingEventArgs e)
        {
            Terminal.Instance.CurrentClient.Notifier.Log(TraceLevel.Info,
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
                var manager = new ZeroStockPackManager(Terminal.Instance);
                using (var ent = BusinessContext.CreateTemporaryModelManager(manager))
                {
                    var info = new ExportEntitiesPackInfo(ModuleCode, WorkingDirectory);
                    info.TerminalToCodes.AddRange(
                        ent.GetExportTerminal(Terminal.Instance.TerminalCode).Where(
                            t => t.IsTerminalZero && t.Code != Terminal.Instance.TerminalCode).Select(t => t.Code));

                    info.AddExportableEntities(ent.StockHeaders);
                    info.AddExportableEntities(ent.StockItems);
                    info.AddExportableEntities(ent.DeliveryDocumentHeaders);
                    info.AddExportableEntities(ent.DeliveryDocumentItems);

                    if (info.SomeEntityHasRows)
                    {
                        using (manager)
                        {
                            if (manager.Export(info))
                            {
                                ent.SaveChanges();
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

        #region Handlers

        private void OpenStockView()
        {
            BusinessContext.Instance.BeginOperation();
            var view = new CurrentStockView();
            Terminal.Instance.CurrentClient.ShowView(view);
        }

        private void OpenNewStockView()
        {
            BusinessContext.Instance.BeginOperation();
            var view = new CreateStockView(0);
            Terminal.Instance.CurrentClient.ShowView(view);
        }

        private void OpenModifyStockView()
        {
            BusinessContext.Instance.BeginOperation();
            var view = new CreateStockView(1);
            Terminal.Instance.CurrentClient.ShowView(view);
        }

        private void OpenDeliveryNoteView()
        {
            BusinessContext.Instance.BeginOperation();
            var view = new DeliveryDocumentView();
            Terminal.Instance.CurrentClient.ShowView(view);
        }

        private void CreateStockFromSale()
        {
            SaleHeader header =  Terminal.Instance.Session[typeof (SaleHeader)].Value as SaleHeader;

            StockHeader stockNew =
                new StockHeader(BusinessContext.Instance.ModelManager.StockTypes.First(st => st.Code == 1),
                                Terminal.Instance.TerminalCode);

            foreach (SaleItem item in header.SaleItems)
            {
                stockNew.AddNewStockItem(item.Product,item.Quantity,item.Batch);    
            }

            BusinessContext.Instance.ModelManager.AddToStockHeaders(stockNew);

        }

        #endregion
    }
}
