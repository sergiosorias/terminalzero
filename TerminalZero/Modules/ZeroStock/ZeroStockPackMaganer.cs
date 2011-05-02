using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using ZeroBusiness.Entities.Data;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Pack;

namespace ZeroStock
{
    public class ZeroStockPackMaganer : PackManager
    {
        public ZeroStockPackMaganer(ITerminal termminal)
            : base(termminal)
        {
            
        }

        protected override void ImportProcess(PackProcessingEventArgs args)
        {
 	        base.ImportProcess(args);
            args.Pack.IsMasterData = false;
            args.Pack.IsUpgrade = false;
            ImportEntities(args);
        }

        protected override void ExportProcess(PackProcessingEventArgs args)
        {
            base.ExportProcess(args);
            var packInfo = (ExportEntitiesPackInfo)args.PackInfo;
            packInfo.ExportTables();

        }

        private void ImportEntities(PackProcessingEventArgs e)
        {
            var packInfo = (ExportEntitiesPackInfo)e.PackInfo;
            using (var ent = new DataModelManager())
            {
                if (packInfo.ContainsTable<StockHeader>())
                {
                    ImportStockHeader(ent, packInfo.GetTable<StockHeader>());
                    if (packInfo.ContainsTable<StockHeader>())
                    {
                        ImportStockItem(ent, packInfo.GetTable<StockItem>());
                    }
                }

                if (packInfo.ContainsTable<DeliveryDocumentHeader>())
                {
                    ImportDeliveryDocumentHeader(ent, packInfo.GetTable<DeliveryDocumentHeader>());
                    if (packInfo.ContainsTable<DeliveryDocumentItem>()) 
                        ImportDeliveryDocumentItem(ent, packInfo.GetTable<DeliveryDocumentItem>());
                }
                ent.SaveChanges();
            }
        }

        private static void ImportDeliveryDocumentHeader(DataModelManager ent, IEnumerable<DeliveryDocumentHeader> items)
        {
            foreach (var item in items)
            {
                item.Stamp = DateTime.Now;
                ent.DeliveryDocumentHeaders.AddObject(item);
            }
        }

        private static void ImportDeliveryDocumentItem(DataModelManager ent, IEnumerable<DeliveryDocumentItem> items)
        {
            foreach (var item in items)
            {
                item.Stamp = DateTime.Now;
                ent.DeliveryDocumentItems.AddObject(item);
            }
        }

        private static void ImportStockItem(DataModelManager ent, IEnumerable<StockItem> items)
        {
            foreach (var item in items)
            {
                item.Stamp = DateTime.Now;
                ent.StockItems.AddObject(item);
            }
        }

        private static void ImportStockHeader(DataModelManager ent, IEnumerable<StockHeader> items)
        {
            foreach (var item in items)
            {
                item.Stamp = DateTime.Now;
                ent.StockHeaders.AddObject(item);
            }
        }

        

    }
}
