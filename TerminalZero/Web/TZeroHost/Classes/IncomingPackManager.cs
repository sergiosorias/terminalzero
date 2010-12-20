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
                var a = PackManagerBuilder.GetManager(data.PackPath);
                if (a != null)
                {
                    a.ConnectionID = data.ConnID;
                    a.Imported += new EventHandler<ZeroCommonClasses.PackClasses.PackEventArgs>(a_Imported);
                    a.Process();
                    System.Threading.Thread.Sleep(2000);
                    System.IO.File.Delete(data.PackPath);
                }
                
            }
        }

        void a_Imported(object sender, ZeroCommonClasses.PackClasses.PackEventArgs e)
        {
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