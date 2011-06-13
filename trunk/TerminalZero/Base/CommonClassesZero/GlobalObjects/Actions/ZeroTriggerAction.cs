using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeroCommonClasses.GlobalObjects.Actions
{
    /// <summary>
    /// This Action will be executed after can execute become true
    /// </summary>
    public class ZeroTriggerAction : ZeroAction
    {
        public ZeroTriggerAction(string name, Action<object> action, string ruleToSatisfy = null)
            :base(name,action,ruleToSatisfy,false)
        {
            
        }

        public override void RaiseCanExecuteChanged()
        {
            base.RaiseCanExecuteChanged();
            if(CanExecute(null))
                Execute(null);
        }

    }
}
