using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
