using System;


namespace ZeroCommonClasses.Pack
{
    public class PackProcessingEventArgs : EventArgs
    {
        public ZeroCommonClasses.Entities.Pack Pack { get; set; }
        public PackInfoBase PackInfo { get; set; }
        public string ConnectionID { get; set; }

        public PackProcessingEventArgs()
        {
            Pack = null;
        }
    }
}
