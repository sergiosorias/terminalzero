using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Reflection;
using ZeroCommonClasses.Helpers;
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
                item.SerializeRows(e.WorkingDirectory);
            }
        }

        void MasterDataPackManager_Importing(object sender, PackEventArgs e)
        {
            e.Pack.IsMasterData = true;
            ImportEntities(e);
        }

        private static void ImportEntities(PackEventArgs e)
        {
            var packInfo = (ExportEntitiesPackInfo)e.PackInfo;
            
            using (var ent = new MasterDataEntities())
            {
                ent.MetadataWorkspace.LoadFromAssembly(Assembly.GetExecutingAssembly());
                foreach (var item in packInfo.Tables)
                {
                    if (typeof(Price).ToString() == item.RowTypeName)
                        ContextExtentions.MergeEntities(ent, item.DeserializeRows<Price>(e.WorkingDirectory));
                    else if (typeof(Weight).ToString() == item.RowTypeName)
                        ContextExtentions.MergeEntities(ent, item.DeserializeRows<Weight>(e.WorkingDirectory));
                    else if (typeof(PaymentInstrument).ToString() == item.RowTypeName)
                        ContextExtentions.MergeEntities(ent, item.DeserializeRows<PaymentInstrument>(e.WorkingDirectory));
                    else if (typeof(ProductGroup).ToString() == item.RowTypeName)
                        ContextExtentions.MergeEntities(ent, item.DeserializeRows<ProductGroup>(e.WorkingDirectory));
                    else if (typeof(Tax).ToString() == item.RowTypeName)
                        ContextExtentions.MergeEntities(ent, item.DeserializeRows<Tax>(e.WorkingDirectory));
                    else if (typeof(TaxPosition).ToString() == item.RowTypeName)
                        ContextExtentions.MergeEntities(ent, item.DeserializeRows<TaxPosition>(e.WorkingDirectory));
                    else if (typeof(Product).ToString() == item.RowTypeName)
                        ContextExtentions.MergeEntities(ent, item.DeserializeRows<Product>(e.WorkingDirectory));
                    else if (typeof(Supplier).ToString() == item.RowTypeName)
                        ContextExtentions.MergeEntities(ent, item.DeserializeRows<Supplier>(e.WorkingDirectory));
                    else if (typeof(Customer).ToString() == item.RowTypeName)
                    {
                        ContextExtentions.MergeEntities(ent, item.DeserializeRows<Customer>(e.WorkingDirectory));
                    }
                }

                ent.SaveChanges();
            }
        }
        
        //private static void SaveCustomers(string filePath, MasterDataEntities ent, PackTableInfo item)
        //{
            
        //    IEnumerable<Customer> items = item.DeserializeRows<Customer>(filePath);
        //    foreach (var price in items.OrderBy(p => p.Code))
        //    {
        //        price.Stamp = DateTime.Now;
        //        Customer P = ent.Customers.FirstOrDefault(p => p.Code == price.Code && p.TerminalCode == price.TerminalCode);
        //        if (P != null)
        //            ent.Customers.ApplyCurrentValues(price);
        //        else
        //            ent.Customers.AddObject(price);
        //    }
        //}

        //private static void SaveSupplier(string filePath, MasterDataEntities ent, PackTableInfo item)
        //{
        //    IEnumerable<Supplier> items = item.DeserializeRows<Supplier>(filePath);
        //    foreach (var price in items.OrderBy(p => p.Code))
        //    {
        //        price.Stamp = DateTime.Now;
        //        Supplier P = ent.Suppliers.FirstOrDefault(p => p.Code == price.Code);
        //        if (P != null)
        //            ent.Suppliers.ApplyCurrentValues(price);
        //        else
        //            ent.Suppliers.AddObject(price);
        //    }
        //}

        //private static void SaveProduct(string filePath, MasterDataEntities ent, PackTableInfo item)
        //{
        //    IEnumerable<Product> items = item.DeserializeRows<Product>(filePath);
        //    foreach (var price in items.OrderBy(p => p.Code))
        //    {
        //        price.Stamp = DateTime.Now;
        //        Product P = ent.Products.FirstOrDefault(p => p.Code == price.Code);
        //        if (P != null)
        //            ent.Products.ApplyCurrentValues(price);
        //        else
        //            ent.Products.AddObject(price);
        //    }
        //}

        //private static void SaveTaxPosition(string filePath, MasterDataEntities ent, PackTableInfo item)
        //{
        //    IEnumerable<TaxPosition> items = item.DeserializeRows<TaxPosition>(filePath);
        //    foreach (var price in items.OrderBy(p => p.Code))
        //    {
        //        price.Stamp = DateTime.Now;
        //        TaxPosition P = ent.TaxPositions.FirstOrDefault(p => p.Code == price.Code);
        //        if (P != null)
        //            ent.TaxPositions.ApplyCurrentValues(price);
        //        else
        //            ent.TaxPositions.AddObject(price);
        //    }
        //}

        //private static void SaveTax(string filePath, MasterDataEntities ent, PackTableInfo item)
        //{
        //    IEnumerable<Tax> items = item.DeserializeRows<Tax>(filePath);
        //    foreach (var price in items.OrderBy(p => p.Code))
        //    {
        //        price.Stamp = DateTime.Now;
        //        Tax P = ent.Taxes.FirstOrDefault(p => p.Code == price.Code);
        //        if (P != null)
        //            ent.Taxes.ApplyCurrentValues(price);
        //        else
        //            ent.Taxes.AddObject(price);
        //    }
        //}

        //private static void SaveProductGroup(string filePath, MasterDataEntities ent, PackTableInfo item)
        //{
        //    IEnumerable<ProductGroup> items = item.DeserializeRows<ProductGroup>(filePath);
        //    foreach (var price in items.OrderBy(p => p.Code))
        //    {
        //        price.Stamp = DateTime.Now;
        //        ProductGroup P = ent.ProductGroups.FirstOrDefault(p => p.Code == price.Code);
        //        if (P != null)
        //            ent.ProductGroups.ApplyCurrentValues(price);
        //        else
        //            ent.ProductGroups.AddObject(price);
        //    }
        //}

        //private static void SavePaymentInstrument(string filePath, MasterDataEntities ent, PackTableInfo item)
        //{
        //    IEnumerable<PaymentInstrument> items = item.DeserializeRows<PaymentInstrument>(filePath);
        //    foreach (var price in items.OrderBy(p => p.Code))
        //    {
        //        price.Stamp = DateTime.Now;
        //        PaymentInstrument P = ent.PaymentInstruments.FirstOrDefault(p => p.Code == price.Code);
        //        if (P != null)
        //            ent.PaymentInstruments.ApplyCurrentValues(price);
        //        else
        //            ent.PaymentInstruments.AddObject(price);
        //    }
        //}

        //private static void SaveWeight(string filePath, MasterDataEntities ent, PackTableInfo item)
        //{
        //    IEnumerable<Weight> items = item.DeserializeRows<Weight>(filePath);
        //    foreach (var price in items.OrderBy(p => p.Code))
        //    {
        //        price.Stamp = DateTime.Now;
        //        Weight P = ent.Weights.FirstOrDefault(p => p.Code == price.Code);
        //        if (P != null)
        //            ent.Weights.ApplyCurrentValues(price);
        //        else
        //            ent.Weights.AddObject(price);
        //    }
        //}

        //private static void SavePrices(string filePath, MasterDataEntities ent, PackTableInfo item)
        //{
        //    IEnumerable<Price> items = item.DeserializeRows<Price>(filePath);
        //    foreach (var price in items.OrderBy(p => p.Code))
        //    {
        //        price.Stamp = DateTime.Now;
        //        Price P = ent.Prices.FirstOrDefault(p => p.Code == price.Code);
        //        if (P != null)
        //            ent.Prices.ApplyCurrentValues(price);
        //        else
        //            ent.Prices.AddObject(price);
        //    }
        //}

        

    }
}
