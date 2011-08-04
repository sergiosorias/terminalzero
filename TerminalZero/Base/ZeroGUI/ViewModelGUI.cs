using System.Windows.Input;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.MVVMSupport;

namespace ZeroGUI
{
    public abstract class ViewModelGui : ViewModelBase
    {
        #region Properties
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

        private string viewHeader;

        public string ViewHeader
        {
            get { return viewHeader; }
            protected set
            {
                if (viewHeader != value)
                {
                    viewHeader = value;
                    OnPropertyChanged("ViewHeader");
                }
            }
        }
        #endregion

        #region Commands
        private ICommand printAction;

        public virtual ICommand PrintCommand 
        {
            get { return printAction ?? (printAction = new ZeroActionDelegate(PrintCommandExecution)); }
        }

        protected virtual void PrintCommandExecution(object parameter)
        {

        }
        #endregion

        protected ViewModelGui(NavigationBasePage view)
        {
            View = view;
        }

        protected void Exit()
        {
            if (!ZeroCommonClasses.Terminal.Instance.Session.Actions.Exists(ZeroBusiness.Actions.AppHome)
                || !ZeroCommonClasses.Terminal.Instance.Session.Actions[ZeroBusiness.Actions.AppHome].TryExecute())
            {
                View.IsEnabled = false;
            }
        }

        
    }
}
