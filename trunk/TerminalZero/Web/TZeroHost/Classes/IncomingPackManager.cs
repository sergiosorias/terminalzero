using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.PackClasses;
using ZeroConfiguration.Entities;
using ThreadState = System.Threading.ThreadState;

namespace TZeroHost.Classes
{
    public class IncomingPackManager
    {
        private class IncomingPack
        {
            public string ConnId { get; set; }
            public bool IsFromDb { get; set; }
            public string PackPath { get; set; }
        }
        private Thread _importProcessThread;
        private readonly Queue<IncomingPack> _packsToImport;

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
            _importProcessThread = new Thread(ImportProcessEntryPoint);
            _importProcessThread.Name = "IncomingPackManagerThread";
        }

        private void ImportProcessEntryPoint(object o)
        {
            while (_packsToImport.Count > 0)
            {
                Thread.Sleep(1000);
                IncomingPack data = _packsToImport.Dequeue();
                using (var packManager = PackManagerBuilder.GetManager(data.PackPath))
                {
                    if (packManager != null)
                    {
                        Trace.Write(string.Format("Starting import: ConnID = {0}", data.ConnId), "");
                        packManager.ConnectionID = data.ConnId;
                        packManager.Imported += a_Imported;
                        packManager.Error += a_Error;
                        try
                        {
                            packManager.Import(data.PackPath);
                        }
                        catch (Exception ex)
                        {
                            Trace.Write(string.Format("Import EXCEPTION: ConnID = {0}, ERROR = {1}", data.ConnId, ex), "EXCEPTION");
                        }
                        finally
                        {
                            packManager.Imported -= a_Imported;
                            packManager.Error -= a_Error;
                        }

                        Thread.Sleep(500);
                    }
                }

            }
        }

        private void a_Error(object sender, ErrorEventArgs e)
        {
            var pack = sender as PackManager;
            if (pack != null)
                Trace.Write(string.Format("Import ERROR: ConnID = {0}, ERROR = {1}", pack.ConnectionID, e.GetException()), "ERROR");
        }

        private void a_Imported(object sender, PackEventArgs e)
        {
            Trace.Write(string.Format("Import Finished: Status = {3}, ConnID = {0}, DB Pack = {1}, Pack Module = {2}", e.ConnectionID, e.Pack.Code, e.PackInfo != null ? e.PackInfo.ModuleCode : -1, e.Pack.PackStatusCode), "Information");

            if (e.Pack.PackStatusCode == 2 && 
                    (
                    (e.Pack.IsMasterData.HasValue && e.Pack.IsMasterData.Value)
                    || (e.Pack.IsUpgrade.HasValue && e.Pack.IsUpgrade.Value)
                    )
                )
            {
                using (var packEnt = new CommonEntities())
                {
                    using (var ent = new ConfigurationEntities())
                    {
                        foreach (var item in ent.Terminals)
                        {
                            packEnt.PackPendings.AddObject(PackPending.CreatePackPending(e.Pack.Code, item.Code));
                        }
                    }
                    packEnt.SaveChanges();
                }
            }
        }

        private static IncomingPackManager _instance;
        public static IncomingPackManager Instance
        {
            get
            {
                return _instance ?? (_instance = new IncomingPackManager());
            }
        }

        internal void AddPack(string connectionId, string packName)
        {
            _packsToImport.Enqueue(new IncomingPack { ConnId = connectionId, PackPath = packName });

            switch (_importProcessThread.ThreadState)
            {
                case ThreadState.AbortRequested:
                case ThreadState.Aborted:
                case ThreadState.Background:
                case ThreadState.Running:
                case ThreadState.SuspendRequested:
                case ThreadState.Suspended:
                case ThreadState.WaitSleepJoin:
                    break;
                case ThreadState.StopRequested:
                case ThreadState.Stopped:
                    CreateImportThread();
                    _importProcessThread.Start();
                    break;
                case ThreadState.Unstarted:
                    _importProcessThread.Start();
                    break;
            }
        }
    }
}