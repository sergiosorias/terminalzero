﻿using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ZeroBusiness;
using ZeroCommonClasses;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Pack;
using ZeroStock.Entities;
using ZeroStock.Pages;
using ZeroStock.Properties;

namespace ZeroStock
{
    public class ZeroStockModule : ZeroModule
    {
        public ZeroStockModule(ITerminal currentTerminal)
            : base(currentTerminal, 4, Resources.StockModuleDescription)
        {
            BuildPosibleActions();
        }

        private void BuildPosibleActions()
        {
            OwnerTerminal.Session.AddAction(new ZeroAction( ActionType.MenuItem, Actions.OpenCurrentStockView, openStockView));
            OwnerTerminal.Session.AddAction(new ZeroAction( ActionType.MainViewButton, Actions.OpenNewStockView, openNewStockView, Rules.IsTerminalZero));
            OwnerTerminal.Session.AddAction(new ZeroAction( ActionType.MainViewButton, Actions.OpenModifyStockView, openModifyStockView, Rules.IsTerminalZero));
            OwnerTerminal.Session.AddAction(new ZeroAction( ActionType.MainViewButton, Actions.OpenDeliveryNoteView, OpenDeliveryNoteView, Rules.IsTerminalZero));
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
            var PackReceived = new ZeroStockPackMaganer(OwnerTerminal);
            PackReceived.Imported += (o, e) =>
            {
                try
                {
                    File.Delete(path);
                }
                catch
                {
                    OwnerTerminal.Session.Notifier.Log(TraceLevel.Verbose, string.Format(
                                                    "Error deleting pack imported. Module = {0}, Path = {1}",
                                                    ModuleCode, path));
                }
            };
            PackReceived.Imported += PackReceived_Imported;
            PackReceived.Error += (o, e) => OwnerTerminal.Session.Notifier.Log(TraceLevel.Error, e.GetException().ToString());
            PackReceived.Import(path);

        }

        void PackReceived_Imported(object sender, PackEventArgs e)
        {
            OwnerTerminal.Session.Notifier.Log(TraceLevel.Info,
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
                using (var ent = new StockEntities())
                {
                    var info = new ExportEntitiesPackInfo(ModuleCode, WorkingDirectory);
                    info.TerminalToCodes.AddRange(
                        ent.GetExportTerminal(OwnerTerminal.TerminalCode).Where(
                            t => t.IsTerminalZero && t.Code != OwnerTerminal.TerminalCode).Select(t => t.Code));

                    info.AddTable(ent.StockHeaders.Where(h => h.Status == (int)EntityStatus.New));
                    info.AddTable(ent.StockItems.Where(h => h.Status == (int)EntityStatus.New));
                    info.AddTable(ent.DeliveryDocumentHeaders.Where(h => h.Status == (int)EntityStatus.New));
                    info.AddTable(ent.DeliveryDocumentItems.Where(h => h.Status == (int) EntityStatus.New));

                    if (info.TableCount > 0)
                    {
                        foreach (PackTableInfo packTableInfo in info.Tables)
                        {
                            foreach (IExportableEntity exportable in packTableInfo.GetRowsAs<IExportableEntity>())
                            {
                                exportable.UpdateStatus(EntityStatus.Exported);
                                if (!info.TerminalToCodes.Contains(exportable.TerminalDestination))
                                {
                                    info.TerminalToCodes.Add(exportable.TerminalDestination);
                                }
                            }
                        }
                        
                        using (var manager = new ZeroStockPackMaganer(OwnerTerminal))
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
                OwnerTerminal.Session.Notifier.SetUserMessage(true, ex.ToString());
            }
        }

        #region Handlers

        private void openStockView()
        {
            var view = new CurrentStockView(OwnerTerminal);
            OnModuleNotifing(new ModuleNotificationEventArgs { ControlToShow = view });
        }

        private void openNewStockView()
        {
            var view = new CreateStockView(OwnerTerminal, 0);
            OnModuleNotifing(new ModuleNotificationEventArgs { ControlToShow = view });
        }

        private void openModifyStockView()
        {
            var view = new CreateStockView(OwnerTerminal, 1);
            OnModuleNotifing(new ModuleNotificationEventArgs { ControlToShow = view });
        }

        private void OpenDeliveryNoteView()
        {
            var view = new DeliveryDocumentView(OwnerTerminal);
            OnModuleNotifing(new ModuleNotificationEventArgs { ControlToShow = view });
        }

        #endregion
    }
}
