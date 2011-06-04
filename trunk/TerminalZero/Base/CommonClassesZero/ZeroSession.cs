using System;
using System.Collections.Generic;
using System.Linq;
using ZeroCommonClasses.GlobalObjects.Actions;

namespace ZeroCommonClasses
{
    public class ZeroSession
    {
        internal Dictionary<string, ActionParameterBase> SessionParams { get; set; }

        public RuleCollection Rules
        {
            get;
            private set;
        }

        public ActionCollection Actions
        {
            get;
            private set;
        }

        public ZeroSession()
        {
            SessionParams = new Dictionary<string, ActionParameterBase>();
            Actions = new ActionCollection();
            Rules = new RuleCollection();
        }

        public ActionParameterBase this[string name]
        {
            get { return GetParameter(name); }
            set { AddParameter(value); }
        }

        public ActionParameterBase this[Type type]
        {
            get { return GetParameter(type); }
            set { AddParameter(value); }
        }

        private void AddParameter(ActionParameterBase value)
        {
            if (!SessionParams.ContainsKey(value.Name))
                SessionParams.Add(value.Name, value);
            else
                SessionParams[value.Name] = value;

            Actions.Refresh();
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
    }
}
