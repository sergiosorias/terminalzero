﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.ComponentModel;

namespace ZeroCommonClasses.Interfaces
{
    public enum Mode
    {
        New = 0,
        Update = 1,
        Delete = 2,
        ReadOnly = 3,
        Selection = 4,
    }

    public interface IZeroPage 
    {
        Mode Mode { get; set; }
        bool CanAccept();
        bool CanCancel();
        
    }
}