using System;
using System.Runtime.Serialization;

namespace ZeroLogHandle.Classes
{
    [DataContract]
    public class VirtualLogEntry
    {
        [DataMember]
        public int IndentLevel { get; private set; }
        [DataMember]
        public DateTime Stamp { get; private set; }
        [DataMember]
        public string Message { get; private set; }

        public VirtualLogEntry(string msg)
        {
            Stamp = DateTime.Now;
            Message = msg;
            IndentLevel = System.Diagnostics.Trace.IndentLevel;
        }
    }
}
