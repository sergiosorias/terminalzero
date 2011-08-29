using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using SLFramework;

namespace TerminalZeroRiaWebClient.Controls
{
    public partial class TerminalStatus : UserControl
    {
        readonly SolidColorBrush _borderActive = new SolidColorBrush(Colors.Gray);
        readonly SolidColorBrush _borderInactive = new SolidColorBrush(Colors.Transparent);

        public TerminalStatus()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                popUpMoreInfo.IsOpen = false;

                //ServiceHelperReference.TerminalStatus ter = DataContext as ServiceHelperReference.TerminalStatus;

                //if (ter != null)
                //{
                //    if (!ter.Terminal.IsSyncronized.GetValueOrDefault())
                //    {
                //        LinearGradientBrush b = BackBorder.Background as LinearGradientBrush;
                //        b.GradientStops[2].Color = (Color)Resources["invalidRed"];
                //        btnMore.Background = new SolidColorBrush(b.GradientStops[2].Color);
                //        BackBorder.UpdateLayout();
                        
                //    }

                //    TerminalProperty prop = ter.Terminal.TerminalProperties.FirstOrDefault(tp => tp.Code == "SYNC_EVERY");
                //    if (prop != null)
                //    {
                //        MessageBoxPopUp.Text += string.Format("Conexiones cada {0} minutos.",  prop.Value);
                //    }
                //}
            }
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            Hidepoopanim.Begin();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Point position = ControlsExtentions.GetPosition(sender as UIElement, this);
            popUpMoreInfo.HorizontalOffset = position.X -60;
            popUpMoreInfo.VerticalOffset = position.Y -20;
            popUpMoreInfo.IsOpen = true;
        }

        private void LayoutRoot_MouseEnter(object sender, MouseEventArgs e)
        {
            BackBorder.BorderBrush = _borderActive;
        }

        private void LayoutRoot_MouseLeave(object sender, MouseEventArgs e)
        {
            BackBorder.BorderBrush = _borderInactive;
        }

        private void popUpMoreInfo_Opened(object sender, System.EventArgs e)
        {
            Showpoopanim.Begin();
        }

        private void Hidepoopanim_Completed(object sender, System.EventArgs e)
        {
            popUpMoreInfo.IsOpen = false;
        }
    }
}
