using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using ZeroCommonClasses.GlobalObjects;
using ZeroConfiguration.Entities;
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

            using (CommonEntities ent = new CommonEntities())
            {
                ent.ContextOptions.LazyLoadingEnabled = false;
                ret.AddRange(ent.Packs.Where(p => p.Stamp >= startDate && p.Stamp <= endDate).ToList());
            }

            return ret;
        }

        [OperationContract]
        public ZeroResponse<List<Terminal>> GetTerminalsStatus()
        {
            ZeroResponse<List<Terminal>> ret = new ZeroResponse<List<Terminal>>();
            try
            {
                using (ConfigurationEntities ent = new ConfigurationEntities())
                {
                    ent.ContextOptions.LazyLoadingEnabled = false;
                    ret.Result = new List<Terminal>();
                    ret.Result.AddRange(ent.Terminals);
                    foreach (var item in ret.Result)
                    {
                        TerminalProperty prop = ent.TerminalProperties.FirstOrDefault(tp => tp.TerminalCode == item.Code && tp.Code == "SYNC_EVERY");
                        if (prop != null)
                        {
                            if (item.LastSync.HasValue)
                            {
                                item.IsSyncronized = item.LastSync.Value.AddMinutes(int.Parse(prop.Value)) > DateTime.Now;
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
