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
        List<ZeroAction> GetShorcutActions();
        List<ZeroAction> BuilSessionActions();
        bool ExecuteAction(ZeroAction Action);
        bool ValidateRule(string ruleName);
        bool ValidateRule(string ruleName, ref string result);
        bool CanExecute(ZeroAction Action, out string result);
        bool ExistsAction(string actionName, out ZeroAction action);
    }
}
