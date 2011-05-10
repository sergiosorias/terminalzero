using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ZeroBusiness;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.GlobalObjects;
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
            Terminal.Instance.Session.AddAction(new ZeroAction( ActionType.MenuItem, Actions.OpenCurrentStockView, openStockView));
            Terminal.Instance.Session.AddAction(new ZeroAction( ActionType.MainViewButton, Actions.OpenNewStockView, openNewStockView, Rules.IsTerminalZero));
            Terminal.Instance.Session.AddAction(new ZeroAction( ActionType.MainViewButton, Actions.OpenModifyStockView, openModifyStockView, Rules.IsTerminalZero));
            Terminal.Instance.Session.AddAction(new ZeroAction( ActionType.MainViewButton, Actions.OpenDeliveryNoteView, OpenDeliveryNoteView, Rules.IsTerminalZero));
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
            var PackReceived = new ZeroStockPackMaganer(Terminal.Instance);
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

        void PackReceived_Imported(object sender, PackProcessingEventArgs e)
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
                var manager = new ZeroStockPackMaganer(Terminal.Instance);
                using (var ent = BusinessContext.CreateTemporaryManager(manager))
                {
                    var info = new ExportEntitiesPackInfo(ModuleCode, WorkingDirectory);
                    info.TerminalToCodes.AddRange(
                        ent.GetExportTerminal(Terminal.Instance.TerminalCode).Where(
                            t => t.IsTerminalZero && t.Code != Terminal.Instance.TerminalCode).Select(t => t.Code));

                    info.AddTable(ent.StockHeaders.Where(h => h.Status == (int)EntityStatus.New));
                    info.AddTable(ent.StockItems.Where(h => h.Status == (int)EntityStatus.New));
                    info.AddTable(ent.DeliveryDocumentHeaders.Where(h => h.Status == (int)EntityStatus.New));
                    info.AddTable(ent.DeliveryDocumentItems.Where(h => h.Status == (int) EntityStatus.New));

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
            finally
            {
                
            }
        }

        #region Handlers

        private void openStockView()
        {
            BusinessContext.Instance.BeginOperation();
            var view = new CurrentStockView();
            OnModuleNotifing(new ModuleNotificationEventArgs { ControlToShow = view });
        }

        private void openNewStockView()
        {
            BusinessContext.Instance.BeginOperation();
            var view = new CreateStockView(0);
            OnModuleNotifing(new ModuleNotificationEventArgs { ControlToShow = view });
        }

        private void openModifyStockView()
        {
            BusinessContext.Instance.BeginOperation();
            var view = new CreateStockView(1);
            OnModuleNotifing(new ModuleNotificationEventArgs { ControlToShow = view });
        }

        private void OpenDeliveryNoteView()
        {
            BusinessContext.Instance.BeginOperation();
            var view = new DeliveryDocumentView();
            OnModuleNotifing(new ModuleNotificationEventArgs { ControlToShow = view });
        }

        #endregion
    }
}
