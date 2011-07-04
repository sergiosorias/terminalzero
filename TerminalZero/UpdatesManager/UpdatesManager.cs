using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace UpdatesManager
{
    public static class UpdatesManager
    {
        public static bool ExistsNewVersions
        {
            get { return Directory.GetDirectories(Path.Combine(Environment.CurrentDirectory, "Upgrade")).Length > 0; }
        }

        public static IEnumerable<AppVersion> GetNewerVersions()
        {
            var versions = new List<AppVersion>();
            AppVersion version;
            foreach (var item in Directory.GetDirectories(Path.Combine(Environment.CurrentDirectory, "Upgrade")))
            {
                version = new SimpleAppVersion(item);

                if(((IList) Environment.GetCommandLineArgs()).Contains("-c"))
                    version = new AppVersionCleaner(version);

                versions.Add(version);
            }
            return versions;
        }

        public static void RunUpdateProcess(Action<string> messageCallback, Action<bool> finishStatus)
        {
            try
            {
                if (messageCallback != null)
                {
                    messageCallback("Running update Process..");
                    messageCallback("Por favor espere!");
                }
                Thread.Sleep(250);
                IEnumerable<AppVersion> versions = GetNewerVersions();
                foreach (AppVersion appVersion in versions)
                {
                    if (messageCallback != null) messageCallback(string.Format("Update {0}", appVersion.Version));
                    Thread.Sleep(250);
                    appVersion.RunUpdate();
                }
                if (messageCallback != null) messageCallback("La aplicación se actualizo correctamente!");
                if (finishStatus != null) finishStatus(true);
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
                if (messageCallback != null)
                    messageCallback("Ocurrio un error durante el proceso de actualización, se intentara cargar normalmente!");

                if (finishStatus != null) finishStatus(false);
            }
            
        }
    }
}