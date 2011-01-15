﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroCommonClasses.Entities;

namespace ZeroCommonClasses.PackClasses
{
    public class PackEventArgs : EventArgs
    {
        public Pack Pack { get; set; }
        public PackInfoBase PackInfo { get; set; }
        public string WorkingDirectory { get; set; }
        public string ConnectionID { get; set; }

        public PackEventArgs()
        {
            Pack = null;
        }
    }
}