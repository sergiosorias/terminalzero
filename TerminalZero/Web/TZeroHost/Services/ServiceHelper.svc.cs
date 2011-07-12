using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel;
using TZeroHost.Classes;
using ZeroBusiness.Entities.Configuration;
using ZeroCommonClasses.Entities;
using ZeroCommonClasses.GlobalObjects;
using ZeroConfiguration;
using ZeroLogHandle;
using ZeroLogHandle.Classes;

namespace TZeroHost.Services
{
    [ServiceContract]
    public class ServiceHelper
    {
        [OperationContract]
        public List<string> GetLogsStr(DateTime lastStamp)
        {
            var res = new List<string>();
            List<VirtualLogEntry> algo = GetLogs(lastStamp);
            if (algo != null)
                res.AddRange(algo.Select(l => string.Format("{1}{0} {2}", l.Stamp.ToString("yyyy/MM/dd hh:mm:ss"), ("".PadLeft(l.IndentLevel, '\t')), l.Message)));


            return res;
        }

        [OperationContract]
        public List<VirtualLogEntry> GetLogs(DateTime lastStamp)
        {
            List<VirtualLogEntry> res = null;
            VirtualTraceListener vl = Trace.Listeners.OfType<VirtualTraceListener>().FirstOrDefault();
            if (vl != null)
            {
                res = vl.GetLogs();
            }

            return res;
        }

        [OperationContract]
        public List<Pack> GetPack(DateTime startDate, DateTime endDate)
        {
            var ret = new List<Pack>();

            using (var ent = new CommonEntitiesManager())
            {
                ent.ContextOptions.LazyLoadingEnabled = false;
                ret.AddRange(ent.Packs.Where(p => p.Stamp >= startDate && p.Stamp <= endDate));
            }
            ret.ForEach(p => p.Data = null);
            return ret;
        }

        [OperationContract]
        public Pack GetPackData(Pack pack)
        {
            Pack pack1;
            using (var ent = new CommonEntitiesManager())
            {
                ent.ContextOptions.LazyLoadingEnabled = false;
                pack1 = ent.Packs.FirstOrDefault(p => p.Code == pack.Code);
            }
            return pack1;
        }

        
        [OperationContract]
        public ZeroResponse<List<TerminalStatus>> GetTerminalsStatus()
        {
            var ret = new ZeroResponse<List<TerminalStatus>>();
            try
            {
                var conf = new ZeroServerConfiguration();
                
                using (var ent = new ConfigurationModelManager())
                {
                    ent.ContextOptions.LazyLoadingEnabled = false;
                    ret.Result = new List<TerminalStatus>();
                    foreach (var terminal in ent.Terminals)
                    {
                        var tst = new TerminalStatus();
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
