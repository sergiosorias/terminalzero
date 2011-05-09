using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroCommonClasses.Entities;

namespace ZeroBusiness.Entities.Data
{
    public partial class SalePaymentItem
    {
        public SalePaymentItem()
        {
            
        }
        public SalePaymentItem(SalePaymentHeader header, PaymentInstrument inst, double quantity)
        {
            Code = 0;
            TerminalCode = header.TerminalCode;
            TerminalToCode = header.TerminalToCode;
            SalePaymentHeaderCode = header.Code;
            Enable = true;
            Status = (int)EntityStatus.New;
            PaymentInstrument = inst;
            Quantity = quantity;
        }
    }
}
