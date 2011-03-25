
using ZeroCommonClasses.Context;

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
            : base(ContextInfo.GetConnectionForCurrentEnvironment("DBCommonTables"))
        {
            
        }
    }
}
