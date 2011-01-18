using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using ZeroCommonClasses.Interfaces;

namespace ZeroConfiguration.Controls
{
    /// <summary>
    /// Interaction logic for Properties.xaml
    /// </summary>
    public partial class Properties : IZeroPage
    {
        Entities.ConfigurationEntities _dataProvider;
        readonly ITerminal _terminal;
        public Properties(ITerminal terminal)
        {
            Mode = Mode.ReadOnly;
            _dataProvider = null;
            InitializeComponent();
            _terminal = terminal;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                _dataProvider = new Entities.ConfigurationEntities();
                switch (Mode)
                {
                    case Mode.New:
                    case Mode.Update:
                    case Mode.Delete:
                        cbTerminals.ItemsSource = _dataProvider.Terminals;
                        break;
                    case Mode.ReadOnly:
                        cbTerminals.ItemsSource = _dataProvider.Terminals.Where(t => t.Code == _terminal.TerminalCode);
                        cbTerminals.IsEnabled = false;
                        terminalPropertiesDataGrid.IsEnabled = false;
                        modulesListView.IsEnabled = false;
                        cbTerminalIsActive.IsEnabled = false;
                        tbTerminal.IsReadOnly = descriptionTextBox.IsReadOnly = true;
                        cbsendMasterData.Visibility = Visibility.Hidden;
                        break;
                    default:
                        break;
                }

                cbTerminals.SelectedItem = _dataProvider.Terminals.First(t => t.Code == _terminal.TerminalCode);
            }
        }

        private void cbTerminals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int terminal = (int)cbTerminals.SelectedValue;
            Entities.Terminal T = _dataProvider.Terminals.First(c => c.Code == terminal);
            tbTerminal.DataContext = T;
            if (T.LastSync != null)
            {
                lblLastSynclabel.Visibility = Visibility.Visible;
                lblLastSync.Content = T.LastSync.GetValueOrDefault(DateTime.MinValue).ToString("dd/MM hh:mm:ss tt");
            }
            else
                lblLastSynclabel.Visibility = System.Windows.Visibility.Hidden;

            cbTerminalIsActive.DataContext = T;
            cbsendMasterData.DataContext = T;
            descriptionTextBox.DataContext = T;
            terminalPropertiesDataGrid.ItemsSource = T.TerminalProperties;
            if (!T.Modules.IsLoaded)
                T.Modules.Load();
            modulesListView.ItemsSource = _dataProvider.Modules;
        }

        #region IZeroPage Members
        
        public bool CanAccept()
        {
            _dataProvider.SaveChanges();
            return true;
        }

        public bool CanCancel()
        {
            _dataProvider.SaveChanges();
            return true;
        }

        public Mode Mode { get; set; }

        #endregion

        private void UserControlUnloaded(object sender, RoutedEventArgs e)
        {
            _syncProcess.SyncCountdownTick -= SyncSyncCountdownTick;
            if (_dataProvider != null)
                _dataProvider.SaveChanges();
        }

        private Synchronizer _syncProcess;
        public void UpdateTimeRemaining(Synchronizer sync)
        {
            _syncProcess = sync;
            sync.SyncCountdownTick += SyncSyncCountdownTick;
        }

        private void SyncSyncCountdownTick(object sender, Synchronizer.SyncCountdownTickEventArgs e)
        {
            Dispatcher.Invoke(
                new MethodInvoker(
                    () => {
                        if (e.RemainingTime.TotalSeconds < 1)
                        {
                            lblNextSync.Content = "Sincronizando!";
                        }
                        else
                        lblNextSync.Content = string.Format("{0:00}:{1:00}:{2:00}", e.RemainingTime.Hours, e.RemainingTime.Minutes, e.RemainingTime.Seconds);

                    }), null);
        }

        private void UserControlLoaded(object sender, RoutedEventArgs e)
        {

            // Do not load your data at design time.
            // if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            // {
            // 	//Load your data here and assign the result to the CollectionViewSource.
            // 	System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["Resource Key for CollectionViewSource"];
            // 	myCollectionViewSource.Source = your data
            // }
            // Do not load your data at design time.
            // if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            // {
            // 	//Load your data here and assign the result to the CollectionViewSource.
            // 	System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["Resource Key for CollectionViewSource"];
            // 	myCollectionViewSource.Source = your data
            // }
        }

        private void TerminalPropertiesDataGridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
