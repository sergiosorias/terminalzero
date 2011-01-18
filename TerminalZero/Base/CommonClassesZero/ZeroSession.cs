using System.Collections.Generic;
using System.Linq;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.Interfaces;

namespace ZeroCommonClasses
{
    public class ZeroSession
    {
        public ZeroSession()
        {
            SessionParams = new Dictionary<string, ZeroActionParameterBase>();
            SystemActions = new Dictionary<string, ZeroAction>();
            SystemRules = new Dictionary<string, ZeroRule>();
            ModuleList = new List<ZeroModule>();
            AddNavigationParameter(new ZeroActionParameter<List<ZeroModule>>("ExistingModules",true,ModuleList));
        }

        public IProgressNotifier Notifier { get; set; }

        public List<ZeroModule> ModuleList { get; private set; }

        public Dictionary<string, ZeroAction> SystemActions { get; private set; }
        public Dictionary<string, ZeroRule> SystemRules{ get; private set; }
        public Dictionary<string, ZeroActionParameterBase> SessionParams { get; set; }

        public void AddModule(ZeroModule module)
        {
            ModuleList.Add(module);
        }

        public void AddAction(ZeroAction action)
        {
            Notifier.SetUserMessage(false, "Acción --> ''" + action.Name + "''");
            SystemActions.Add(action.Name, action);
        }

        public void AddRule(ZeroRule rule)
        {
            Notifier.SetUserMessage(false, "Regla --> ''" + rule.ID + "''");
            SystemRules.Add(rule.ID, rule);
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

        

        
    }
}
