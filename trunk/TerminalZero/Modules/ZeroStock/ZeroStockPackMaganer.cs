using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using ZeroCommonClasses.PackClasses;
using System.Xml.Serialization;
using ZeroStock.Entities;

namespace ZeroStock
{
    public class ZeroStockPackMaganer : ZeroCommonClasses.PackClasses.PackManager
    {
        ZeroCommonClasses.PackClasses.ExportEntitiesPackInfo MyInfo;

        public ZeroStockPackMaganer(string packPath)
            : base(packPath)
        {
            Importing += new EventHandler<ZeroCommonClasses.PackClasses.PackEventArgs>(ZeroStockPackMaganer_Importing);
        }

        void ZeroStockPackMaganer_Importing(object sender, ZeroCommonClasses.PackClasses.PackEventArgs e)
        {
            e.Pack.IsMasterData = false;
            Type infoType = typeof(ExportEntitiesPackInfo);
            XmlSerializer reader = new XmlSerializer(infoType);
            using (XmlReader xmlreader = XmlReader.Create(Path.Combine(e.WorkingDirectory, infoType.ToString())))
            {
                MyInfo = (ExportEntitiesPackInfo)reader.Deserialize(xmlreader);
                xmlreader.Close();
            }
            e.PackInfo = MyInfo;

            ImportEntities(e);
        }

        private void ImportEntities(PackEventArgs e)
        {
            ExportEntitiesPackInfo packInfo = (ExportEntitiesPackInfo)e.PackInfo;
            
            using (ZeroStock.Entities.StockEntities ent = new Entities.StockEntities())
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

        public ZeroStockPackMaganer(ZeroCommonClasses.PackClasses.ExportEntitiesPackInfo info)
            : base(info)
        {
            MyInfo = info;
            base.Exporting += new EventHandler<ZeroCommonClasses.PackClasses.PackEventArgs>(ZeroStockPackMaganer_Exporting);
        }

        void ZeroStockPackMaganer_Exporting(object sender, ZeroCommonClasses.PackClasses.PackEventArgs e)
        {
            foreach (var item in MyInfo.Tables)
            {
                using (XmlWriter xmlwriter = XmlWriter.Create(Path.Combine(e.WorkingDirectory, item.RowType.ToString())))
                {
                    item.SerializeRows(xmlwriter);
                    xmlwriter.Close();
                }
            }
        }

    }
}
