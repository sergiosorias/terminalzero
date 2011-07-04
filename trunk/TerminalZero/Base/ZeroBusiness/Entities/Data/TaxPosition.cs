using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroCommonClasses.Entities;

namespace ZeroBusiness.Entities.Data
{
    public partial class TaxPosition
    {
        public PrintMode ResolvePrintMode()
        {
            if (Code == 0 || Code == 2)
                return PrintMode.UseTax;

            return PrintMode.NoTax;
        }

        #region Implementation of IExportableEntity

        public int TerminalDestination
        {
            get { return 0; }
        }

        public void UpdateStatus(EntityStatus status)
        {
            Stamp = DateTime.Now;
            Status = (short)status;
        }

        #endregion
    }
}
