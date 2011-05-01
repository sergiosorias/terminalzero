using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;
using ZeroBusiness.Entities.Configuration;

namespace TZeroHost.Classes
{
    [DataContract]
    public class TerminalStatus 
    {
        [DataMember]
        public Terminal Terminal { get; set; }
        [DataMember]
        public string Info { get; set; }
    }
}