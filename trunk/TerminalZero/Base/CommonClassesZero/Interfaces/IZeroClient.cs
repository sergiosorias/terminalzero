using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroCommonClasses.GlobalObjects;

namespace ZeroCommonClasses.Interfaces
{
    public interface IZeroClient
    {
        bool Initialized { get; }
        IProgressNotifier Notifier { get; set; }
        List<ZeroModule> ModuleList { get; }
        ZeroMenu MainMenu { get; }
        void Initialize();
    }
}
