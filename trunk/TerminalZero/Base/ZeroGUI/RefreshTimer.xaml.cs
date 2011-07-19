using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace ZeroGUI
{
    public partial class RefreshTimer : UserControl
    {
        private DateTime lastUpdate;
        public event EventHandler Tick;

        protected void OnTick()
        {
            if (Tick != null)
            {
                Tick(this, EventArgs.Empty);
            }
        }
        
        public bool TickOnStartEnable
        {
            get { return (bool)GetValue(TickOnStartEnableProperty); }
            set { SetValue(TickOnStartEnableProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TickOnStartEnable.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TickOnStartEnableProperty =
            DependencyProperty.Register("TickOnStartEnable", typeof(bool), typeof(RefreshTimer), null);

        public int RefreshEvery { get; set; }

        // Using a DependencyProperty as the backing store for RefreshEvery.  This enables animation, styling, binding, etc...

        private Timer timer;

        public RefreshTimer()
        {
            InitializeComponent();
            RefreshEvery = 40;
            sliderRefreshEvery.DataContext = this;
            timer = new Timer(timeElapsed,null,Timeout.Infinite, Timeout.Infinite);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnTick();
        }

        private void autoUpdate_Checked(object sender, RoutedEventArgs e)
        {
            lastUpdate = DateTime.Now;
            timer.Change(0, 200);
        }

        private void autoUpdate_Unchecked(object sender, RoutedEventArgs e)
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
        }
        
        private void timeElapsed(object o)
        {
            DateTime maxTime = lastUpdate.AddSeconds(RefreshEvery);
            DateTime stamp = DateTime.Now;
            TimeSpan ts1 = maxTime - lastUpdate;
            TimeSpan ts2 = stamp - lastUpdate;

            Dispatcher.BeginInvoke(new UpdateBar(UpdateBarDel), ts2.Ticks * 100 / ts1.Ticks);

            if (stamp > maxTime)
            {
                lastUpdate = stamp;
                Dispatcher.BeginInvoke(new Action(OnTick), null);
            }

        }

        private void UpdateBarDel(double newValue)
        {
            remainingTime.Value = newValue;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if(TickOnStartEnable)
                OnTick();
        }
    }
}
