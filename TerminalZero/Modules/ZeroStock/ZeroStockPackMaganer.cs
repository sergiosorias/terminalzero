using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.PackClasses;
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

        void ZeroStockPackMaganer_Importing(object sender, PackEventArgs e)
        {
            e.Pack.IsMasterData = false;
            Type infoType = typeof(ExportEntitiesPackInfo);
            var reader = new XmlSerializer(infoType);
            using (XmlReader xmlreader = XmlReader.Create(Path.Combine(e.WorkingDirectory, infoType.ToString())))
            {
                e.PackInfo = (ExportEntitiesPackInfo)reader.Deserialize(xmlreader);
                xmlreader.Close();
            }
            
            ImportEntities(e);
        }

        void ZeroStockPackMaganer_Exporting(object sender, PackEventArgs e)
        {
            foreach (var item in ((ExportEntitiesPackInfo)e.PackInfo).Tables)
            {
                using (XmlWriter xmlwriter = XmlWriter.Create(Path.Combine(e.WorkingDirectory, item.RowType.ToString())))
                {
                    item.SerializeRows(xmlwriter);
                    xmlwriter.Close();
                }
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
                    ImportStockItem(e.WorkingDirectory, ent, a);
                }

                a = packInfo.Tables.FirstOrDefault(T => T.RowTypeName == typeof(DeliveryDocumentHeader).ToString());
                if (a != null)
                {
                    ImportDeliveryDocumentHeader(e.WorkingDirectory, ent, a);
                    a = packInfo.Tables.FirstOrDefault(T => T.RowTypeName == typeof(DeliveryDocumentItem).ToString());
                    ImportDeliveryDocumentItem(e.WorkingDirectory, ent, a);
                }
                ent.SaveChanges();
            }
        }

        private void ImportDeliveryDocumentHeader(string p, StockEntities ent, PackTableInfo a)
        {
            foreach (var item in a.DeserializeRows<DeliveryDocumentHeader>(p))
            {
                item.Stamp = DateTime.Now;
                ent.DeliveryDocumentHeaders.AddObject(item);
            }
        }

        private void ImportDeliveryDocumentItem(string p, StockEntities ent, PackTableInfo a)
        {
            foreach (var item in a.DeserializeRows<DeliveryDocumentItem>(p))
            {
                item.Stamp = DateTime.Now;
                ent.DeliveryDocumentItems.AddObject(item);
            }
        }

        private void ImportStockItem(string p, StockEntities ent, PackTableInfo a)
        {
            foreach (var item in a.DeserializeRows<StockItem>(p))
            {
                item.Stamp = DateTime.Now;
                ent.StockItems.AddObject(item);
            }
        }

        private void ImportStockHeader(string p, StockEntities ent, PackTableInfo a)
        {
            foreach (var item in a.DeserializeRows<StockHeader>(p))
            {
                item.Stamp = DateTime.Now;
                ent.StockHeaders.AddObject(item);
            }
        }

        

    }
}
