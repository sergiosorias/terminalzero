using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TZeroHost.Handlers;

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
        private System.Threading.Thread ImportProcessThread = null;
        private Queue<IncomingPack> PacksToImport = null;

        public int PackToProcessCount
        {
            get
            {
                return PacksToImport.Count;
            }
        }

        private IncomingPackManager()
        {
            PacksToImport = new Queue<IncomingPack>();
            CreateImportThread();
        }

        private void CreateImportThread()
        {
            ImportProcessThread = new System.Threading.Thread(ImportProcessEntryPoint);
            ImportProcessThread.Name = "IncomingPackManagerThread";
        }

        private void ImportProcessEntryPoint(object o)
        {
            while (PacksToImport.Count > 0)
            {
                System.Threading.Thread.Sleep(1000);
                IncomingPack data = PacksToImport.Dequeue();
                using (var a = PackManagerBuilder.GetManager(data.PackPath))
                {
                    if (a != null)
                    {
                        System.Diagnostics.Trace.Write(string.Format("Starting import: ConnID = {0}", data.ConnID), "");
                        a.ConnectionID = data.ConnID;
                        a.Imported += a_Imported;
                        a.Error += new System.IO.ErrorEventHandler(a_Error);
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

        void a_Error(object sender, System.IO.ErrorEventArgs e)
        {
            ZeroCommonClasses.PackClasses.PackManager pack = sender as ZeroCommonClasses.PackClasses.PackManager;
            System.Diagnostics.Trace.Write(string.Format("Import ERROR: ConnID = {0}, ERROR = {1}", pack.ConnectionID, e.GetException()),"ERROR");
        }

        void a_Imported(object sender, ZeroCommonClasses.PackClasses.PackEventArgs e)
        {
            System.Diagnostics.Trace.Write(string.Format("Import Finished: Status = {3}, ConnID = {0}, DB Pack = {1}, Pack Module = {2}", e.ConnectionID, e.Pack.Code,e.PackInfo.ModuleCode, e.Pack.PackStatusCode), "Information");

            if (e.Pack.PackStatusCode == 2 && e.Pack.IsMasterData.HasValue && e.Pack.IsMasterData.Value)
            {
                using (ZeroConfiguration.Entities.ConfigurationEntities ent = new ZeroConfiguration.Entities.ConfigurationEntities())
                {
                    foreach (var item in ent.Terminals.Where(t=>t.Code !=0))
                    {
                        item.ConnectionRequired = item.ExistsMasterData = true;
                    }

                    ent.SaveChanges();
                }
            }
        }

        private static IncomingPackManager _Instance = null;
        public static IncomingPackManager Instance
        {
            get
            {
                return _Instance ?? (_Instance = new IncomingPackManager());
            }
        }
        
        internal void AddPack(string ConnectionID, string packName)
        {
            PacksToImport.Enqueue(new IncomingPack {ConnID = ConnectionID, PackPath = packName });
            
            switch (ImportProcessThread.ThreadState)
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
                    ImportProcessThread.Start();
                    break;
                case System.Threading.ThreadState.Unstarted:
                    ImportProcessThread.Start();
                    break;
            }
        }
    }
}