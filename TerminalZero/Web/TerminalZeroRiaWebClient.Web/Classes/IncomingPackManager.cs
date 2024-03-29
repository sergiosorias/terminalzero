﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using ZeroBusiness.Entities.Configuration;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.Pack;
using ThreadState = System.Threading.ThreadState;

namespace TerminalZeroRiaWebClient.Web.Classes
{
    public class IncomingPackEventArgs : EventArgs
    {
        public IncomingPackEventArgs(string fileName, string connID)
        {
            Processed = false;
            Name = fileName;
            ConnID = connID;
        }

        public string Name { get; private set; }
        public string ConnID { get; private set; }
        public bool Processed { get; set; }
    }

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
                IncomingPack data = _packsToImport.Dequeue();
                using (var packManager = PackManagerBuilder.GetManager(data.PackPath))
                {
                    if (packManager != null)
                    {
                        Trace.WriteIf(ZeroCommonClasses.Environment.Config.LogLevel.TraceInfo, string.Format("Starting import: ConnID = {0}", data.ConnId), "Information");
                        packManager.ConnectionID = data.ConnId;
                        packManager.Imported += a_Imported;
                        packManager.Error += PackError;
                        try
                        {
                            packManager.Import(data.PackPath);
                        }
                        catch (Exception ex)
                        {
                            Trace.WriteIf(ZeroCommonClasses.Environment.Config.LogLevel.TraceError, string.Format("Import EXCEPTION: ConnID = {0}, ERROR = {1}", data.ConnId, ex), "EXCEPTION");
                        }
                        finally
                        {
                            packManager.Imported -= a_Imported;
                            packManager.Error -= PackError;
                        }

                        Thread.Sleep(200);
                    }
                }

            }
        }

        private void PackError(object sender, ErrorEventArgs e)
        {
            var pack = sender as PackManager;
            if (pack != null)
                Trace.Write(string.Format("Import ERROR: ConnID = {0}, ERROR = {1}", pack.ConnectionID, e.GetException()), "ERROR");
        }

        private void a_Imported(object sender, PackProcessEventArgs e)
        {
            Trace.WriteIf(ZeroCommonClasses.Environment.Config.LogLevel.TraceInfo,
                string.Format("Import Finished: Status = {3}, ConnID = {0}, DB Pack = {1}, Pack Module = {2}",
                              e.ConnectionID, e.Pack.Code, e.PackInfo != null ? e.PackInfo.ModuleCode : -1,
                              (PackManager.PackStatus)e.Pack.PackStatusCode), "Information");

            if (e.Pack.PackStatusCode == (int)PackManager.PackStatus.Imported)
            {
                if ((e.Pack.IsMasterData.GetValueOrDefault()) || (e.Pack.IsUpgrade.GetValueOrDefault()))
                {
                    using (var packEnt = new CommonEntitiesManager())
                    {
                        using (var ent = new ConfigurationModelManager())
                        {
                            foreach (var item in ent.Terminals.Where(t => t.Code != e.PackInfo.TerminalCode))
                            {
                                Trace.WriteIf(ZeroCommonClasses.Environment.Config.LogLevel.TraceVerbose, string.Format("Saved to Pendings of Terminal {0}", item.Code), "Verbose");
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

                    if (e.PackInfo != null && e.PackInfo.TerminalToCodes.Count > 0)
                    {
                        using (var packEnt = new CommonEntitiesManager())
                        {
                            using (var ent = new ConfigurationModelManager())
                            {
                                foreach (int terminal in e.PackInfo.TerminalToCodes.Where(c => c != e.PackInfo.TerminalCode))
                                {
                                    if (ent.Terminals.FirstOrDefault(t => t.Code == terminal) != null)
                                    {
                                        Trace.WriteIf(ZeroCommonClasses.Environment.Config.LogLevel.TraceVerbose, string.Format("Saved to Pendings of Terminal {0}", terminal), "Verbose");
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