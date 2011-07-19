using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace ZeroCommonClasses.Pack
{
    [DataContract]
    public class PackInfoBase
    {
        public PackInfoBase()
        {
            TerminalToCodes = new List<int>();
        }

        [DataMember]
        public int TerminalCode { get; set; }

        [DataMember]
        public int ModuleCode { get; set; }

        [DataMember]
        public List<int> TerminalToCodes { get; set; }

        [XmlIgnore]
        public string WorkingDirectory { get; set; }

        [DataMember]
        public DateTime Stamp { get; set; }

        [OnSerializing]
        protected void Token()
        {
            Stamp = DateTime.Now;
        }

    }
}
