﻿using System;
using System.Runtime.Serialization;

namespace ZeroCommonClasses.PackClasses
{
    [DataContract]
    public abstract class PackInfoBase
    {
        public PackInfoBase()
        {
            
        }

        [DataMember]
        public int Flags { get; set; }
        
        [DataMember]
        public int ModuleCode { get; set; }

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
