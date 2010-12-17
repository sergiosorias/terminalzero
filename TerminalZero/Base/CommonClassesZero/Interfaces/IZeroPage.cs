using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace ZeroCommonClasses.Interfaces
{
    public enum Mode
    {
        New = 0,
        Update = 1,
        Delete = 2,
        ReadOnly = 3,
    }

    public interface IZeroPage 
    {
        Mode Mode { get; set; }
        bool CanAccept();
        bool CanCancel();
        
    }
}
