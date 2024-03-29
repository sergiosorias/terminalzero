﻿using System;
using System.Linq;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Interfaces;

namespace ZeroBusiness.Entities.Data
{
    public partial class SalePaymentHeader : IExportableEntity
    {
        public SalePaymentHeader()
        {
            
        }

        public SalePaymentHeader(int terminalToCode)
        {
            Code = GetNextSalePaymentHeaderCode();
            TerminalToCode = terminalToCode;
            TerminalCode = Terminal.Instance.Code;
            TotalQuantity = 0;
            UpdateStatus(EntityStatus.New);
        }

        private static int GetNextSalePaymentHeaderCode()
        {
            return BusinessContext.Instance.Model.SalePaymentHeaders.Count(p=>p.TerminalCode == Terminal.Instance.Code)+1;
        }

        #region Generated Properties
        
        public double Change { set; get; }

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
            RefreshTotalQuantity();
        }

        private void RefreshTotalQuantity()
        {
            TotalQuantity = SalePaymentItems.Select(pi => pi.Quantity).Sum();
            UpdateViewProperties();
        }

        public void RemovePaymentInstrument(SalePaymentItem payment)
        {
            SalePaymentItems.Remove(payment);
            RefreshTotalQuantity();
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
