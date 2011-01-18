using System;
using System.Windows;

namespace ZeroGUI
{
    public partial class RefreshTimer : System.Windows.Controls.UserControl
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

        public int RefreshEvery { get; set; }

        // Using a DependencyProperty as the backing store for RefreshEvery.  This enables animation, styling, binding, etc...

        private System.Threading.Timer timer;

        public RefreshTimer()
        {
            InitializeComponent();
            RefreshEvery = 40;
            sliderRefreshEvery.DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OnTick();
        }

        private void autoUpdate_Checked(object sender, RoutedEventArgs e)
        {
            lastUpdate = DateTime.Now;
            timer = new System.Threading.Timer(timeElapsed, null, 0, 200);
        }

        private void autoUpdate_Unchecked(object sender, RoutedEventArgs e)
        {
            timer.Dispose();
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
                Dispatcher.BeginInvoke(new Update(OnTick),null);
            }

        }

        private void UpdateBarDel(double newValue)
        {
            remainingTime.Value = newValue;
        }
    }
}
