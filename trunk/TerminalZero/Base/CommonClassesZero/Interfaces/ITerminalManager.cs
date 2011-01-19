using System;
using System.Collections.Generic;
using ZeroCommonClasses.GlobalObjects;

namespace ZeroCommonClasses.Interfaces
{
    public interface ITerminalManager
    {
        event EventHandler ConfigurationRequired;
        void InitializeTerminal();
        ModuleStatus GetModuleStatus(ZeroModule module);
        List<ZeroAction> GetShorcutActions();
        List<ZeroAction> BuilSessionActions();
        bool ValidateRule(string ruleName);
        bool ExecuteAction(ZeroAction action);
        bool ExistsAction(string actionName, out ZeroAction action);
    }
}
