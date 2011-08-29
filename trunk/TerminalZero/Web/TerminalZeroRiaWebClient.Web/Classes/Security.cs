using System;
using System.Web.Security;

namespace TerminalZeroRiaWebClient.Web.Classes
{
    public class Security
    {
        public const string DefaultRoleId = "User";
        public const string AdministratorRoleId = "SystemAdministrator";
        public const string CientAdministratorRoleId = "ClientAdministrator";

        public static UserDefaultInfo SystemAdministrator = new UserDefaultInfo { Id = AdministratorRoleId, Name = "Administrator", DefaultPassword = "tzadmin" };
        public static UserDefaultInfo ClientAdministrator = new UserDefaultInfo { Id = CientAdministratorRoleId, Name = "Client Administrator", DefaultPassword = "terminalzero" };
        public static UserDefaultInfo Default = new UserDefaultInfo { Id = DefaultRoleId, Name = "User", DefaultPassword = "invitado" };

        public static bool UserIsAuthenticated
        {
            get
            {
                return GetUser() != null;
            }
        }

        private static DateTime lastUpdate = DateTime.Now;
        private static MembershipUser currentUser;
        private static MembershipUser GetUser()
        {
            if(lastUpdate - DateTime.Now > TimeSpan.FromSeconds(20) || currentUser == null)
            {
                currentUser = Membership.GetUser();
                lastUpdate = DateTime.Now;
            }
            return currentUser;
        }

        public static TerminalZeroUser CurrentUser
        {
            get
            {
                var user = GetUser();
                if(user!=null)
                return new TerminalZeroUser
                             {
                                Id = user.ProviderUserKey,
                                Name = user.UserName,
                             };
                
                return null;
            }
        }

    }

    public class UserDefaultInfo
    {
        public string Id {get;set;}
        public string Name { get; set; }
        public string DefaultPassword { get; set; }
    }

    public class TerminalZeroUser
    {
        public object Id { get; set; }
        public string Name { get; set; }
    }
}