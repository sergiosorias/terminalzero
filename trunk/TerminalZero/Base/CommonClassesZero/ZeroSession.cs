using System;
using System.Collections.Generic;
using System.Linq;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.Interfaces;

namespace ZeroCommonClasses
{
    public class ZeroSession
    {
        public ZeroSession()
        {
            SessionParams = new Dictionary<string, ActionParameterBase>();
            SystemActions = new Dictionary<string, ZeroAction>();
            SystemRules = new Dictionary<string, Predicate<object>>();
        }

        public Dictionary<string, ZeroAction> SystemActions { get; private set; }
        public Dictionary<string, Predicate<object>> SystemRules { get; private set; }
        internal Dictionary<string, ActionParameterBase> SessionParams { get; set; }

        public void AddAction(ZeroAction action)
        {
            Terminal.Instance.CurrentClient.Notifier.SetUserMessage(false, "Acción --> ''" + action.Name + "''");
            SystemActions.Add(action.Name, action);
        }

        public void AddRule(string name,Predicate<object> rule)
        {
            Terminal.Instance.CurrentClient.Notifier.SetUserMessage(false, "Regla --> '" + name + "'");
            SystemRules.Add(name, rule);
        }

        private void AddNavigationParameter(ActionParameterBase value)
        {
            if (!SessionParams.ContainsKey(value.Name))
                SessionParams.Add(value.Name, value);
            else
                SessionParams[value.Name] = value;

            foreach (KeyValuePair<string, ZeroAction> systemAction in SystemActions)
            {
                systemAction.Value.RaiseCanExecuteChanged();
            }
        }

        private ActionParameterBase GetParameter(string name)
        {
            ActionParameterBase param = null;

            if (SessionParams.ContainsKey(name))
            {
                param = SessionParams[name];
                if (param.IsVolatile)
                    SessionParams.Remove(name);
            }

            return param;
        }

        private ActionParameterBase GetParameter(Type type)
        {
            var param = SessionParams.Select(p => p.Value).FirstOrDefault(c => c.ParameterType == type);
            if (param != null && param.IsVolatile)
                SessionParams.Remove(param.Name);

            return param;
        }

        public ActionParameterBase this[string name]
        {
            get { return GetParameter(name); }
            set { AddNavigationParameter(value); }
        }

        public ActionParameterBase this[Type type]
        {
            get { return GetParameter(type); }
            set { AddNavigationParameter(value); }
        }
        

        
    }
}
