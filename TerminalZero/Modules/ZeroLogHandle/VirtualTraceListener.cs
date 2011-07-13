using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using ZeroLogHandle.Classes;

namespace ZeroLogHandle
{
    public class VirtualTraceListener : TraceListener
    {
        private const int LogEntryTimeOut = 600;
        private const int LogEntryMaxCount = 500;

        private readonly Timer timer;
        public VirtualTraceListener()
        {
            timer = new Timer(1000 * LogEntryTimeOut);
            timer.Elapsed += Clean;
            timer.Start();
        }

        private List<VirtualLogEntry> Logs = new List<VirtualLogEntry>();
        private readonly object oSync = new object();

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
            lock (oSync)
            {
                Logs.RemoveAll(l => l.Stamp < maxStamp);
            }
            if (Logs.Count > LogEntryMaxCount)
            {
                lock (oSync)
                {
                    Logs.RemoveRange(0, Logs.Count - LogEntryMaxCount);
                }
            }
            
        }

        public override void Write(string message)
        {
            var args = new VirtualLogEntry(message);
            lock (oSync)
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
