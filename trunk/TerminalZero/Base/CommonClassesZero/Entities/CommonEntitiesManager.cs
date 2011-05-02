
using ZeroCommonClasses.Context;

namespace ZeroCommonClasses.Entities
{
    public class CommonEntitiesManager : Entities
    {
        public CommonEntitiesManager()
            : base(ContextInfo.GetConnectionForCurrentEnvironment("CommonModel"))
        {
            
        }
    }
}
