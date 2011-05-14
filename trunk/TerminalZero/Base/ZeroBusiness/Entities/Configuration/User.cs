using System;
using ZeroCommonClasses.GlobalObjects.Actions;

namespace ZeroBusiness.Entities.Configuration
{
    public class User
    {
        public static Guid GetCurrentUserCode()
        {
            ActionParameterBase param = ZeroCommonClasses.Terminal.Instance.Session[typeof(User)];
            if (param != null)
            {
                //_header.UserCode = (Guid)((MembershipUser)param.Value).ProviderUserKey;
            }
            return Guid.Empty;
        }
    }
}
