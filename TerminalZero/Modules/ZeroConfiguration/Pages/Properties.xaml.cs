using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using ZeroBusiness.Entities.Configuration;
using ZeroCommonClasses.Interfaces;


namespace ZeroConfiguration.Pages
{
    /// <summary>
    /// Interaction logic for Properties.xaml
    /// </summary>
    [ToolboxItem(false)]
    public partial class Properties : ZeroGUI.NavigationBasePage
    {
        ConfigurationModelManager _dataProvider;
        
        public Properties()
        {
            ControlMode = ControlMode.ReadOnly;
            _dataProvider = null;
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                _dataProvider = new ConfigurationModelManager();
                switch (ControlMode)
                {
                    case ControlMode.New:
                    case ControlMode.Update:
                    case ControlMode.Delete:
                        cbTerminals.ItemsSource = _dataProvider.Terminals;
                        break;
                    case ControlMode.ReadOnly:
                        cbTerminals.ItemsSource = _dataProvider.Terminals.Where(t => t.Code == ZeroCommonClasses.Terminal.Instance.TerminalCode);
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

                cbTerminals.SelectedItem = _dataProvider.Terminals.First(t => t.Code == ZeroCommonClasses.Terminal.Instance.TerminalCode);
            }
        }

        private void cbTerminals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var terminal = (int)cbTerminals.SelectedValue;
            Terminal T = _dataProvider.Terminals.First(c => c.Code == terminal);
            tbTerminal.DataContext = T;
            if (T.LastSync != null)
            {
                lblLastSynclabel.Visibility = Visibility.Visible;
                lblLastSync.Content = T.LastSync.GetValueOrDefault(DateTime.MinValue).ToString("dd/MM hh:mm:ss tt");
            }
            else
                lblLastSynclabel.Visibility = Visibility.Hidden;

            cbTerminalIsActive.DataContext = T;
            cbsendMasterData.DataContext = T;
            descriptionTextBox.DataContext = T;
            terminalPropertiesDataGrid.ItemsSource = T.TerminalProperties;
            if (!T.Modules.IsLoaded)
                T.Modules.Load();
            modulesListView.ItemsSource = _dataProvider.Modules;
        }

        #region IZeroPage Members

        public override bool CanAccept(object parameter)
        {
            bool ret = base.CanAccept(parameter);
            _dataProvider.SaveChanges();
            return ret;
        }

        public override bool CanCancel(object parameter)
        {
            _dataProvider.SaveChanges();
            return true;
        }

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
                    () =>
                    {
                        if (e.RemainingTime.TotalSeconds < 1)
                        {
                            lblNextSync.Content = "Sincronizando!";
                        }
                        else
                            lblNextSync.Content = string.Format("{0:00}:{1:00}:{2:00}", e.RemainingTime.Hours, e.RemainingTime.Minutes, e.RemainingTime.Seconds);

                    }), null);
        }

        private void ZeroToolBar_Save(object sender, RoutedEventArgs e)
        {
            _dataProvider.SaveChanges();
        }
    }
}
