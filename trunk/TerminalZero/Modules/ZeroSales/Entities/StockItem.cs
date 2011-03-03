using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Interfaces;

namespace ZeroSales.Entities
{
    public partial class SaleItem:  IExportableEntity
    {
        //public double NetPriceValue
        //{
        //    get { return PriceValue - TaxValue - Tax1Value; }
        //}

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
