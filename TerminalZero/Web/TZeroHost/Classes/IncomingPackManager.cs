using System;
using System.Collections.Generic;
using ZeroCommonClasses.PackClasses;

namespace TZeroHost.Classes
{
    public class IncomingPackManager
    {
        private class IncomingPack
        {
            public string ConnID { get; set; }
            public bool IsFromDB { get; set; }
            public string PackPath { get; set; }
        }
        private System.Threading.Thread _importProcessThread = null;
        private readonly Queue<IncomingPack> _packsToImport = null;

        public int PackToProcessCount
        {
            get
            {
                return _packsToImport.Count;
            }
        }

        private IncomingPackManager()
        {
            _packsToImport = new Queue<IncomingPack>();
            CreateImportThread();
        }

        private void CreateImportThread()
        {
            _importProcessThread = new System.Threading.Thread(ImportProcessEntryPoint);
            _importProcessThread.Name = "IncomingPackManagerThread";
        }

        private void ImportProcessEntryPoint(object o)
        {
            while (_packsToImport.Count > 0)
            {
                System.Threading.Thread.Sleep(1000);
                IncomingPack data = _packsToImport.Dequeue();
                using (var a = PackManagerBuilder.GetManager(data.PackPath))
                {
                    if (a != null)
                    {
                        System.Diagnostics.Trace.Write(string.Format("Starting import: ConnID = {0}", data.ConnID), "");
                        a.ConnectionID = data.ConnID;
                        a.Imported += a_Imported;
                        a.Error += a_Error;
                        try
                        {

                            a.Process();
                        }
                        catch (Exception ex)
                        {
                            System.Diagnostics.Trace.Write(string.Format("Import EXCEPTION: ConnID = {0}, ERROR = {1}", data.ConnID, ex), "EXCEPTION");
                        }
                        finally
                        {
                            a.Imported -= a_Imported;
                            a.Error -= a_Error;
                        }

                        System.Threading.Thread.Sleep(500);
                    }
                }

            }
        }

        private void a_Error(object sender, System.IO.ErrorEventArgs e)
        {
            var pack = sender as PackManager;
            if (pack != null)
                System.Diagnostics.Trace.Write(string.Format("Import ERROR: ConnID = {0}, ERROR = {1}", pack.ConnectionID, e.GetException()), "ERROR");
        }

        private void a_Imported(object sender, PackEventArgs e)
        {
            System.Diagnostics.Trace.Write(string.Format("Import Finished: Status = {3}, ConnID = {0}, DB Pack = {1}, Pack Module = {2}", e.ConnectionID, e.Pack.Code, e.PackInfo != null ? e.PackInfo.ModuleCode : -1, e.Pack.PackStatusCode), "Information");

            if (e.Pack.PackStatusCode == 2 && 
                    (
                    (e.Pack.IsMasterData.HasValue && e.Pack.IsMasterData.Value)
                    || (e.Pack.IsUpgrade.HasValue && e.Pack.IsUpgrade.Value)
                    )
                )
            {
                using (var packEnt = new ZeroCommonClasses.Entities.CommonEntities())
                {
                    using (var ent = new ZeroConfiguration.Entities.ConfigurationEntities())
                    {
                        foreach (var item in ent.Terminals)
                        {
                            packEnt.PackPendings.AddObject(ZeroCommonClasses.Entities.PackPending.CreatePackPending(e.Pack.Code, item.Code));
                        }
                    }
                    packEnt.SaveChanges();
                }
            }
        }

        private static IncomingPackManager _instance = null;
        public static IncomingPackManager Instance
        {
            get
            {
                return _instance ?? (_instance = new IncomingPackManager());
            }
        }

        internal void AddPack(string connectionId, string packName)
        {
            _packsToImport.Enqueue(new IncomingPack { ConnID = connectionId, PackPath = packName });

            switch (_importProcessThread.ThreadState)
            {
                case System.Threading.ThreadState.AbortRequested:
                case System.Threading.ThreadState.Aborted:
                case System.Threading.ThreadState.Background:
                case System.Threading.ThreadState.Running:
                case System.Threading.ThreadState.SuspendRequested:
                case System.Threading.ThreadState.Suspended:
                case System.Threading.ThreadState.WaitSleepJoin:
                    break;
                case System.Threading.ThreadState.StopRequested:
                case System.Threading.ThreadState.Stopped:
                    CreateImportThread();
                    _importProcessThread.Start();
                    break;
                case System.Threading.ThreadState.Unstarted:
                    _importProcessThread.Start();
                    break;
            }
        }
    }
}