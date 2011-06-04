using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroBusiness.Events;
using ZeroCommonClasses;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Interfaces;
using ZeroBusiness.Manager.Data;

namespace ZeroBusiness.Entities.Data
{
    public partial class SalePaymentHeader : IExportableEntity
    {
        public event EventHandler<ItemsCollectionChangeEventArgs<SalePaymentItem>> ItemsCollectionChanged;

        private void InvokeItemsCollectionChanged(ItemsCollectionChangeEventArgs<SalePaymentItem> e)
        {
            EventHandler<ItemsCollectionChangeEventArgs<SalePaymentItem>> handler = ItemsCollectionChanged;
            if (handler != null) handler(this, e);
        }

        internal SalePaymentHeader()
        {
            
        }

        public SalePaymentHeader(int terminalToCode)
        {
            Code = GetNextSalePaymentHeaderCode();
            Ready = false;
            TerminalToCode = terminalToCode;
            TerminalCode = Terminal.Instance.TerminalCode;
            TotalQuantity = 0;
            UpdateStatus(EntityStatus.New);
        }

        private static int GetNextSalePaymentHeaderCode()
        {
            return BusinessContext.Instance.ModelManager.SalePaymentHeaders.Count();
        }

        #region Generated Properties
        
        public double Change
        {
            get
            {
                var res = TotalQuantity- (SaleHeaders.Count>0? SaleHeaders.Select(sh=>sh.PriceSumValue).Sum():0);
                return Math.Round(res > 0 ? res : 0,2);
            }
        }

        public double RestToPay
        {
            get
            {
                return
                    Math.Round(
                        SaleHeaders.Count > 0 ? SaleHeaders.Select(sh => sh.PriceSumValue).Sum() - TotalQuantity : 0, 2);
            }
        }

        public bool Ready { get; set; }
        #endregion

        public void AddPaymentInstrument(SalePaymentItem payment)
        {
            SalePaymentItems.Add(payment);
            TotalQuantity = SalePaymentItems.Select(pi => pi.Quantity).Sum();
            UpdateViewProperties();
            InvokeItemsCollectionChanged(new ItemsCollectionChangeEventArgs<SalePaymentItem>(payment));
        }

        public void RemovePaymentInstrument(SalePaymentItem payment)
        {
            SalePaymentItems.Remove(payment);
            TotalQuantity = SalePaymentItems.Select(pi => pi.Quantity).Sum();
            UpdateViewProperties();
            InvokeItemsCollectionChanged(new ItemsCollectionChangeEventArgs<SalePaymentItem>(payment));
        }

        private void UpdateViewProperties()
        {
            Ready = !(RestToPay > 0);
            OnPropertyChanged("Change");
            OnPropertyChanged("RestToPay");
            OnPropertyChanged("Ready");
        }

        #region IExportableEntity Members

        public int TerminalDestination
        {
            get { return TerminalToCode; }
        }

        public void UpdateStatus(EntityStatus status)
        {
            Status = (short)status;
            Stamp = DateTime.Now;
        }

        #endregion
    }
}
