using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZeroCommonClasses.Interfaces;
using System.Windows.Forms;

namespace ZeroConfiguration.Controls
{
    /// <summary>
    /// Interaction logic for Properties.xaml
    /// </summary>
    public partial class Properties : IZeroPage
    {
        Entities.ConfigurationEntities DataProvider = null;
        ITerminal Terminal;
        public Properties(ITerminal terminal)
        {
            InitializeComponent();
            Terminal = terminal;
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                DataProvider = new Entities.ConfigurationEntities();
                switch (Mode)
                {
                    case Mode.New:
                    case Mode.Update:
                    case Mode.Delete:
                        cbTerminals.ItemsSource = DataProvider.Terminals;
                        break;
                    case Mode.ReadOnly:
                        cbTerminals.ItemsSource = DataProvider.Terminals.Where(t => t.Code == Terminal.TerminalCode);
                        cbTerminals.IsEnabled = false;
                        terminalPropertiesDataGrid.IsEnabled = false;
                        modulesListView.IsEnabled = false;
                        cbTerminalIsActive.IsEnabled = false;
                        tbTerminal.IsReadOnly = tbTerminalDesc.IsReadOnly = true;
                        cbsendMasterData.Visibility = System.Windows.Visibility.Hidden;
                        break;
                    default:
                        break;
                }

                cbTerminals.SelectedItem = DataProvider.Terminals.First(t => t.Code == Terminal.TerminalCode);
            }
        }

        private void cbTerminals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int terminal = (int)cbTerminals.SelectedValue;
            Entities.Terminal T = DataProvider.Terminals.First(c => c.Code == terminal);
            tbTerminal.DataContext = T;
            if (T.LastSync != null)
            {
                lblLastSynclabel.Visibility = System.Windows.Visibility.Visible;
                lblLastSync.Content = T.LastSync.GetValueOrDefault(DateTime.MinValue).ToString("dd/MM hh:mm:ss tt");
            }
            else
                lblLastSynclabel.Visibility = System.Windows.Visibility.Hidden;

            cbTerminalIsActive.DataContext = T;
            cbsendMasterData.DataContext = T;
            terminalPropertiesDataGrid.ItemsSource = T.TerminalProperties;
            if (!T.Modules.IsLoaded)
                T.Modules.Load();
            modulesListView.ItemsSource = DataProvider.Modules;
        }

        #region IZeroPage Members

        public bool CanAccept()
        {
            DataProvider.SaveChanges();
            return true;
        }

        public bool CanCancel()
        {
            return true;
        }

        private Mode _Mode = Mode.ReadOnly;
        public Mode Mode
        {
            get
            {
                return _Mode;
            }
            set
            {
                _Mode = value;
            }
        }

        #endregion

        private void UserControl_Unloaded(object sender, RoutedEventArgs e)
        {
            SyncProcess.SyncCountdownTick -= sync_SyncCountdownTick;
            if (DataProvider != null)
                DataProvider.SaveChanges();
        }

        Synchronizer SyncProcess;
        public void UpdateTimeRemaining(Synchronizer sync)
        {
            SyncProcess = sync;
            sync.SyncCountdownTick += new EventHandler<Synchronizer.SyncCountdownTickEventArgs>(sync_SyncCountdownTick);
        }

        void sync_SyncCountdownTick(object sender, Synchronizer.SyncCountdownTickEventArgs e)
        {
            Dispatcher.Invoke(
                new MethodInvoker(
                    () => {
                        if (e.RemainingTime.Seconds == 0)
                        {
                            lblNextSync.Content = "Sincronizando!";
                        }
                        else
                        lblNextSync.Content = string.Format("{0:00}:{1:00}:{2:00}", e.RemainingTime.Hours, e.RemainingTime.Minutes, e.RemainingTime.Seconds);

                    }), null);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            // Do not load your data at design time.
            // if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            // {
            // 	//Load your data here and assign the result to the CollectionViewSource.
            // 	System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["Resource Key for CollectionViewSource"];
            // 	myCollectionViewSource.Source = your data
            // }
        }

        private void terminalPropertiesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
