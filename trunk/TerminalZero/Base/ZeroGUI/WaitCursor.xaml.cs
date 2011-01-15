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
using System.Windows.Media.Animation;
using System.Threading;

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
            this.Visibility = System.Windows.Visibility.Visible;
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
                this.Visibility = System.Windows.Visibility.Collapsed;
            }), null);
            
            
        }
    }
}
