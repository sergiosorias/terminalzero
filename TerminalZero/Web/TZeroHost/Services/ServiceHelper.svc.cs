using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using TZeroHost.Classes;
using ZeroBusiness.Entities.Configuration;
using ZeroCommonClasses.GlobalObjects;
using ZeroConfiguration;
using ZeroCommonClasses.Entities;

namespace TZeroHost.Services
{
    [ServiceContract]
    public class ServiceHelper
    {
        [OperationContract]
        public List<string> GetLogsStr(DateTime lastStamp)
        {
            List<string> res = new List<string>();
            List<ZeroLogHandle.Classes.VirtualLogEntry> algo = GetLogs(lastStamp);
            if (algo != null)
                res.AddRange(algo.Select(l => string.Format("{1}{0} {2}", l.Stamp.ToString("yyyy/MM/dd hh:mm:ss"), ("".PadLeft(l.IndentLevel, '\t')), l.Message)));


            return res;
        }

        [OperationContract]
        public List<ZeroLogHandle.Classes.VirtualLogEntry> GetLogs(DateTime lastStamp)
        {
            List<ZeroLogHandle.Classes.VirtualLogEntry> res = null;
            ZeroLogHandle.VirtualTraceListener vl = System.Diagnostics.Trace.Listeners.OfType<ZeroLogHandle.VirtualTraceListener>().FirstOrDefault();
            if (vl != null)
            {
                res = vl.GetLogs();
            }

            return res;
        }

        [OperationContract]
        public List<Pack> GetPack(DateTime startDate, DateTime endDate)
        {
            List<Pack> ret = new List<Pack>();

            using (ZeroCommonClasses.Entities.CommonEntitiesManager ent = new ZeroCommonClasses.Entities.CommonEntitiesManager())
            {
                ent.ContextOptions.LazyLoadingEnabled = false;
                ret.AddRange(ent.Packs.Where(p => p.Stamp >= startDate && p.Stamp <= endDate).ToList());
            }

            return ret;
        }

        [OperationContract]
        public ZeroResponse<List<TerminalStatus>> GetTerminalsStatus()
        {
            ZeroResponse<List<TerminalStatus>> ret = new ZeroResponse<List<TerminalStatus>>();
            try
            {
                ZeroServerConfiguration conf = new ZeroServerConfiguration();
                
                using (var ent = new ConfigurationModelManager())
                {
                    ent.ContextOptions.LazyLoadingEnabled = false;
                    ret.Result = new List<TerminalStatus>();
                    foreach (var terminal in ent.Terminals)
                    {
                        TerminalStatus tst = new TerminalStatus();
                        tst.Terminal = terminal;
                        tst.Info += string.Format("IP: {0}\n", terminal.LastKnownIP);
                        var tt = conf.GetPacksToSend(tst.Terminal.Code);
                        if (tt.Count > 0)
                        {
                            tst.Info += string.Format("Packs pendientes: {0}\n", tt.Count);
                            foreach (var i in tt)
                            {
                                tst.Info += string.Format("Módulo: {0}(Pack: {1})\n", i.Value, i.Key);
                            }
                        }

                        ret.Result.Add(tst);
                    }

                    foreach (var item in ret.Result)
                    {
                        TerminalProperty prop =
                            ent.TerminalProperties.FirstOrDefault(
                                tp => tp.TerminalCode == item.Terminal.Code && tp.Code == "SYNC_EVERY");
                        if (prop != null)
                        {
                            if (item.Terminal.LastSync.HasValue)
                            {
                                item.Terminal.IsSyncronized = item.Terminal.LastSync.Value.AddMinutes(int.Parse(prop.Value)) >
                                                     DateTime.Now;
                            }
                        }
                        
                    }
                    
                }

                ret.IsValid = true;
            }
            catch (Exception ex)
            {
                ret.IsValid = false;
                ret.Message = ex.ToString();
            }

            return ret;
        }
        
    }
}
