using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Interfaces;

namespace ZeroBusiness.Entities.Data
{
    public partial class SaleItem : IExportableEntity
    {
        internal SaleItem()
        {
            
        }

        #region Implementation of IExportableEntity

        public int TerminalDestination
        {
            get { return TerminalToCode; }
        }

        public void UpdateStatus(EntityStatus status)
        {
            Stamp = DateTime.Now;
            Status = (short)status;
        }

        #endregion
    }
}
