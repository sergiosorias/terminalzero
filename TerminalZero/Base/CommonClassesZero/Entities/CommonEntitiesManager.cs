
using ZeroCommonClasses.Context;

namespace ZeroCommonClasses.Entities
{
    public class CommonEntitiesManager : Entities
    {
        public CommonEntitiesManager()
            : base(ConfigurationContext.GetConnectionForCurrentEnvironment("CommonModel"))
        {
            
        }
    }
}
