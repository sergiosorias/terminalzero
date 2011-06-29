using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ZeroCommonClasses.GlobalObjects.Actions
{
    public class ZeroActionDelegate : ICommand
    {
        public event EventHandler Finished;
        protected void OnFinished()
        {
            if (Finished != null)
                Finished(this, EventArgs.Empty);
        }

        public Action<object> Action { get; private set; }
        public Predicate<object> Predicate { get; set; }

        public ZeroActionDelegate(Action<object> action) : this(action, null)
        {

        }

        public ZeroActionDelegate(Action<object> action, Predicate<object> canExecute)
        {
            Action = action;
            Predicate = canExecute;
        }

        #region Implementation of ICommand

        public virtual void Execute(object parameter)
        {
            Action(parameter);
            OnFinished();
        }

        public virtual bool CanExecute(object parameter)
        {
            return Predicate == null || Predicate(parameter);
        }

        public virtual event EventHandler CanExecuteChanged;

        public virtual void RaiseCanExecuteChanged()
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        #endregion
    }
}
