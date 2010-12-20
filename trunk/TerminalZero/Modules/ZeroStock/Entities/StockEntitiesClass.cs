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

        public enum EntitiesStatus
        {
            New = 0,
            Exported = 1,
            Imported = 2,
        }
    }
}
