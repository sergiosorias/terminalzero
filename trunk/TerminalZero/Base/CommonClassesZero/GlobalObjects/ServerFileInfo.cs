using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;

namespace ZeroCommonClasses.Files
{
    [MessageContract]
    public class ServerFileInfo
    {
        [MessageBodyMember(Order = 1)]
        public string FileName;
        [MessageBodyMember(Order = 2)]
        public bool IsFromDB;
    }
}
