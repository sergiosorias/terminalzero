using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ZeroCommonClasses.GlobalObjects
{
    public class ModuleNotificationEventArgs : EventArgs
    {
        public bool SomethingToShow { get; private set; }
        private object _ControlToShow = null;
        public object ControlToShow
        {
            get
            {
                SomethingToShow = false;
                return _ControlToShow;
            }
            set
            {
                SomethingToShow = true;
                _ControlToShow = value;
            }
        }
    }
}
