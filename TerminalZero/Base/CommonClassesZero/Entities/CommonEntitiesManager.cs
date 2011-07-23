
using ZeroCommonClasses.Environment;

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
