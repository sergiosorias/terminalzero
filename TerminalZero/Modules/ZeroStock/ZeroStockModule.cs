using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces;
using ZeroStock.Pages;
using ZeroCommonClasses.PackClasses;
using ZeroStock.Entities;

namespace ZeroStock
{
    public class ZeroStockModule : ZeroModule
    {
        public ZeroStockModule(ITerminal currentTerminal)
            :base(currentTerminal,4,"Operaciones referentes al stock de productos")
        {

        }

        public override void BuildPosibleActions(List<ZeroAction> actions)
        {
            actions.Add(new ZeroAction(ActionType.MenuItem,"Operaciones@Stock@Actual",openStockView));
            actions.Add(new ZeroAction(ActionType.MainViewButton, "Operaciones@Stock@Alta", openNewStockView));
            actions.Add(new ZeroAction(ActionType.MainViewButton, "Operaciones@Stock@Baja", openModifyStockView));
        }

        public override void BuildRulesActions(List<ZeroRule> rules)
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
                                if (info.Tables.Exists(t=>t.Equals(typeof(StockHeader))))
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

                                if (info.Tables.Exists(t => t.Equals(typeof(DeliveryDocumentHeader))))
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
        
        public override void Init()
        {
            
        }

        #region Handlers

        private void openStockView(ZeroRule rule)
        {
            CurrentStockView view = new CurrentStockView();
            OnNotifing(new ModuleNotificationEventArgs { ControlToShow = view });
        }

        private void openNewStockView(ZeroRule rule)
        {
            StockView view = new StockView(Terminal,0);
            OnNotifing(new ModuleNotificationEventArgs { ControlToShow = view });
        }

        private void openModifyStockView(ZeroRule rule)
        {
            StockView view = new StockView(Terminal, 1);
            OnNotifing(new ModuleNotificationEventArgs { ControlToShow = view });
        }

        #endregion
    }
}
