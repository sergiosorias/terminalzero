using System;
using System.Collections.Generic;
using System.Linq;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Pack;

namespace ZeroSales
{
    public class ZeroSalesPackManager : PackManager
    {
        public ZeroSalesPackManager(ITerminal terminal)
            :base(terminal)
        {
            
        }

        protected override void ExportProcess(PackProcessingEventArgs args)
        {
            ((ExportEntitiesPackInfo)args.PackInfo).ExportTables();
        }
        
        protected override void ImportProcess(PackProcessingEventArgs args)
        {
            args.Pack.IsMasterData = false;
            args.Pack.IsUpgrade = false;
            ImportEntities(args);
        }
        
        private void ImportEntities(PackProcessingEventArgs e)
        {
            var packInfo = (ExportEntitiesPackInfo)e.PackInfo;
            using (var modelManager = BusinessContext.CreateTemporaryModelManager(this))
            {
                if (packInfo.ContainsTable<SaleHeader>())
                {
                    ImportSaleHeader(modelManager, packInfo.GetTable<SaleHeader>());
                    if (packInfo.ContainsTable<SaleItem>()) ImportSaleItem(modelManager, packInfo.GetTable<SaleItem>());
                }
                if (packInfo.ContainsTable<SalePaymentHeader>())
                {
                    ImportSalePaymentHeader(modelManager, packInfo.GetTable<SalePaymentHeader>());
                    if (packInfo.ContainsTable<SalePaymentItem>()) ImportSalePaymentItem(modelManager, packInfo.GetTable<SalePaymentItem>());
                }
                modelManager.SaveChanges();
            }
        }

        private static void ImportSaleItem(DataModelManager ent, IEnumerable<SaleItem> items)
        {
            foreach (var item in items)
            {
                item.Stamp = DateTime.Now;
                ent.SaleItems.AddObject(item);
                
            }
        }
        
        private static void ImportSaleHeader(DataModelManager ent, IEnumerable<SaleHeader> items)
        {
            foreach (var item in items)
            {
                item.Stamp = DateTime.Now;
                ent.SaleHeaders.AddObject(item);
            }
        }

        private static void ImportSalePaymentItem(DataModelManager ent, IEnumerable<SalePaymentItem> items)
        {
            foreach (var item in items)
            {
                item.Stamp = DateTime.Now;
                ent.SalePaymentItems.AddObject(item);

            }
        }

        private static void ImportSalePaymentHeader(DataModelManager ent, IEnumerable<SalePaymentHeader> items)
        {
            foreach (var item in items)
            {
                item.Stamp = DateTime.Now;
                ent.SalePaymentHeaders.AddObject(item);
            }
        }
    }
}
