﻿using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using ZeroBusiness;
using ZeroCommonClasses;
using ZeroCommonClasses.Context;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroUpdateManager.Properties;

namespace ZeroUpdateManager
{
    public class ZeroUpdateManagerModule : ZeroModule
    {
        public ZeroUpdateManagerModule()
            : base(5, "Maneja las actualizaciones del sistema")
        {
            BuildPosibleActions();
        }

        public void BuildPosibleActions()
        {
            Terminal.Instance.Session.Actions.Add(new ZeroBackgroundAction(Actions.ExecUpgradeProcess, ImportScriptFile, null, false, true));
            Terminal.Instance.Session.Actions.Add(new ZeroBackgroundAction(Actions.ExecUpgradeAppProcess, ImportApplication, null, true, true));
        }

        public override string[] GetFilesToSend()
        {
            return new string[] { };
        }

        public override void Initialize()
        {
            
        }

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

        private void ImportApplication(object parameter)
        {
            Terminal.Instance.CurrentClient.ShowDialog("Desea cerrar la app para actualizar?", (dialogResult) =>
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
            if (File.Exists(ConfigurationContext.Directories.ApplicationUpdaterPath))
                File.Delete(ConfigurationContext.Directories.ApplicationUpdaterPath);

            var file = File.Create(ConfigurationContext.Directories.ApplicationUpdaterPath);
            file.Write(Resources.UpdatesManager, 0, Resources.UpdatesManager.Length);
            file.Close();
            Process proc = new Process();
            proc.StartInfo = new ProcessStartInfo(ConfigurationContext.Directories.ApplicationUpdaterPath);
            proc.Start();
        }
        
        public override void NewPackReceived(string path)
        {
            base.NewPackReceived(path);
            var PackReceived = new UpdateManagerPackManager(Terminal.Instance);
            PackReceived.Imported += (o, e) => { try { File.Delete(path); } catch { Terminal.Instance.CurrentClient.Notifier.Log(TraceLevel.Verbose, string.Format("Error deleting pack imported. Module = {0}, Path = {1}", ModuleCode, path)); } };
            PackReceived.Error += PackReceived_Error;
            if (PackReceived.Import(path))
            {
                Terminal.Instance.CurrentClient.Notifier.SendNotification(Resources.SuccessfullyUpgrade);
            }
            else
            {
                Terminal.Instance.CurrentClient.Notifier.SendNotification(Resources.UnsuccessfullyUpgrade);
            }
        }

        private void PackReceived_Error(object sender, ErrorEventArgs e)
        {
            Terminal.Instance.CurrentClient.Notifier.Log(TraceLevel.Error, e.GetException().ToString());
        }

    }
}
