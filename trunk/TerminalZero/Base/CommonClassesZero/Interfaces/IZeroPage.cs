
using System;

namespace ZeroCommonClasses.Interfaces
{   
    [Flags]
    public enum ControlMode
    {
        /// <summary>
        /// Default Mode
        /// </summary>
        NotSet = -1,
        New = 0,
        Update = 1,
        Delete = 2,
        ReadOnly = 3,
        Selection = 4,
    }
}
