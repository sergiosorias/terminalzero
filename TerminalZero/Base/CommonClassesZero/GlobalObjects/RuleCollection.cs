using System;
using System.Collections.Generic;

namespace ZeroCommonClasses
{
    public class RuleCollection
    {
        public static string NullRuleName = "NullRule";
        private static Predicate<object> NullRule = o => false;

        private Dictionary<string, Predicate<object>> SystemRules { get; set; }

        internal RuleCollection()
        {
            SystemRules = new Dictionary<string, Predicate<object>>();
        }

        public void Add(string name, Predicate<object> rule)
        {
            Terminal.Instance.CurrentClient.Notifier.SetUserMessage(false, "Regla --> '" + name + "'");
            SystemRules.Add(name, rule);
        }

        public bool IsValid(string ruleName)
        {
            return SystemRules.ContainsKey(ruleName) && SystemRules[ruleName](null);
        }

        internal bool Exists(string ruleName)
        {
            return SystemRules.ContainsKey(ruleName);
        }

        public Predicate<object> this[string ruleName]
        {
            get 
            { 
                if(Exists(ruleName))
                {
                    return SystemRules[ruleName];
                }

                return (o) => false;
            }
        }
    }
}