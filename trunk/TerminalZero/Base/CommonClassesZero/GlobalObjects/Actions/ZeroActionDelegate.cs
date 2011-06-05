using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ZeroCommonClasses.GlobalObjects.Actions
{
    public class ZeroActionDelegate : ICommand
    {
        public Action<object> Action { get; private set; }
        public Predicate<object> Predicate { get; private set; }

        public ZeroActionDelegate(Action<object> action) : this(action, null)
        {

        }

        public ZeroActionDelegate(Action<object> action, Predicate<object> canExecute)
        {
            Action = action;
            Predicate = canExecute;
        }

        #region Implementation of ICommand

        public void Execute(object parameter)
        {
            Action(parameter);
        }

        public bool CanExecute(object parameter)
        {
            return Predicate == null || Predicate(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void RaiseCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        #endregion
    }
}
