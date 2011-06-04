using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace ZeroCommonClasses.GlobalObjects.Actions
{
    public class ZeroBackgroundAction : ZeroAction
    {
        private bool Async { get; set; }
        public ZeroBackgroundAction(string name, Action action, string ruleToSatisfy = null, bool isOnMenu = true, bool async = false)
            : base(name, action,ruleToSatisfy, isOnMenu)
        {
            Async = async;
        }

        public override void Execute(object parameter)
        {
            if (Async)
            {
                var th = new Thread(base.Execute);
                th.Start(parameter);
            }
            else
            {
                base.Execute(parameter);
            }
        }

    }
}
