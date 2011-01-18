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
        }

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
                Visibility = System.Windows.Visibility.Collapsed;
            }), null);
            
            
        }
    }
}
