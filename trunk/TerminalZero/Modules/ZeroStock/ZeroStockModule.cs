using System;
using System.Collections.Generic;
using System.Linq;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.PackClasses;
using ZeroStock.Entities;
using ZeroStock.Pages;

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
            Terminal.Session.AddAction( new ZeroAction(null,ActionType.MainViewButton, "Operaciones@Stock@Alta", openNewStockView));
            Terminal.Session.AddAction( new ZeroAction(null,ActionType.MainViewButton, "Operaciones@Stock@Baja", openModifyStockView));
            Terminal.Session.AddAction(new ZeroAction(null,ActionType.MainViewButton, "Operaciones@Remitos", openModifyStockView));
        }

        public override void Init()
        {
            
        }

        public override string[] GetFilesToSend()
        {
            TryExportStockDataPack();
            return PackManager.GetPacks(ModuleCode, WorkingDirectory);
        }

        private void TryExportStockDataPack()
        {
            try
            {
                using (StockEntities ent = new StockEntities())
                {
                    ZeroCommonClasses.PackClasses.ExportEntitiesPackInfo info = new ExportEntitiesPackInfo(ModuleCode, WorkingDirectory);
                    IEnumerable<StockHeader> headers = ent.StockHeaders.Where(h => h.Status == (int)StockEntities.EntitiesStatus.New);
                    IEnumerable<StockItem> stockItems = null;
                    if (headers.Count() > 0)
                    {
                        stockItems = ent.StockItems.Where(it => it.Status == (int)StockEntities.EntitiesStatus.New);
                        info.AddTable(stockItems);
                        info.AddTable(headers);
                    }

                    IEnumerable<DeliveryDocumentHeader> Delheaders = ent.DeliveryDocumentHeaders.Where(h => h.Status == (int)StockEntities.EntitiesStatus.New);
                    IEnumerable<DeliveryDocumentItem> DelItems = null;
                    if (Delheaders.Count() > 0)
                    {
                        DelItems = ent.DeliveryDocumentItems.Where(it => it.Status == (int)StockEntities.EntitiesStatus.New);
                        info.AddTable(Delheaders);
                        info.AddTable(DelItems);
                    }

                    if (info.TableCount > 0)
                    {
                        using (ZeroStockPackMaganer manager = new ZeroStockPackMaganer(info))
                        {
                            if (manager.Process())
                            {
                                if (info.Tables.Exists(t=>t.RowType ==  typeof(DeliveryDocumentHeader)))
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
            CurrentStockView view = new CurrentStockView();
            OnModuleNotifing(new ModuleNotificationEventArgs { ControlToShow = view });
        }

        private void openNewStockView()
        {
            StockView view = new StockView(Terminal, 0);
            OnModuleNotifing(new ModuleNotificationEventArgs { ControlToShow = view });
        }

        private void openModifyStockView()
        {
            StockView view = new StockView(Terminal, 1);
            OnModuleNotifing(new ModuleNotificationEventArgs { ControlToShow = view });
        }

        #endregion
    }
}
