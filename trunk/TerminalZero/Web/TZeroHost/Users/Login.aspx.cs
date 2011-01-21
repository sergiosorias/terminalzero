using System;
using System.Web;
using System.Web.Security;

namespace TZeroHost.Users
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ValidateAdminUser();
        }

        private void ValidateAdminUser()
        {
            if (Membership.GetUser("Administrator") == null)
            {
                Membership.CreateUser("Administrator", "tzadmin");
            }
        }

        protected void TryLogIn(object sender, EventArgs e)
        {
            
        }

		[System.Web.Services.WebMethod]
		public static void EndSession()
		{
			HttpContext.Current.Session.Abandon();
			System.Web.Security.FormsAuthentication.SignOut();
			System.Web.Security.FormsAuthentication.RedirectToLoginPage();
		}
    }
}
