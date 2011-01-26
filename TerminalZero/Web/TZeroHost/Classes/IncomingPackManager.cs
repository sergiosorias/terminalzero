using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Pack;
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
                        Trace.WriteIf(ZeroCommonClasses.Context.ContextBuilder.LogLevel.TraceInfo, string.Format("Starting import: ConnID = {0}", data.ConnId), "Information");
                        packManager.ConnectionID = data.ConnId;
                        packManager.Imported += a_Imported;
                        packManager.Error += a_Error;
                        try
                        {
                            packManager.Import(data.PackPath);
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteIf(ZeroCommonClasses.Context.ContextBuilder.LogLevel.TraceError, string.Format("Import EXCEPTION: ConnID = {0}, ERROR = {1}", data.ConnId, ex), "EXCEPTION");
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
            Trace.WriteIf(ZeroCommonClasses.Context.ContextBuilder.LogLevel.TraceInfo,
                string.Format("Import Finished: Status = {3}, ConnID = {0}, DB Pack = {1}, Pack Module = {2}",
                              e.ConnectionID, e.Pack.Code, e.PackInfo != null ? e.PackInfo.ModuleCode : -1,
                              (PackManager.PackStatus)e.Pack.PackStatusCode), "Information");

            if (e.Pack.PackStatusCode == (int)PackManager.PackStatus.Imported)
            {

                if ((e.Pack.IsMasterData.GetValueOrDefault()) || (e.Pack.IsUpgrade.GetValueOrDefault()))
                {
                    using (var packEnt = new CommonEntities())
                    {
                        using (var ent = new ConfigurationEntities())
                        {
                            foreach (var item in ent.Terminals)
                            {
                                Trace.WriteIf(ZeroCommonClasses.Context.ContextBuilder.LogLevel.TraceVerbose, string.Format("Saved to Pendings of Terminal {0}", item.Code), "Verbose");
                                PackPending pp = PackPending.CreatePackPending(e.Pack.Code, item.Code);
                                pp.Stamp = DateTime.Now;
                                packEnt.PackPendings.AddObject(pp);
                            }
                        }
                        packEnt.SaveChanges();
                    }
                }
                else
                {
                    if (e.PackInfo.TerminalToCodes.Count > 0)
                    {
                        using (var packEnt = new CommonEntities())
                        {
                            using (var ent = new ConfigurationEntities())
                            {
                                foreach (int terminal in e.PackInfo.TerminalToCodes)
                                {
                                    if (ent.Terminals.FirstOrDefault(t => t.Code == terminal) != null)
                                    {
                                        Trace.WriteIf(ZeroCommonClasses.Context.ContextBuilder.LogLevel.TraceVerbose, string.Format("Saved to Pendings of Terminal {0}", terminal), "Verbose");
                                        PackPending pp = PackPending.CreatePackPending(e.Pack.Code, terminal);
                                        pp.Stamp = DateTime.Now;
                                        packEnt.PackPendings.AddObject(pp);
                                    }

                                }
                            }
                            packEnt.SaveChanges();
                        }
                    }
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