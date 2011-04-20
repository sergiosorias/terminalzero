using System;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Interfaces;

namespace ZeroStock.Entities
{
    public partial class DeliveryDocumentHeader : IExportableEntity, ISelectable
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

        public DeliveryDocumentItem AddNewDeliveryDocumentItem(Product prod, double qty, string lot)
        {
            DeliveryDocumentItem item = DeliveryDocumentItem.CreateDeliveryDocumentItem
                    (DeliveryDocumentItems.Count,
                    TerminalCode,
                    Code,
                    true,
                    0,
                    lot,
                    prod.Code,
                    prod.MasterCode,
                    prod.ByWeight,
                    prod.Price1 != null ? prod.Price1.Value : 0,
                    prod.ByWeight ? qty : 1, TerminalToCode);

            DeliveryDocumentItems.Add(item);

            return item;
        }

        #region ISelectable Members

        public bool Contains(string data)
        {
            return Supplier.Contains(data) || ZeroCommonClasses.Helpers.ComparisonExtentions.ContainsIgnoreCase(data, Note);
        }

        public bool Contains(DateTime data)
        {
            return Date.Date == data.Date;
        }

        #endregion
    }

    public partial class DeliveryDocumentItem : IExportableEntity
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