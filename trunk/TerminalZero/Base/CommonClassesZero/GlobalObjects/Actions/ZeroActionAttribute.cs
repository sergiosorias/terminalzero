using System;
using System.Linq;
using System.Reflection;

namespace ZeroCommonClasses.GlobalObjects.Actions
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public sealed class ZeroActionAttribute : Attribute
    {
        private readonly string name;
        private readonly string ruleToSatisfy;
        private readonly bool isOnMenu;
        private readonly bool runOnBackground;
        private readonly bool async;

        public ZeroActionAttribute(string Name) 
            : this(Name, null)
        {
        }

        public ZeroActionAttribute(string Name, string RuleToSatisfy) 
            : this(Name, RuleToSatisfy, true)
        {
        }

        public ZeroActionAttribute(string Name, string RuleToSatisfy, bool IsOnMenu) 
            : this(Name, RuleToSatisfy, IsOnMenu, false, false)
        {
        }

        public ZeroActionAttribute(string Name, string RuleToSatisfy, bool IsOnMenu, bool RunOnBackground, bool Async)
        {
            name = Name;
            ruleToSatisfy = RuleToSatisfy;
            isOnMenu = IsOnMenu;
            runOnBackground = RunOnBackground;
            this.async = Async;
        }

        public ZeroAction GetAction(ZeroModule parent, MethodInfo info)
        {
            var action = BuildAction(parent,info);
            foreach (var attribute in info.GetCustomAttributes(typeof(ZeroActionParameterAttribute), true).Cast<ZeroActionParameterAttribute>())
            {
                action.AddParam(attribute.Key, attribute.IsMandatory);
            }
            return action;
        }

        private ZeroAction BuildAction(ZeroModule parent, MethodInfo info)
        {
            if(runOnBackground)
                return new ZeroBackgroundAction(name, ResolveDelegate(parent,info), ruleToSatisfy, isOnMenu, async);
            
            return new ZeroAction(name, ResolveDelegate(parent,info), ruleToSatisfy, isOnMenu);
        }

        private Action<object> ResolveDelegate(ZeroModule parent, MethodInfo info)
        {
            return (Action<object>)Delegate.CreateDelegate(typeof(Action<object>), parent,info);
        }
    }
    
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class ZeroRuleAttribute : Attribute
    {
        public string RuleName { get; private set; }
        
        public ZeroRuleAttribute(string RuleName)
        {
            this.RuleName = RuleName;
        }

        public Predicate<object> GetPredicate(ZeroModule parent, MethodInfo info)
        {
            return (Predicate<object>)Delegate.CreateDelegate(typeof(Predicate<object>), parent, info);
        }
        
    }

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
    public sealed class ZeroActionParameterAttribute : Attribute
    {
        public object Key { get; private set; }
        public bool IsMandatory { get; private set; }

        public ZeroActionParameterAttribute(string Name, bool IsMandatory)
        {
            this.Key = Name;
            this.IsMandatory = IsMandatory;
        }

        public ZeroActionParameterAttribute(Type Type, bool IsMandatory)
        {
            this.Key = Type;
            this.IsMandatory = IsMandatory;
        }
    }
}