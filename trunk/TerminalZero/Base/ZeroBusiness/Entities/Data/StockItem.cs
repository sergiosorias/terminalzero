using System;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Interfaces;

namespace ZeroBusiness.Entities.Data
{
    public partial class StockItem :  IExportableEntity
    {
        internal StockItem()
        {
            
        }

        public StockItem(StockHeader header, Product product, double quantity, string lot)
            : this()
        {
            Code = header.StockItems.Count;
            UpdateStatus(EntityStatus.New);
            Product = product;
            ProductByWeight = product.ByWeight;
            ProductMasterCode = product.MasterCode;
            PriceValue = product.Price1 != null ? product.Price1.Value : 0;
            Quantity = quantity;
            this.Batch = lot;
        }

        #region Implementation of IExportableEntity

        public int TerminalDestination
        {
            get { return TerminalCode; }
        }

        public void UpdateStatus(EntityStatus status)
        {
            Stamp = DateTime.Now;
            Status = (short)status;
        }

        #endregion
    }
}
