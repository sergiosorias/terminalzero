using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ZeroCommonClasses.Pack
{
    [DataContract]
    public abstract class PackInfoBase
    {
        protected PackInfoBase()
        {
            TerminalToCodes = new List<int>();
        }

        [DataMember]
        public int ModuleCode { get; set; }

        [DataMember]
        public List<int> TerminalToCodes { get; set; }

        [IgnoreDataMember]
        public string Path { get; set; }

        [DataMember]
        public DateTime Stamp { get; set; }

        [OnSerializing]
        protected void Token()
        {
            Stamp = DateTime.Now;
        }

    }
}
