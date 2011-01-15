using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.GlobalObjects;
using System.ComponentModel;


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
            Terminal.Session.AddAction(new ZeroAction(ActionType.MenuItem, "Configuración@Actualizaciones@Base de datos", ImportScriptFile));
        }

        public override string[] GetFilesToSend()
        {
            return new string[] { };
        }

        public override void Init()
        {
            
        }

        private void ImportScriptFile(ZeroRule rule)
        {
            List<string> filesToProcess = new List<string>();
            filesToProcess.AddRange(System.IO.Directory.GetFiles(WorkingDirectoryIn, "*.zip"));
            filesToProcess.AddRange(System.IO.Directory.GetFiles(WorkingDirectoryIn, "*.zpack"));            
            foreach (var item in filesToProcess)
            {
                NewPackReceived(item);
            }
        }

        public override void NewPackReceived(string path)
        {
            base.NewPackReceived(path);
            UpdateManagerPackManager PackReceived = new UpdateManagerPackManager(path);
            PackReceived.Imported += (o, e) => { try { System.IO.File.Delete(path); } catch { Terminal.Session.Notifier.Log(System.Diagnostics.TraceLevel.Verbose, string.Format("Error deleting pack imported. Module = {0}, Path = {1}", ModuleCode, path)); } };
            PackReceived.Error += new System.IO.ErrorEventHandler(PackReceived_Error);
            if (PackReceived.Process())
            {
                Terminal.Session.Notifier.SendNotification("Actualización completada con éxito!");
            }
            else
            {
                Terminal.Session.Notifier.SendNotification("Ocurrio un error durante el proceso de actualización!");
            }
        }

        private void PackReceived_Error(object sender, System.IO.ErrorEventArgs e)
        {
            Terminal.Session.Notifier.Log(System.Diagnostics.TraceLevel.Error, e.GetException().ToString());
        }

    }
}
