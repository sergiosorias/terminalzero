using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using SLFramework.Services;
using SLFramework.ViewModel;
using TerminalZeroRiaWebClient.Web.Models;
using TerminalZeroRiaWebClient.Web.Services;
using ZeroCommonClasses.Entities;

namespace TerminalZeroRiaWebClient.ViewModels
{
    public class HomeViewModel : ViewModel
    {
        #region Properties
        
        private ObservableCollection<TerminalStatus> statuses;

        public ObservableCollection<TerminalStatus> Statuses
        {
            get { return statuses; }
            set
            {
                if (statuses != value)
                {
                    statuses = value;
                    OnPropertyChanged("Statuses");
                }
            }
        }

        private ICommand refresh;

        public ICommand Refresh
        {
            get
            {
                return refresh??(refresh = new DelegateCommand(DoRefresh));
            }
            set
            {
                if (refresh != value)
                {
                    refresh = value;
                    OnPropertyChanged("Refresh");
                }
            }
        }

        private void DoRefresh(object obj)
        {
            LoadStatuses();
        }

        #endregion
        private TerminalZeroConfigDomainContext context;

        public HomeViewModel()
        {
            Statuses = new ObservableCollection<TerminalStatus>();
        }

        protected override void Initialize()
        {
            base.Initialize();
            this.context = new TerminalZeroConfigDomainContext();
            LoadStatuses();
        }

        private void LoadStatuses()
        {
            AppViewModel.Instance.OpenBusyIndicator();
            Context.ResponseValidator.Hadle(
                context.GetTerminalStatus(),
                (successOperation) =>
                    {
                        Statuses.Clear();
                        var obs = successOperation.Value.ToObservable();
                        obs.Subscribe((ts) => Statuses.Add(ts));
                    },
                (errorOperation) =>
                    {
                        errorOperation.MarkErrorAsHandled();
                    },
                (finalOperation) =>
                {
                    AppViewModel.Instance.CloseBusyIndicator();
                });
        }

    }
}
