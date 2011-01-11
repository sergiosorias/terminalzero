using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TZeroHost
{
    public partial class Login : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

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
