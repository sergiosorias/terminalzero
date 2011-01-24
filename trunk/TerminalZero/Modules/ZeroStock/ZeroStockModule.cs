using System;
using System.Collections.Generic;
using System.Linq;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Pack;
using ZeroGUI;
using ZeroStock.Entities;
using ZeroStock.Pages;
using ZeroStock.Properties;

namespace ZeroStock
{
    public class ZeroStockModule : ZeroModule
    {
        public ZeroStockModule(ITerminal currentTerminal)
            : base(currentTerminal, 4, "Operaciones referentes al stock de productos")
        {
            BuildPosibleActions();
        }

        private void BuildPosibleActions()
        {
            Terminal.Session.AddAction( new ZeroAction(null,ActionType.MenuItem, "Operaciones@Stock@Actual", openStockView));
            Terminal.Session.AddAction(new ZeroAction(null, ActionType.MainViewButton, "Operaciones@Stock@Alta", openNewStockView, "ValidateTerminalZero"));
            Terminal.Session.AddAction( new ZeroAction(null,ActionType.MainViewButton, "Operaciones@Stock@Baja", openModifyStockView));
            Terminal.Session.AddAction(new ZeroAction(null, ActionType.MainViewButton, "Operaciones@Remitos de salida", OpenDeliveryNoteView, "ValidateTerminalZero"));
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
            var PackReceived = new ZeroStockPackMaganer(Terminal);
            PackReceived.Imported += (o, e) =>
            {
                try
                {
                    System.IO.File.Delete(path);
                }
                catch
                {
                    Terminal.Session.Notifier.Log(System.Diagnostics.TraceLevel.Verbose, string.Format(
                                                    "Error deleting pack imported. Module = {0}, Path = {1}",
                                                    ModuleCode, path));
                }
            };
            PackReceived.Imported+=PackReceived_Imported;
            PackReceived.Error += (o,e)=> Terminal.Session.Notifier.Log(System.Diagnostics.TraceLevel.Error, e.GetException().ToString());
            PackReceived.Import(path);

        }

        void PackReceived_Imported(object sender, PackEventArgs e)
        {
            Terminal.Session.Notifier.Log(System.Diagnostics.TraceLevel.Info,
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
                    foreach (TerminalTo terminalTo in ent.TerminalToes.Where(tt=>tt.Code!=Terminal.TerminalCode).ToList())
                    {
                        var info = new ExportEntitiesPackInfo(ModuleCode, WorkingDirectory);
                        info.TerminalToCodes.Add(terminalTo.Code);
                        List<StockHeader> headers = ent.StockHeaders.Where(h => h.TerminalToCode == terminalTo.Code && h.Status == (int)StockEntities.EntitiesStatus.New).ToList();
                        List<StockItem> stockItems = new List<StockItem>();
                        if (headers.Count() > 0)
                        {
                            foreach (StockHeader header in headers)
                            {
                                stockItems.AddRange(header.StockItems);
                                info.AddTable(stockItems);
                                info.AddTable(headers);    
                            }
                            
                        }

                        List<DeliveryDocumentHeader> Delheaders = ent.DeliveryDocumentHeaders.Where(h => h.TerminalToCode == terminalTo.Code && h.Status == (int)StockEntities.EntitiesStatus.New).ToList();
                        List<DeliveryDocumentItem> DelItems = null;
                        if (Delheaders.Count() > 0)
                        {
                            DelItems = new List<DeliveryDocumentItem>();
                            foreach (DeliveryDocumentHeader deliveryDocumentHeader in Delheaders)
                            {
                                DelItems.AddRange(deliveryDocumentHeader.DeliveryDocumentItems);
                                info.AddTable(Delheaders);
                                info.AddTable(DelItems);
                            }
                        }

                        if (info.TableCount > 0)
                        {
                            using (var manager = new ZeroStockPackMaganer(Terminal))
                            {
                                if (manager.Export(info))
                                {
                                    if (info.Tables.Exists(t => t.RowType == typeof(DeliveryDocumentHeader)))
                                    {
                                        UpdatteDeliveryDocumentStatus(Delheaders, DelItems);

                                    }
                                    if (info.Tables.Exists(t => t.RowType == typeof(StockHeader)))
                                    {
                                        UpdateStockStatus(headers, stockItems);
                                    }
                                    ent.SaveChanges();
                                }
                            }
                        }
                    }
               }
            }
            catch (Exception ex)
            {
                Terminal.Session.Notifier.SetUserMessage(true, ex.ToString());
            }
        }

        private static void UpdatteDeliveryDocumentStatus(IEnumerable<DeliveryDocumentHeader> Delheaders, IEnumerable<DeliveryDocumentItem> DelItems)
        {

            foreach (var header in Delheaders)
            {
                header.Stamp = DateTime.Now;
                header.Status = (int)StockEntities.EntitiesStatus.Exported;
            }
            foreach (var sitem in DelItems)
            {
                sitem.Stamp = DateTime.Now;
                sitem.Status = (int)StockEntities.EntitiesStatus.Exported;
            }

        }

        private static void UpdateStockStatus(IEnumerable<StockHeader> headers, IEnumerable<StockItem> stockItems)
        {
            foreach (var header in headers)
            {
                header.Stamp = DateTime.Now;
                header.Status = (int)StockEntities.EntitiesStatus.Exported;
            }
            foreach (var sitem in stockItems)
            {
                sitem.Stamp = DateTime.Now;
                sitem.Status = (int)StockEntities.EntitiesStatus.Exported;
            }

        }
        
        #region Handlers

        private void openStockView()
        {
            var view = new CurrentStockView(Terminal);
            OnModuleNotifing(new ModuleNotificationEventArgs { ControlToShow = view });
        }

        private void openNewStockView()
        {
            var view = new StockView(Terminal, 0);
            OnModuleNotifing(new ModuleNotificationEventArgs { ControlToShow = view });
        }

        private void openModifyStockView()
        {
            var view = new StockView(Terminal, 1);
            OnModuleNotifing(new ModuleNotificationEventArgs { ControlToShow = view });
        }

        private void OpenDeliveryNoteView()
        {
            var view = new DeliveryDocumentView(Terminal);
            OnModuleNotifing(new ModuleNotificationEventArgs { ControlToShow = view });
        }

        #endregion
    }
}
