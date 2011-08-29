using System.Web.Security;
using System.Linq;
using TerminalZeroRiaWebClient.Web.Classes;
using TerminalZeroRiaWebClient.Web.Models;

namespace TerminalZeroRiaWebClient.Web
{
    using System.Security.Authentication;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using System.ServiceModel.DomainServices.Server.ApplicationServices;
    using System.Threading;

    // TODO: Switch to a secure endpoint when deploying the application.
    //       The user's name and password should only be passed using https.
    //       To do this, set the RequiresSecureEndpoint property on EnableClientAccessAttribute to true.
    //   
    //       [EnableClientAccess(RequiresSecureEndpoint = true)]
    //
    //       More information on using https with a Domain Service can be found on MSDN.

    /// <summary>
    /// Domain Service responsible for authenticating users when they log on to the application.
    ///
    /// Most of the functionality is already provided by the AuthenticationBase class.
    /// </summary>
    [EnableClientAccess]
    public class AuthenticationService : AuthenticationBase<User>
    {
        static AuthenticationService()
        {
            if (!Roles.RoleExists(Security.DefaultRoleId))
            {
                Roles.CreateRole(Security.DefaultRoleId);
                Roles.CreateRole(Security.AdministratorRoleId);
                Roles.CreateRole(Security.CientAdministratorRoleId);
            }

            if (Membership.GetUser(Security.SystemAdministrator.Name) == null)
            {
                Membership.CreateUser(Security.SystemAdministrator.Name, Security.SystemAdministrator.DefaultPassword);
            }

            var roles = Roles.GetRolesForUser(Security.SystemAdministrator.Name);
            if (!roles.Contains(Security.AdministratorRoleId))
            {
                Roles.AddUserToRole(Security.SystemAdministrator.Name, Security.AdministratorRoleId);
            }
        }
    }
}
