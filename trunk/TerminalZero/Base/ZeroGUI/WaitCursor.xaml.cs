using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace ZeroGUI
{
    /// <summary>
    /// Interaction logic for WaitCursor.xaml
    /// </summary>
    public partial class WaitCursor : UserControl
    {
        public WaitCursor()
        {
            InitializeComponent();
            DataContext = this;
            
        }

        public string WaitingText
        {
            get { return (string)GetValue(WaitingTextProperty); }
            set { SetValue(WaitingTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for WaitingText.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty WaitingTextProperty =
            DependencyProperty.Register("WaitingText", typeof(string), typeof(WaitCursor), new PropertyMetadata("Loading.."));
        
        public void Start()
        {
            Visibility = System.Windows.Visibility.Visible;
            Storyboard sb = Resources["rotation"] as Storyboard;
            if(sb!=null)
            {
                sb.Begin();
            }
        }

        public void Stop()
        {
            Storyboard sb = Resources["rotation"] as Storyboard;
            if (sb != null)
            {
                sb.Stop();
            }
            Dispatcher.BeginInvoke(new Update(() =>
            {
                Visibility = Visibility.Collapsed;
            }), null);
            
            
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Stop();
        }
    }
}
