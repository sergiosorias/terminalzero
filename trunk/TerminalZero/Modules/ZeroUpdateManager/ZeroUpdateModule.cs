using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ZeroBusiness;
using ZeroCommonClasses;
using ZeroCommonClasses.Environment;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroUpdate.Properties;

namespace ZeroUpdate
{
    
    
    public class ZeroUpdateModule : ZeroModule
    {
        public ZeroUpdateModule()
            : base(5, "Maneja las actualizaciones del sistema")
        {
            
        }

        public override string[] GetFilesToSend()
        {
            return new string[] { };
        }

        public override void Initialize()
        {
            
        }

        [ZeroAction(Actions.ExecUpgradeProcess, null, false, true, true)]
        private void ImportScriptFile(object parameter)
        {
            var filesToProcess = new List<string>();
            filesToProcess.AddRange(Directory.GetFiles(WorkingDirectoryIn, "*" + Resources.CompressFileExtention));
            filesToProcess.AddRange(Directory.GetFiles(WorkingDirectoryIn, "*" + Resources.ZeroPackFileExtention));
            foreach (var item in filesToProcess)
            {
                NewPackReceived(item);
            }
        }

        [ZeroAction(Actions.ExecUpgradeAppProcess, null, true, true, true)]
        private void ImportApplication(object parameter)
        {
            Terminal.Instance.Client.ShowDialog("Desea cerrar la aplicación para actualizar?","Atención", (dialogResult) =>
            {
                if(dialogResult)
                {
                    if (Terminal.Instance.Session.Actions[Actions.AppExit].TryExecute())
                    {
                        UpdateApp();
                    }
                }
            });
        }

        private void UpdateApp()
        {
            if (File.Exists(Directories.ApplicationUpdaterPath))
                File.Delete(Directories.ApplicationUpdaterPath);

            var file = File.Create(Directories.ApplicationUpdaterPath);
            file.Write(Resources.UpdatesManager, 0, Resources.UpdatesManager.Length);
            file.Close();
            Process proc = new Process();
            proc.StartInfo = new ProcessStartInfo(Directories.ApplicationUpdaterPath);
            proc.Start();
        }
        
        public override void NewPackReceived(string path)
        {
            base.NewPackReceived(path);
            var packManager = new UpdateManagerPackManager();
            packManager.Imported += (o, e) => { try { File.Delete(path); } catch { Terminal.Instance.Client.Notifier.Log(TraceLevel.Verbose, string.Format("Error deleting pack imported. Module = {0}, Path = {1}", ModuleCode, path)); } };
            packManager.Error += (o, e) => Terminal.Instance.Client.Notifier.Log(TraceLevel.Error, e.GetException().ToString());
            if (packManager.Import(path))
            {
                Terminal.Instance.Client.Notifier.SendNotification(Resources.SuccessfullyUpgrade);
            }
            else
            {
                Terminal.Instance.Client.Notifier.SendNotification(Resources.UnsuccessfullyUpgrade);
            }
        }
    }
}
