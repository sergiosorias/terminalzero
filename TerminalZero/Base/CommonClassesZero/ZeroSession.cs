using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ZeroCommonClasses.Interfaces;
using System.IO;
using ZeroCommonClasses.GlobalObjects;

namespace ZeroCommonClasses
{
    public class ZeroSession
    {
        public ZeroSession()
        {
            SessionParams = new Dictionary<string, ZeroActionParameterBase>();
            SystemActions = new Dictionary<string, ModuleAction>();
            SystemRules = new Dictionary<string, ModuleRule>();
            ModuleList = new List<ZeroModule>();
            AddNavigationParameter(new ZeroActionParameter<List<ZeroModule>>("ExistingModules",true,ModuleList));
        }
        public IProgressNotifier Notifier { get; set; }

        private struct ModuleAction
        {
            public int Module;
            public ZeroAction Action;
        }
        private struct ModuleRule
        {
            public int Module;
            public ZeroRule Rule;
        }
        public List<ZeroModule> ModuleList { get; private set; }
        private Dictionary<string, ModuleAction> SystemActions;
        private Dictionary<string, ModuleRule> SystemRules;
        private Dictionary<string, ZeroActionParameterBase> SessionParams { get; set; }

        public void AddModule(ZeroModule module)
        {
            List<ZeroAction> actions = new List<ZeroAction>();
            List<ZeroRule> rules = new List<ZeroRule>();
            module.BuildPosibleActions(actions);
            foreach (var action in actions)
            {
                AddSessionAction(module.ModuleCode, action);
            }

            module.BuildRulesActions(rules);
            foreach (var rule in rules)
            {
                AddSessionRule(module.ModuleCode, rule);
            }
            module.SetSession(this);
            ModuleList.Add(module);
        }

        private bool ValidateActionParams(ref string result, ZeroAction buttonAction)
        {
            bool ret = true;
            ZeroActionParameterBase obj = null;
            foreach (var item in buttonAction.Parameters)
            {
                if (SessionParams.ContainsKey(item.Name))
                    obj = SessionParams[item.Name];

                if ((obj == null || obj.Value == null) && item.IsMandatory)
                {
                    ret = false;
                    result += "No se ha asignado el parámetro '" + item.Name + "'\n";
                }

                obj = null;
            }

            if (ret && buttonAction.RuleToSatisfy != null && buttonAction.RuleToSatisfy.CheckRuleAction != null)
                ValidateActionParams(ref result, buttonAction.RuleToSatisfy.CheckRuleAction);

            return ret;
        }

        private void AddSessionAction(int moduleCode, ZeroAction action)
        {
            Notifier.SetUserMessage(false, "Acción --> ''" + action.Name + "''");
            SystemActions.Add(action.Name, new ModuleAction { Action = action, Module = moduleCode });
        }

        private void AddSessionRule(int moduleCode, ZeroRule rule)
        {
            Notifier.SetUserMessage(false, "Regla --> ''" + rule.ID + "''");
            SystemRules.Add(rule.ID, new ModuleRule { Rule = rule, Module = moduleCode });
        }

        public List<ZeroAction> BuilSessionActions()
        {
            List<ZeroAction> validActions = new List<ZeroAction>();
            //El sistema posee reglas, estas reglas poseen (o no) una acción asociada para que valide la regla, las lineas siguentes
            //asocian a las reglas con sus respectias acciones
            foreach (var item in SystemRules)
            {
                item.Value.Rule.Satisfied = null;
                if (SystemActions.ContainsKey(item.Value.Rule.CheckRuleActionName))
                    item.Value.Rule.CheckRuleAction = SystemActions[item.Value.Rule.CheckRuleActionName].Action;
                else
                    item.Value.Rule.Satisfied = false;
            }

            //despues de haber cargado las reglas, ahora asocio las acciones finales a las reglas a validar
            foreach (var item in SystemActions)
            {
                if (!string.IsNullOrEmpty(item.Value.Action.RuleToSatisfyName))
                    item.Value.Action.RuleToSatisfy = SystemRules[item.Value.Action.RuleToSatisfyName].Rule;

                validActions.Add(item.Value.Action);
            }

            
            return validActions;
        }

        public bool ValidateRule(string ruleName)
        {
            string aux = "";
            return ValidateRule(ruleName, ref aux);
        }

        public bool ValidateRule(string ruleName, ref string result)
        {
            bool ret = false;
            if (SystemRules.ContainsKey(ruleName))
            {
                if (!SystemRules[ruleName].Rule.Satisfied.HasValue)
                    SystemRules[ruleName].Rule.Check();

                ret = SystemRules[ruleName].Rule.Satisfied.Value;
                result = SystemRules[ruleName].Rule.Result;

                if (Notifier != null)
                    Notifier.Log(System.Diagnostics.TraceLevel.Verbose, string.Format("Rule {0} - Status: {1}, result {2}", ruleName,ret,result));
            }
            else
            {
                if (Notifier != null)
                    Notifier.Log(System.Diagnostics.TraceLevel.Verbose, string.Format("Rule {0} does not exists", ruleName));
                result = "No existe la regla con el nombre ''" + ruleName + "''";
            }

            return ret;
        }
        
        public void AddNavigationParameter<T>(ZeroActionParameter<T> value)
        {
            if (!SessionParams.ContainsKey(value.Name))
                SessionParams.Add(value.Name, value);
            else
                SessionParams[value.Name] = value;
        }

        public ZeroActionParameter<T> GetParameter<T>(string name)
        {
            ZeroActionParameterBase O = null;

            if (SessionParams.ContainsKey(name))
            {
                O = SessionParams[name];
                if (O.IsVolatile)
                    SessionParams.Remove(name);
            }

            return (ZeroActionParameter<T>)O;
        }

        public ZeroActionParameter<T> GetParameter<T>()
        {
            ZeroActionParameter<T> param = (ZeroActionParameter<T>)SessionParams.Select(p => p.Value).FirstOrDefault(C => C.ParameterType == typeof(T));
            if (param != null && param.IsVolatile)
                SessionParams.Remove(param.Name);

            return param;
        }

        public bool CanExecute(ZeroAction Action, out string result)
        {
            bool ret = true;
            result = "";
            if (ret = ValidateActionParams(ref result, Action))
            {
                if (!Action.CanExecute(null))
                {
                    ret = false;
                    result = Action.RuleToSatisfy.Result;
                }
            }
            
            return ret;
        }
        
        public bool ExistsAction(string actionName, out ZeroAction action)
        {
            action = null;
            if (SystemActions.ContainsKey(actionName))
            {
                action = SystemActions[actionName].Action;
                return true;
            }
            return false;
        }
    }
}
