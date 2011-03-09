using System;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Interfaces;

namespace ZeroStock.Entities
{
    public partial class StockHeader : IExportableEntity
    {
        public int TerminalDestination
        {
            get { return TerminalToCode; }
        }

        public void UpdateStatus(EntityStatus status)
        {
            Stamp = DateTime.Now;
            Status = (short)status;
        }

        public StockItem AddNewStockItem(Product prod, double qty, string lot)
        {
            StockItem item = StockItem.CreateStockItem(
                StockItems.Count,
                TerminalCode,
                Code,
                true,
                (int)EntityStatus.New,
                lot,
                prod.Code,
                prod.MasterCode,
                prod.ByWeight,
                prod.Price1 != null ? prod.Price1.Value : 0,
                prod.ByWeight ? qty : 1,
                TerminalToCode);

            StockItems.Add(item);
            return item;
        }
    }

    public partial class StockItem : IExportableEntity
    {
        public int TerminalDestination
        {
            get { return TerminalToCode; }
        }

        public void UpdateStatus(EntityStatus status)
        {
            Stamp = DateTime.Now;
            Status = (short)status;
        }
    }
}