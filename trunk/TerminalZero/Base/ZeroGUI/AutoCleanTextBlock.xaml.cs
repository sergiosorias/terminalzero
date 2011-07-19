using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace ZeroGUI
{
    /// <summary>
    /// Interaction logic for AutoCleanTextBlock.xaml
    /// </summary>
    public partial class AutoCleanTextBlock : TextBox
    {
        public int ClearTimeOut
        {
            get { return (int)GetValue(ClearTimeOutProperty); }
            set { SetValue(ClearTimeOutProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CleatTimeOut.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ClearTimeOutProperty =
            DependencyProperty.Register("ClearTimeOut", typeof(int), typeof(AutoCleanTextBlock), new PropertyMetadata(5000));

        public AutoCleanTextBlock()
        {
            InitializeComponent();
            cleanResTimer = new Timer(cleanResTimer_Elapsed, null, ClearTimeOut, Timeout.Infinite);
            TextChanged += (o, e) => { 
                CreateCleanTimer();
                Opacity = 1;
            };
        }

        private Timer cleanResTimer;

        private void CreateCleanTimer()
        {
            cleanResTimer.Change(ClearTimeOut, Timeout.Infinite);
        }

        private void cleanResTimer_Elapsed(object o)
        {
            Dispatcher.BeginInvoke(new Action(() => Opacity = 0), null);
        }

        
    }
}
