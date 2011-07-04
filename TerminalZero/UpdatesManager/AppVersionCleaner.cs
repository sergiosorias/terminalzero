using System;
using System.Collections.Generic;
using System.IO;

namespace UpdatesManager
{
    public class AppVersionCleaner : AppVersionDecorator
    {
        protected List<string> ExtraFileExtentions { private get; set;}

        public AppVersionCleaner(AppVersion appVersion)
            :base(appVersion)
        {
            ExtraFileExtentions = new List<string>();
            ExtraFileExtentions.Add("*.sync");
            ExtraFileExtentions.Add("*.pdb");
            ExtraFileExtentions.Add("*.vshost.*");
        }

        public override void RunUpdate()
        {
            base.RunUpdate();
            CleanPackages();
        }

        private void CleanPackages()
        {
            InvokeProgressChanged(new AppVersionUpdateProgress {HasError = false,Message="Empezando Limpieza..."});
            BeginClean(Environment.CurrentDirectory);
            InvokeProgressChanged(new AppVersionUpdateProgress { HasError = false, Message = "Finalizando Limpieza.." });
        }

        private void BeginClean(string root)
        {
            foreach (string directory in Directory.GetDirectories(root))
            {
                BeginClean(directory);
            }

            foreach (string extention in ExtraFileExtentions)
            {
                foreach (string file in Directory.GetFiles(root, extention))
                {
                    var args = new AppVersionUpdateProgress {HasError = false, Message = string.Format("Eliminando Archivo {0}",Path.GetFileName(file))};
                    InvokeProgressChanged(args);
                    File.Delete(file);
                }    
            }
        }
        
    }
}