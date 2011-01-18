using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using ZeroLogHandle.Classes;

namespace ZeroLogHandle
{
    public class VirtualTraceListener : System.Diagnostics.TraceListener
    {
        private const int LogEntryTimeOut = 600;
        private const int LogEntryMaxCount = 500;

        readonly Timer _timer;
        public VirtualTraceListener()
        {
            _timer = new Timer(1000 * LogEntryTimeOut);
            _timer.Elapsed += new ElapsedEventHandler(Clean);
            _timer.Start();
        }

        private List<VirtualLogEntry> Logs = new List<VirtualLogEntry>();
        private readonly object _oSync = new object();

        public List<VirtualLogEntry> GetLogs()
        {
            return Logs;
        }

        public IEnumerable<VirtualLogEntry> GetLogs(DateTime lastStamp)
        {
            return Logs.Where(l => l.Stamp > lastStamp);
        }

        private void Clean(object sender, ElapsedEventArgs e)
        {
            DateTime maxStamp = DateTime.Now.AddSeconds(LogEntryTimeOut*-1);
            lock (_oSync)
            {
                Logs.RemoveAll(l => l.Stamp < maxStamp);
            }
            if (Logs.Count > LogEntryMaxCount)
            {
                lock (_oSync)
                {
                    Logs.RemoveRange(0, Logs.Count - LogEntryMaxCount);
                }
            }
            
        }

        #region ILogBuilder Members

        #endregion

        public override void Write(string message)
        {
            VirtualLogEntry args = new VirtualLogEntry(message);
            lock (_oSync)
            {
                Logs.Add(args);
            }
        }

        public override void WriteLine(string message)
        {
            Write(message);
        }

        
    }
}
