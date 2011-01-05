using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

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
            ZeroLogHandle.VirtualTraceListener VL = System.Diagnostics.Trace.Listeners.OfType<ZeroLogHandle.VirtualTraceListener>().FirstOrDefault();
            if (VL != null)
            {
                res = VL.GetLogs();
            }

            return res;
        }
    }
}
