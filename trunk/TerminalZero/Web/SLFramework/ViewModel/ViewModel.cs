using System;
using System.ComponentModel;
using System.Windows.Input;

namespace SLFramework.ViewModel
{
    public class ViewModel : INotifyPropertyChanged 
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }

    public class DelegateCommand : ICommand
    {
        private Action<object> action;
        private Predicate<object> predicate;

        public DelegateCommand(Action<object> action) : this(action, null)
        {
        }

        public DelegateCommand(Action<object> action, Predicate<object> predicate)
        {
            this.action = action;
            this.predicate = predicate;
        }

        #region Implementation of ICommand

        public bool CanExecute(object parameter)
        {
            return predicate == null || predicate(parameter);
        }

        public void Execute(object parameter)
        {
            action(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void InvokeCanExecuteChanged(EventArgs e)
        {
            EventHandler handler = CanExecuteChanged;
            if (handler != null) handler(this, e);
        }

        #endregion
    }
}
