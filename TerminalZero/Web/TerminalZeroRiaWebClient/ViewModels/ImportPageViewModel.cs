using System;
using System.Collections.ObjectModel;
using System.IO;
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
using TerminalZeroRiaWebClient.Web.Services;
using ZeroCommonClasses.Entities;

namespace TerminalZeroRiaWebClient.ViewModels
{
    public class ImportPageViewModel : ViewModel
    {
        public class PackItemViewModel : ViewModel
        {
            private Pack pack;

            public Pack Pack
            {
                get { return pack; }
                set
                {
                    if (pack != value)
                    {
                        pack = value;
                        OnPropertyChanged("Pack");
                    }
                }
            }

            private DelegateCommand downloadPack;

            public DelegateCommand DownloadPack
            {
                get { return downloadPack??(downloadPack = new DelegateCommand(DownloadPackAction)); }
                set
                {
                    if (downloadPack != value)
                    {
                        downloadPack = value;
                        OnPropertyChanged("DownloadPack");
                    }
                }
            }

            private void DownloadPackAction(object obj)
            {
                
            }

            private DelegateCommand reProcessPack;

            public DelegateCommand ReProcessPack
            {
                get { return reProcessPack??(reProcessPack = new DelegateCommand(ReprocessPackAction)); }
                set
                {
                    if (reProcessPack != value)
                    {
                        reProcessPack = value;
                        OnPropertyChanged("ReProcessPack");
                    }
                }
            }

            private void ReprocessPackAction(object obj)
            {
                
            }
        }

        #region Properties

        private bool hasChanges;

        public bool HasChanges
        {
            get { return hasChanges; }
            set
            {
                if (hasChanges != value)
                {
                    hasChanges = value;
                    OnPropertyChanged("HasChanges");
                }
            }
        }
        
        private DateTime endDate;

        public DateTime EndDate
        {
            get { return endDate; }
            set
            {
                if (endDate != value)
                {
                    endDate = value;
                    HasChanges = true;
                    OnPropertyChanged("EndDate");
                }
            }
        }

        private DateTime startDate;

        public DateTime StartDate
        {
            get { return startDate; }
            set
            {
                if (startDate != value)
                {
                    startDate = value;
                    HasChanges = true;
                    OnPropertyChanged("StartDate");
                }
            }
        }

        private DelegateCommand refresh;

        public DelegateCommand Refresh
        {
            get { return refresh??(refresh = new DelegateCommand(RefreshAction)); }
            set
            {
                if (refresh != value)
                {
                    refresh = value;
                    OnPropertyChanged("Refresh");
                }
            }
        }

        private void RefreshAction(object obj)
        {
            if (HasChanges)
            {
                LoadPacks();
                HasChanges = false;
            }
        }
        
        private ObservableCollection<PackItemViewModel> packs;

        public ObservableCollection<PackItemViewModel> Packs
        {
            get { return packs??(packs= new ObservableCollection<PackItemViewModel>()); }
            set
            {
                if (packs != value)
                {
                    packs = value;
                    OnPropertyChanged("Packs");
                }
            }
        }

        #endregion

        private TerminalZeroConfigDomainContext context;
        
        protected override void Initialize()
        {
            base.Initialize();
            StartDate = DateTime.Now.Date;
            EndDate = DateTime.Now.AddDays(1).Date;
            context = new TerminalZeroConfigDomainContext();
            LoadPacks();
        }
        
        private void LoadPacks()
        {
            AppViewModel.Instance.OpenBusyIndicator();
            Context.ResponseValidator.Hadle(
                context.Load(context.GetPacksQuery(StartDate, EndDate)),
                (successOperation) =>
                    {
                        Packs.Clear();
                        var obs = successOperation.Entities.ToObservable();
                        obs.Subscribe((pack)=>
                                          {
                                              Packs.Add(new PackItemViewModel {Pack = pack});
                                          });
                    },
                    (errorOperation) =>
                    {

                    },
                        (finalOperation) =>
                            {
                                AppViewModel.Instance.CloseBusyIndicator();
                            });
        }

        public void UploadPack(string fileName, Stream fileStream)
        {
            BinaryReader br = new BinaryReader(fileStream);
            
        }
    }
}
