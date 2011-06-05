using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        internal SalePaymentHeader()
        {
            
        }

        public SalePaymentHeader(int terminalToCode)
        {
            Code = GetNextSalePaymentHeaderCode();
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
                return SaleHeaders.Count > 0 ? SaleHeaders.Select(sh => sh.PriceSumValue).Sum() - TotalQuantity : 0;
            }
        }

        public bool Ready { get { return !(RestToPay > 0); } }
        
        #endregion

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
