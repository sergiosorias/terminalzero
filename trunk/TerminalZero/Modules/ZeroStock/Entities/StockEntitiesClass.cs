using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeroStock.Entities
{
    internal class StockEntities : ZeroStock.Entities.Entities
    {
        public StockEntities()
            : base(ZeroCommonClasses.Context.ContextBuilder.GetConnectionForCurrentEnvironment("Stock"))
        {

        }

        internal int GetNextStockHeaderCode()
        {
            return StockHeaders.Max(sh => sh.Code) + 1;
        }
    }
}
