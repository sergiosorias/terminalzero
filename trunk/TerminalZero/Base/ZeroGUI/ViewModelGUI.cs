using System.Windows.Input;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.MVVMSupport;

namespace ZeroGUI
{
    public abstract class ViewModelGui : ViewModelBase
    {
        private NavigationBasePage view;

        public NavigationBasePage View
        {
            get { return view; }
            protected set 
            { 
                view = value;
                if(view!=null)
                    View.DataContext = this;
            }
        }

        private ICommand printAction;
        public virtual ICommand PrintCommand 
        {
            get { return printAction ?? (printAction = new ZeroActionDelegate(PrintCommandExecution)); }
        }

        protected ViewModelGui(NavigationBasePage view)
        {
            View = view;
        }

        protected virtual void PrintCommandExecution(object parameter)
        {

        }
        
    }
}
