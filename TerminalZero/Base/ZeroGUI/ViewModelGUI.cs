using System.Windows.Input;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.MVVMSupport;

namespace ZeroGUI
{
    public abstract class ViewModelGui : ViewModelBase
    {
        public NavigationBasePage View { get; private set; }

        private ICommand printAction;
        public virtual ICommand PrintCommand 
        {
            get { return printAction ?? (printAction = new ZeroActionDelegate(PrintCommandExecution)); }
        }

        protected ViewModelGui(NavigationBasePage view)
        {
            View = view;
            View.DataContext = this;
        }

        protected virtual void PrintCommandExecution(object parameter)
        {

        }
        
    }
}
