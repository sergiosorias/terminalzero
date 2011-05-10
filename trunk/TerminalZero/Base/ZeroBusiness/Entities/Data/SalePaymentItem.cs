using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Interfaces;

namespace ZeroBusiness.Entities.Data
{
    public partial class SalePaymentItem : IExportableEntity
    {
        internal SalePaymentItem()
        {
            Code = 0;
            UpdateStatus(EntityStatus.New);
            Enable = true;
        }

        public SalePaymentItem(SalePaymentHeader header, PaymentInstrument inst, double quantity):this()
        {
            Code = header.SalePaymentItems.Count();
            TerminalCode = header.TerminalCode;
            TerminalToCode = header.TerminalToCode;
            SalePaymentHeaderCode = header.Code;
            PaymentInstrument = inst;
            Quantity = quantity;
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
