using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.Pack;
using ZeroMasterData.Entities;


namespace ZeroMasterData
{
    public class MasterDataPackManager : PackManager
    {
        public MasterDataPackManager(ITerminal terminal)
            : base(terminal)
        {
            Exporting += MasterDataPackManager_Exporting;
            Importing += MasterDataPackManager_Importing;
        }

        void MasterDataPackManager_Exporting(object sender, PackEventArgs e)
        {
            foreach (PackTableInfo item in ((ExportEntitiesPackInfo)e.PackInfo).Tables)
            {
                using (XmlWriter xmlwriter = XmlWriter.Create(Path.Combine(e.WorkingDirectory, item.RowType.ToString())))
                {
                    item.SerializeRows(xmlwriter);
                    xmlwriter.Close();
                }
            }
        }

        void MasterDataPackManager_Importing(object sender, PackEventArgs e)
        {
            e.Pack.IsMasterData = true;
            Type infoType = typeof(ExportEntitiesPackInfo);
            XmlSerializer reader = new XmlSerializer(infoType);
            
            using (XmlReader xmlreader = XmlReader.Create(Path.Combine(e.WorkingDirectory, infoType.ToString())))
            {
                e.PackInfo =  (ExportEntitiesPackInfo)reader.Deserialize(xmlreader);
                xmlreader.Close();
            }
            

            ImportEntities(e);
        }

        private static void ImportEntities(PackEventArgs e)
        {
            var packInfo = (ExportEntitiesPackInfo)e.PackInfo;
            
            using (var ent = new MasterDataEntities())
            {
                foreach (var item in packInfo.Tables)
                {
                    if (typeof(Price).ToString() == item.RowTypeName)
                        SavePrices(e.WorkingDirectory, ent, item);
                    else if (typeof(Weight).ToString() == item.RowTypeName)
                        SaveWeight(e.WorkingDirectory, ent, item);
                    else if (typeof(PaymentInstrument).ToString() == item.RowTypeName)
                        SavePaymentInstrument(e.WorkingDirectory, ent, item);
                    else if (typeof(ProductGroup).ToString() == item.RowTypeName)
                        SaveProductGroup(e.WorkingDirectory, ent, item);
                    else if (typeof(Tax).ToString() == item.RowTypeName)
                        SaveTax(e.WorkingDirectory, ent, item);
                    else if (typeof(TaxPosition).ToString() == item.RowTypeName)
                        SaveTaxPosition(e.WorkingDirectory, ent, item);
                    else if (typeof(Product).ToString() == item.RowTypeName)
                        SaveProduct(e.WorkingDirectory, ent, item);
                    else if (typeof(Supplier).ToString() == item.RowTypeName)
                        SaveSupplier(e.WorkingDirectory, ent, item);
                    else if (typeof(Customer).ToString() == item.RowTypeName)
                        SaveCustomers(e.WorkingDirectory, ent, item);

                }

                ent.SaveChanges();
            }
        }

        private static void SaveCustomers(string filePath, MasterDataEntities ent, PackTableInfo item)
        {
            List<Customer> items = item.DeserializeRows<Customer>(filePath);
            foreach (var price in items.OrderBy(p => p.Code))
            {
                price.Stamp = DateTime.Now;
                Customer P = ent.Customers.FirstOrDefault(p => p.Code == price.Code && p.TerminalCode == price.TerminalCode);
                if (P != null)
                    ent.Customers.ApplyCurrentValues(price);
                else
                    ent.Customers.AddObject(price);
            }
        }

        private static void SaveSupplier(string filePath, MasterDataEntities ent, PackTableInfo item)
        {
            List<Supplier> items = item.DeserializeRows<Supplier>(filePath);
            foreach (var price in items.OrderBy(p => p.Code))
            {
                price.Stamp = DateTime.Now;
                Supplier P = ent.Suppliers.FirstOrDefault(p => p.Code == price.Code);
                if (P != null)
                    ent.Suppliers.ApplyCurrentValues(price);
                else
                    ent.Suppliers.AddObject(price);
            }
        }

