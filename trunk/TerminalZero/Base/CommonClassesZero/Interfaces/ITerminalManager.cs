using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroCommonClasses.GlobalObjects;

namespace ZeroCommonClasses.Interfaces
{
    public interface ITerminalManager
    {
        event EventHandler ConfigurationRequired;
        void InitializeTerminal();
        ModuleStatus GetModuleStatus(ZeroModule c);
        bool Navigate(out string result, ZeroAction Action);
        List<ZeroAction> GetShorcutActions();
    }
}
