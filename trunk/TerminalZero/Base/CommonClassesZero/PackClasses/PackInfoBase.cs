using System;
using System.Runtime.Serialization;

namespace ZeroCommonClasses.PackClasses
{
    [DataContract]
    public abstract class PackInfoBase
    {
        protected PackInfoBase()
        {
            DestinationTerminalCode = -1;
        }

        [DataMember]
        public int ModuleCode { get; set; }

        [DataMember]
        public int DestinationTerminalCode { get; set; }

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
