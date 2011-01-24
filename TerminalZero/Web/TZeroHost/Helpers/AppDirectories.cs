using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TZeroHost.Helpers
{
    public static class AppDirectories
    {
        public static string UploadFolder { get { return _UploadFolder; } }
        private static string _UploadFolder;
        public static string DownloadFolder { get { return _DownloadFolder; } }
        private static string _DownloadFolder;

        public static void Init()
        {
            _UploadFolder = System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "Upload");
            if (!System.IO.Directory.Exists(UploadFolder)) System.IO.Directory.CreateDirectory(UploadFolder);

            _DownloadFolder = System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "Downloads");
            if (!System.IO.Directory.Exists(_DownloadFolder)) System.IO.Directory.CreateDirectory(_DownloadFolder);

        }

        private static string GetDownloadFolder()
        {
            return System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath;
        }
    }
}