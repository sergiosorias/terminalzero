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
        bool ExecuteAction(ZeroAction action);
        bool ValidateRule(string ruleName);
        bool ValidateRule(string ruleName, ref string result);
        bool CanExecute(ZeroAction action, out string result);
        bool ExistsAction(string actionName, out ZeroAction action);
    }
}
