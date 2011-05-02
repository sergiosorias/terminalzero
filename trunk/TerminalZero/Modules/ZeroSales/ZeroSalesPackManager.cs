using System;
using System.Collections.Generic;
using System.Linq;
using ZeroBusiness.Entities.Data;
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
        
        private static void ImportEntities(PackProcessingEventArgs e)
        {
            var packInfo = (ExportEntitiesPackInfo)e.PackInfo;
            using (var ent = new DataModelManager())
            {
                if (packInfo.ContainsTable<SaleHeader>())
                {
                    ImportSaleHeader(ent, packInfo.GetTable<SaleHeader>());
                    if (packInfo.ContainsTable<SaleItem>()) ImportSaleItem(ent, packInfo.GetTable<SaleItem>());
                }
                ent.SaveChanges();
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
    }
}
