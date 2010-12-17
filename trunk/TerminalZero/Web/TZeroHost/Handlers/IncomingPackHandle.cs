using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TZeroHost.Handlers
{
    public class IncomingPackEventArgs : EventArgs
    {
        public IncomingPackEventArgs(string fileName, string connID)
        {
            Processed = false;
            Name = fileName;
            ConnID = connID;
        }

        public string Name { get; private set; }
        public string ConnID { get; private set; }
        public bool Processed { get; set; }
    }

    
}