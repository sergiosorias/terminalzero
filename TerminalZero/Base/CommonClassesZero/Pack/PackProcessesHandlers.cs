using System;
using ZeroCommonClasses.Entities;

namespace ZeroCommonClasses.Pack
{
    public class PackEventArgs : EventArgs
    {
        public Entities.Pack Pack { get; set; }
        public PackInfoBase PackInfo { get; set; }
        public string WorkingDirectory { get; set; }
        public string ConnectionID { get; set; }

        public PackEventArgs()
        {
            Pack = null;
        }
    }
}
