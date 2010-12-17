using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeroCommonClasses.Entities
{
    public class CommonEntities : Entities
    {
        public enum PackStatusEnum
        {
            Started = 0,
            InProgress = 1,
            Ended = 2,
            Error = 3
        }

        public CommonEntities()
            : base(ZeroCommonClasses.Context.ContextBuilder.GetConnectionForCurrentEnvironment("DBCommonTables"))
        {
            
        }
    }
}
