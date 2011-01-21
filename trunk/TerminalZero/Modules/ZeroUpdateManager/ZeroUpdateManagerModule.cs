using System.Collections.Generic;
using ZeroCommonClasses.Interfaces;
using ZeroUpdateManager.Properties;

namespace ZeroUpdateManager
{
    public class ZeroUpdateManagerModule : ZeroCommonClasses.ZeroModule
    {
        public ZeroUpdateManagerModule(ITerminal terminal)
            : base(terminal, 5, "Maneja las actualizaciones del sistema")
        {
            BuildPosibleActions();
        }

        public void BuildPosibleActions()
        {
            //Terminal.Session.AddAction(new ZeroAction(null,ActionType.MenuItem, "Configuración@Actualizaciones@Base de datos", ImportScriptFile));
        }

        public override string[] GetFilesToSend()
        {
            return new string[] { };
        }

        public override void Init()
        {
            
        }

        private void ImportScriptFile()
        {
            var filesToProcess = new List<string>();
            filesToProcess.AddRange(System.IO.Directory.GetFiles(WorkingDirectoryIn, "*"+Resources.CompressFileExtention));
            filesToProcess.AddRange(System.IO.Directory.GetFiles(WorkingDirectoryIn, "*"+Resources.ZeroPackFileExtention));
            foreach (var item in filesToProcess)
            {
                NewPackReceived(item);
            }
        }

        public override void NewPackReceived(string path)
        {
            base.NewPackReceived(path);
            var PackReceived = new UpdateManagerPackManager(Terminal);
            PackReceived.Imported += (o, e) => { try { System.IO.File.Delete(path); } catch { Terminal.Session.Notifier.Log(System.Diagnostics.TraceLevel.Verbose, string.Format("Error deleting pack imported. Module = {0}, Path = {1}", ModuleCode, path)); } };
            PackReceived.Error += PackReceived_Error;
            if (PackReceived.Import(path))
            {
                Terminal.Session.Notifier.SendNotification(Resources.SuccessfullyUpgrade);
            }
            else
            {
                Terminal.Session.Notifier.SendNotification(Resources.UnsuccessfullyUpgrade);
            }
        }

        private void PackReceived_Error(object sender, System.IO.ErrorEventArgs e)
        {
            Terminal.Session.Notifier.Log(System.Diagnostics.TraceLevel.Error, e.GetException().ToString());
        }

    }
}
