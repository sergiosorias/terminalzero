using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using ZeroBusiness.Entities.Configuration;
using ZeroCommonClasses.Interfaces;
using ZeroConfiguration.Presentantion;


namespace ZeroConfiguration.Pages
{
    /// <summary>
    /// Interaction logic for Properties.xaml
    /// </summary>
    [ToolboxItem(false)]
    public partial class Properties : ZeroGUI.NavigationBasePage
    {
        public Properties()
        {
            InitializeComponent();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsInDesignMode)
            {
                DataContext = new PropertiesViewModel(this);
            }
        }

        private void UserControlUnloaded(object sender, RoutedEventArgs e)
        {
            _syncProcess.SyncCountdownTick -= SyncSyncCountdownTick;
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

        
    }
}
