
using ZeroCommonClasses.Environment;

namespace ZeroCommonClasses.Entities
{
    public class CommonEntitiesManager : Entities
    {
        public CommonEntitiesManager()
            : base(Config.GetConnectionForCurrentEnvironment("CommonModel"))
        {
            
        }
    }
}
