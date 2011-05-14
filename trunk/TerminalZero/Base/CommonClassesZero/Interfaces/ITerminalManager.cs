using System;
using System.Collections.Generic;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.GlobalObjects.Actions;

namespace ZeroCommonClasses.Interfaces
{
    public interface ITerminalManager
    {
        event EventHandler ConfigurationRequired;
        void InitializeTerminal();
        ModuleStatus GetModuleStatus(ZeroModule module);
        List<ZeroAction> GetShorcutActions();
        List<ZeroAction> BuilSessionActions();
        bool IsRuleValid(string ruleName);
        bool ExecuteAction(ZeroAction action);
        bool ExistsAction(string actionName, out ZeroAction action);
    }
}
