using System.ComponentModel;
using System.Windows.Input;
using ZeroCommonClasses.GlobalObjects.Actions;

namespace ZeroCommonClasses.MVVMSupport
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        #region Implementation of INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

        public virtual bool CanCancel(object parameter)
        {
            return true;
        }

        public virtual bool CanAccept(object parameter)
        {
            return true;
        }

     }
}
