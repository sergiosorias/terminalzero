using System.Collections.Generic;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.Interfaces;
using ZeroUpdateManager.Properties;

namespace ZeroUpdateManager
{
    public class ZeroUpdateManagerModule : ZeroCommonClasses.ZeroModule
    {
        public ZeroUpdateManagerModule()
            : base(5, "Maneja las actualizaciones del sistema")
        {
            BuildPosibleActions();
        }

        public void BuildPosibleActions()
        {
            ZeroCommonClasses.Terminal.Instance.Session.Actions.Add(new ZeroBackgroundAction( ZeroBusiness.Actions.ExecUpgradeProcess, ImportScriptFile, null,false,true));
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
            var PackReceived = new UpdateManagerPackManager(ZeroCommonClasses.Terminal.Instance);
            PackReceived.Imported += (o, e) => { try { System.IO.File.Delete(path); } catch { ZeroCommonClasses.Terminal.Instance.CurrentClient.Notifier.Log(System.Diagnostics.TraceLevel.Verbose, string.Format("Error deleting pack imported. Module = {0}, Path = {1}", ModuleCode, path)); } };
            PackReceived.Error += PackReceived_Error;
            if (PackReceived.Import(path))
            {
                ZeroCommonClasses.Terminal.Instance.CurrentClient.Notifier.SendNotification(Resources.SuccessfullyUpgrade);
            }
            else
            {
                ZeroCommonClasses.Terminal.Instance.CurrentClient.Notifier.SendNotification(Resources.UnsuccessfullyUpgrade);
            }
        }

        private void PackReceived_Error(object sender, System.IO.ErrorEventArgs e)
        {
            ZeroCommonClasses.Terminal.Instance.CurrentClient.Notifier.Log(System.Diagnostics.TraceLevel.Error, e.GetException().ToString());
        }

    }
}
