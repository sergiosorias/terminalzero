using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Pack;
using ZeroStock.Entities;

namespace ZeroStock
{
    public class ZeroStockPackMaganer : PackManager
    {
        public ZeroStockPackMaganer(ITerminal termminal)
            : base(termminal)
        {
            Importing += ZeroStockPackMaganer_Importing;
            Exporting += ZeroStockPackMaganer_Exporting;
        }

        private void ZeroStockPackMaganer_Importing(object sender, PackEventArgs e)
        {
            e.Pack.IsMasterData = false;
            e.Pack.IsUpgrade = false;
            ImportEntities(e);
        }

        private void ZeroStockPackMaganer_Exporting(object sender, PackEventArgs e)
        {
            foreach (var item in ((ExportEntitiesPackInfo)e.PackInfo).Tables)
            {
                item.SerializeRows(e.WorkingDirectory);
            }
        }

        private void ImportEntities(PackEventArgs e)
        {
            var packInfo = (ExportEntitiesPackInfo)e.PackInfo;
            
            using (var ent = new StockEntities())
            {
                var a = packInfo.Tables.FirstOrDefault(T => T.RowTypeName == typeof(StockHeader).ToString());
                if (a != null)
                {
                    ImportStockHeader(e.WorkingDirectory, ent, a);
                    a = packInfo.Tables.FirstOrDefault(T => T.RowTypeName == typeof(StockItem).ToString());

                    if (a != null) ImportStockItem(e.WorkingDirectory, ent, a);
                }

                a = packInfo.Tables.FirstOrDefault(T => T.RowTypeName == typeof(DeliveryDocumentHeader).ToString());
                if (a != null)
                {
                    ImportDeliveryDocumentHeader(e.WorkingDirectory, ent, a);
                    a = packInfo.Tables.FirstOrDefault(T => T.RowTypeName == typeof(DeliveryDocumentItem).ToString());
                    if (a != null) ImportDeliveryDocumentItem(e.WorkingDirectory, ent, a);
                }
                ent.SaveChanges();
            }
        }

        private static void ImportDeliveryDocumentHeader(string p, StockEntities ent, PackTableInfo a)
        {
            foreach (var item in a.DeserializeRows<DeliveryDocumentHeader>(p))
            {
                item.Stamp = DateTime.Now;
                ent.DeliveryDocumentHeaders.AddObject(item);
            }
        }

        private static void ImportDeliveryDocumentItem(string p, StockEntities ent, PackTableInfo a)
        {
            foreach (var item in a.DeserializeRows<DeliveryDocumentItem>(p))
            {
                item.Stamp = DateTime.Now;
                ent.DeliveryDocumentItems.AddObject(item);
            }
        }

        private static void ImportStockItem(string p, StockEntities ent, PackTableInfo a)
        {
            foreach (var item in a.DeserializeRows<StockItem>(p))
            {
                item.Stamp = DateTime.Now;
                ent.StockItems.AddObject(item);
            }
        }

        private static void ImportStockHeader(string p, StockEntities ent, PackTableInfo a)
        {
            foreach (var item in a.DeserializeRows<StockHeader>(p))
            {
                item.Stamp = DateTime.Now;
                ent.StockHeaders.AddObject(item);
            }
        }

        

    }
}
