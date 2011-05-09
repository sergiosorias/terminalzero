using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroCommonClasses;
using ZeroCommonClasses.Entities;

namespace ZeroBusiness.Entities.Data
{
    public partial class SalePaymentHeader
    {
        public double Change
        {
            get
            {
                var res = TotalQuantity- (SaleHeaders.Count>0? SaleHeaders.Select(sh=>sh.PriceSumValue).Sum():0);
                return res > 0 ? res : 0;
            }
        }

        public double RestToPay
        {
            get { return SaleHeaders.Count>0? SaleHeaders.Select(sh=>sh.PriceSumValue).Sum() - TotalQuantity:0; }
        }

        public bool NotReady { get; set; }

        public SalePaymentHeader()
        {
            NotReady = true;
            Enable = true;
            Status = (int)EntityStatus.New;
            TerminalToCode = TerminalCode = Terminal.Instance.TerminalCode;
            TotalQuantity = 0;
        }

        public void AddPaymentInstrument(SalePaymentItem payment)
        {
            SalePaymentItems.Add(payment);
            TotalQuantity = SalePaymentItems.Select(pi => pi.Quantity).Sum();
            UpdateViewProperties();
        }

        public void RemovePaymentInstrument(SalePaymentItem payment)
        {
            SalePaymentItems.Remove(payment);
            TotalQuantity = SalePaymentItems.Select(pi => pi.Quantity).Sum();
            UpdateViewProperties();
        }

        private void UpdateViewProperties()
        {
            NotReady = RestToPay > 0;
            OnPropertyChanged("Change");
            OnPropertyChanged("RestToPay");
            OnPropertyChanged("NotReady");
        }
    }
}
