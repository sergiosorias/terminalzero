using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.ServiceModel.DomainServices.Client.ApplicationServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using SLFramework.ViewModel;

namespace TerminalZeroRiaWebClient.ViewModels
{
    public sealed class AppViewModel : ViewModel
    {
        #region Singleton
        private AppViewModel()
        {
            
        }

        private static AppViewModel instance;

        public static AppViewModel Instance
        {
            get
            {
                if (instance == null)
                    instance = new AppViewModel();

                return instance;
            }
        } 
        #endregion

        #region Properties
        private int isBusyCount = 0;
        private bool isBusy;

        public bool IsBusy
        {
            get { return isBusy; }
        }

        #endregion

        public void OpenBusyIndicator()
        {
            isBusyCount++;
            isBusy = true;
            RaisePropertyChanged("IsBusy");
        }

        public void CloseBusyIndicator()
        {
            isBusyCount--;
            if (isBusyCount == 0)
            {
                isBusy = false;
                RaisePropertyChanged("IsBusy");
            }
        }

        public event EventHandler Initialized;

        private void InvokeInitialized()
        {
            EventHandler handler = Initialized;
            if (handler != null) handler(this, EventArgs.Empty);
        }

        protected override void Initialize()
        {
            base.Initialize();
            WebContext.Current.Authentication.LoadUser(UserLoaded, null);
            WebContext.Current.Authentication.LoggedIn += Authentication_LoggedIn;
        }

        private void UserLoaded(LoadUserOperation loadOperation)
        {
            if (!loadOperation.HasError)
            {

            }
        }

        private void Authentication_LoggedIn(object sender, AuthenticationEventArgs e)
        {
            if(e.User.Identity.IsAuthenticated)
                InvokeInitialized();
        }

        
    }
}
