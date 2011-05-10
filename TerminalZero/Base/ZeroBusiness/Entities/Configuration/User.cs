using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroCommonClasses.GlobalObjects;


namespace ZeroBusiness.Entities.Configuration
{
    public class User
    {
        public static Guid GetCurrentUserCode()
        {
            //ZeroActionParameterBase param = global::ZeroCommonClasses.Terminal.Instance.Session.GetParameter<MembershipUser>();
            //if (param != null)
            //{
            //    _header.UserCode = (Guid)((MembershipUser)param.Value).ProviderUserKey;
            //}

            return Guid.Empty;
        }
    }
}
