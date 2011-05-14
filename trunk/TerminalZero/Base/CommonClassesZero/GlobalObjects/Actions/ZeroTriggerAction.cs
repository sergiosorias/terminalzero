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
        public ZeroTriggerAction(string name, Action action)
            :base(ActionType.BackgroudAction,name,action)
        {
            CanExecuteChanged += ZeroTriggerAction_CanExecuteChanged;    
        }

        /// <summary>
        /// This method wil execute the action after the CanExecute returns true.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ZeroTriggerAction_CanExecuteChanged(object sender, EventArgs e)
        {
            if(CanExecute(null))
            {
                Execute(null);
            }
        }

    }
}
