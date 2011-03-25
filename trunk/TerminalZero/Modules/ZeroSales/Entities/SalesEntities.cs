using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroCommonClasses.Interfaces;

namespace ZeroSales.Entities
{
    public class SalesEntities : Entities
    {
        public SalesEntities()
            : base(ZeroCommonClasses.Context.ContextInfo.GetConnectionForCurrentEnvironment("Sales"))
        {
            
        }

        public int GetNextSaleHeaderCode(ITerminal terminal)
        {
            return SaleHeaders.Where(hh=>hh.TerminalCode == terminal.TerminalCode).Max(h => h.Code) + 1;
        }
    }
}
