using System.Collections.Generic;
using ZeroCommonClasses.GlobalObjects.Actions;

namespace ZeroCommonClasses
{
    public class ActionCollection
    {
        public static ZeroAction NullAction = new ZeroAction("", DoNothing, RuleCollection.NullRuleName , false);
        private static void DoNothing(object parameter)
        {

        }

        private Dictionary<string, ZeroAction> SystemActions { get; set; }

        internal ActionCollection()
        {
            SystemActions = new Dictionary<string, ZeroAction>();
        }
        
        public void Add(ZeroAction action)
        {
            Terminal.Instance.CurrentClient.Notifier.SetUserMessage(false, "Acción --> ''" + action.Name + "''");
            SystemActions.Add(action.Name, action);
        }

        public bool Exists(string actionName)
        {
            return SystemActions.ContainsKey(actionName);
        }

        public ZeroAction this[string actionName]
        {
            get
            {
                if (Exists(actionName))
                    return SystemActions[actionName];

                return NullAction;
            }
        }

        public void Refresh()
        {
            foreach (KeyValuePair<string, ZeroAction> systemAction in SystemActions)
            {
                systemAction.Value.RaiseCanExecuteChanged();
            }
        }

        public IEnumerable<ZeroAction> GetAll()
        {
            foreach (var item in SystemActions)
            {
                if (!string.IsNullOrEmpty(item.Value.RuleToSatisfyName))
                {
                    if (Terminal.Instance.Session.Rules.Exists(item.Value.RuleToSatisfyName))
                    {
                        item.Value.RuleToSatisfy = Terminal.Instance.Session.Rules[item.Value.RuleToSatisfyName];
                    }
                }
            }

            return SystemActions.Values;
        }
    }
}