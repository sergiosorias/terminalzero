namespace TerminalZeroRiaWebClient.Web.Helpers
{
    public static class AppDirectories
    {
        static AppDirectories()
        {
            Init();
        }

        private static void Init()
        {
            UploadFolder = System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "Upload");
            if (!System.IO.Directory.Exists(UploadFolder)) System.IO.Directory.CreateDirectory(UploadFolder);

            DownloadFolder = System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.ApplicationPhysicalPath, "Downloads");
            if (!System.IO.Directory.Exists(DownloadFolder)) System.IO.Directory.CreateDirectory(DownloadFolder);
        }

        public static string UploadFolder { get; private set; }
        public static string DownloadFolder { get; private set; }
    }
}