using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using TZeroHost.Services;
using TZeroHost.Classes;

namespace TZeroHost
{
    public class Global : System.Web.HttpApplication
    {
        static Global()
        {
            FileTransfer.FileReceived += new EventHandler<Handlers.IncomingPackEventArgs>(FileTransfer_FileReceived);
            System.Diagnostics.Trace.Listeners.Add(new ZeroLogHandle.VirtualTraceListener());
        }
        
        protected void Application_Start(object sender, EventArgs e)
        {
            
        }

        private static void FileTransfer_FileReceived(object sender, Handlers.IncomingPackEventArgs e)
        {
            IncomingPackManager.Instance.AddPack(e.ConnID, e.Name);
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}