        private static void SaveProduct(string filePath, MasterDataEntities ent, PackTableInfo item)
        {
            List<Product> items = item.DeserializeRows<Product>(filePath);
            foreach (var price in items.OrderBy(p => p.Code))
            {
                price.Stamp = DateTime.Now;
                Product P = ent.Products.FirstOrDefault(p => p.Code == price.Code);
                if (P != null)
                    ent.Products.ApplyCurrentValues(price);
                else
                    ent.Products.AddObject(price);
            }
        }

        private static void SaveTaxPosition(string filePath, MasterDataEntities ent, PackTableInfo item)
        {
            List<TaxPosition> items = item.DeserializeRows<TaxPosition>(filePath);
            foreach (var price in items.OrderBy(p => p.Code))
            {
                price.Stamp = DateTime.Now;
                TaxPosition P = ent.TaxPositions.FirstOrDefault(p => p.Code == price.Code);
                if (P != null)
                    ent.TaxPositions.ApplyCurrentValues(price);
                else
                    ent.TaxPositions.AddObject(price);
            }
        }

        private static void SaveTax(string filePath, MasterDataEntities ent, PackTableInfo item)
        {
            List<Tax> items = item.DeserializeRows<Tax>(filePath);
            foreach (var price in items.OrderBy(p => p.Code))
            {
                price.Stamp = DateTime.Now;
                Tax P = ent.Taxes.FirstOrDefault(p => p.Code == price.Code);
                if (P != null)
                    ent.Taxes.ApplyCurrentValues(price);
                else
                    ent.Taxes.AddObject(price);
            }
        }

        private static void SaveProductGroup(string filePath, MasterDataEntities ent, PackTableInfo item)
        {
            List<ProductGroup> items = item.DeserializeRows<ProductGroup>(filePath);
            foreach (var price in items.OrderBy(p => p.Code))
            {
                price.Stamp = DateTime.Now;
                ProductGroup P = ent.ProductGroups.FirstOrDefault(p => p.Code == price.Code);
                if (P != null)
                    ent.ProductGroups.ApplyCurrentValues(price);
                else
                    ent.ProductGroups.AddObject(price);
            }
        }

        private static void SavePaymentInstrument(string filePath, MasterDataEntities ent, PackTableInfo item)
        {
            List<PaymentInstrument> items = item.DeserializeRows<PaymentInstrument>(filePath);
            foreach (var price in items.OrderBy(p => p.Code))
            {
                price.Stamp = DateTime.Now;
                PaymentInstrument P = ent.PaymentInstruments.FirstOrDefault(p => p.Code == price.Code);
                if (P != null)
                    ent.PaymentInstruments.ApplyCurrentValues(price);
                else
                    ent.PaymentInstruments.AddObject(price);
            }
        }

        private static void SaveWeight(string filePath, MasterDataEntities ent, PackTableInfo item)
        {
            List<Weight> items = item.DeserializeRows<Weight>(filePath);
            foreach (var price in items.OrderBy(p => p.Code))
            {
                price.Stamp = DateTime.Now;
                Weight P = ent.Weights.FirstOrDefault(p => p.Code == price.Code);
                if (P != null)
                    ent.Weights.ApplyCurrentValues(price);
                else
                    ent.Weights.AddObject(price);
            }
        }

        private static void SavePrices(string filePath, MasterDataEntities ent, PackTableInfo item)
        {
            List<Price> items = item.DeserializeRows<Price>(filePath);
            foreach (var price in items.OrderBy(p => p.Code))
            {
                price.Stamp = DateTime.Now;
                Price P = ent.Prices.FirstOrDefault(p => p.Code == price.Code);
                if (P != null)
                    ent.Prices.ApplyCurrentValues(price);
                else
                    ent.Prices.AddObject(price);
            }
        }

        

    }
}
