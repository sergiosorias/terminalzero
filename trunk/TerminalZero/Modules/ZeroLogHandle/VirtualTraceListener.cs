using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroCommonClasses.Interfaces;
using System.ComponentModel;
using System.Timers;
using ZeroLogHandle.Classes;

namespace ZeroLogHandle
{
    public class VirtualTraceListener : System.Diagnostics.TraceListener
    {
        private int LogEntryTimeOut = 600;
        private int LogEntryMaxCount = 500;

        Timer timer;
        public VirtualTraceListener()
            : base()
        {
            timer = new Timer(1000 * LogEntryTimeOut);
            timer.Elapsed += new ElapsedEventHandler(Clean);
            timer.Start();
        }

        private List<VirtualLogEntry> Logs = new List<VirtualLogEntry>();
        private object oSync = new object();

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
            DateTime MaxStamp = DateTime.Now.AddSeconds(LogEntryTimeOut*-1);
            lock (oSync)
            {
                Logs.RemoveAll(l => l.Stamp < MaxStamp);
            }
            if (Logs.Count > LogEntryMaxCount)
            {
                lock (oSync)
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
