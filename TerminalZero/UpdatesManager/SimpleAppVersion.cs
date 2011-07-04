using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace UpdatesManager
{
    public class SimpleAppVersion : AppVersion
    {
        public override string RootDir { get; protected set; }

        public override string Version { get; protected set; }

        public SimpleAppVersion(string directory)
        {
            RootDir = directory;
            Version = directory.Substring(directory.LastIndexOf(Path.DirectorySeparatorChar) + 1);
        }

        public override void RunUpdate()
        {
            AppVersionUpdateProgress progress = new AppVersionUpdateProgress
            {
                CanRetry = true
            };
            int count = 0;
            while (count < KRetries && progress.CanRetry)
            {
                progress.HasError = false;
                progress.Message = string.Empty;
                try
                {
                    BeginCopy(RootDir, Environment.CurrentDirectory);
                    Directory.Delete(RootDir, true);
                    progress.Message = string.Format("Actualización a version {0} completa", Version);
                    InvokeProgressChange(progress);
                    break;
                }
                catch (Exception ex)
                {
                    progress.Message = ex.Message;
                    progress.HasError = true;
                    InvokeProgressChange(progress);
                    Trace.TraceError(ex.ToString());
                    count++;
                    Thread.Sleep(500);
                }
            }

            if ( count == KRetries || (!progress.CanRetry && progress.HasError))
            {
                Trace.TraceError(string.Format("Luego de {0} intentos, no ha sido posible actualizar la app con la version {1}", KRetries, Version));
                progress.Message = string.Format("Luego de {0} intentos, no ha sido posible actualizar la aplicación con la version {1}", KRetries, Version);
                InvokeProgressChange(progress);
            }
        }

        private void BeginCopy(string root, string destination)
        {
            string dir;
            foreach (string directory in Directory.GetDirectories(root))
            {
                dir = Path.Combine(destination, directory.Substring(directory.LastIndexOf('\\') + 1));

                if (!Directory.Exists(dir))
                    Directory.CreateDirectory(dir);

                BeginCopy(directory, dir);
            }

            foreach (string file in Directory.GetFiles(root))
            {
                InvokeProgressChange(new AppVersionUpdateProgress { Message = string.Format("Actualizando {0}", Path.GetFileName(file)), HasError = false });
                StartingFileCopy(file, destination);
                File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), true);
            }

        }

        protected virtual void StartingFileCopy(string newFilePath, string destinationDir)
        {
            
        }
        
        public override event EventHandler<AppVersionUpdateProgress> ProgressChanged;
        
        protected void InvokeProgressChange(AppVersionUpdateProgress e)
        {
            EventHandler<AppVersionUpdateProgress> handler = ProgressChanged;
            if (handler != null) handler(this, e);
        }
    }
}
