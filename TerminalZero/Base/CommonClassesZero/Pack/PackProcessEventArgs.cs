using System;
using System.ComponentModel;


namespace ZeroCommonClasses.Pack
{
    public class PackProcessEventArgs : CancelEventArgs
    {
        public Entities.Pack Pack { get; set; }
        public PackInfoBase PackInfo { get; set; }
        public string ConnectionID { get; set; }

        public PackProcessEventArgs()
        {
            Pack = null;
        }
    }
}
