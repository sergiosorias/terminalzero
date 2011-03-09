using System;
using System.Linq;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Pack;
using ZeroSales.Entities;

namespace ZeroSales
{
    public class ZeroSalesPackManager : PackManager
    {
        public ZeroSalesPackManager(ITerminal terminal)
            :base(terminal)
        {
            Importing += ZeroSalesPackManager_Importing;
            Exporting += ZeroSalesPackManager_Exporting;
        }

        void ZeroSalesPackManager_Exporting(object sender, PackEventArgs e)
        {
            foreach (var item in ((ExportEntitiesPackInfo)e.PackInfo).Tables)
            {
                item.SerializeRows(e.WorkingDirectory);
            }
        }

        void ZeroSalesPackManager_Importing(object sender, PackEventArgs e)
        {
            e.Pack.IsMasterData = false;
            e.Pack.IsUpgrade = false;
            ImportEntities(e);
        }

        private void ImportEntities(PackEventArgs e)
        {
            var packInfo = (ExportEntitiesPackInfo)e.PackInfo;

            using (var ent = new SalesEntities())
            {
                var a = packInfo.Tables.FirstOrDefault(T => T.RowTypeName == typeof(SaleHeader).ToString());
                if (a != null)
                {
                    ImportSaleHeader(e.WorkingDirectory, ent, a);
                    a = packInfo.Tables.FirstOrDefault(T => T.RowTypeName == typeof(SaleItem).ToString());

                    if (a != null) ImportSaleItem(e.WorkingDirectory, ent, a);
                }
                ent.SaveChanges();
            }
        }

        private static void ImportSaleItem(string p, SalesEntities ent, PackTableInfo a)
        {
            foreach (var item in a.DeserializeRows<SaleItem>(p))
            {
                item.Stamp = DateTime.Now;
                ent.SaleItems.AddObject(item);
                
            }
        }

        

        private static void ImportSaleHeader(string p, SalesEntities ent, PackTableInfo a)
        {
            foreach (var item in a.DeserializeRows<SaleHeader>(p))
            {
                item.Stamp = DateTime.Now;
                ent.SaleHeaders.AddObject(item);
            }
        }
    }
}
