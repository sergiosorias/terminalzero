using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using TerminalZeroWebClient.ServiceHelperReference;

namespace TerminalZeroWebClient.Controls
{
    public partial class TerminalStatus : UserControl
    {
        public TerminalStatus()
        {
            InitializeComponent();
            
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (DataContext != null)
            {
                popUpMoreInfo.IsOpen = false;
// ReSharper disable SuggestUseVarKeywordEvident
                ServiceHelperReference.TerminalStatus ter = DataContext as ServiceHelperReference.TerminalStatus;
// ReSharper restore SuggestUseVarKeywordEvident
                if (ter != null)
                {
                    if (!ter.Terminal.IsSyncronized.GetValueOrDefault())
                    {
                        LinearGradientBrush b = BackBorder.Background as LinearGradientBrush;
                        b.GradientStops[2].Color = (Color)Resources["invalidRed"];
                        btnMore.Background = new SolidColorBrush(b.GradientStops[2].Color);
                        BackBorder.UpdateLayout();
                        
                    }

                    TerminalProperty prop = ter.Terminal.TerminalProperties.FirstOrDefault(tp => tp.Code == "SYNC_EVERY");
                    if (prop != null)
                    {
                        MessageBoxPopUp.Text += string.Format("Conexiones cada {0} minutos.",  prop.Value);
                    }
                    MessageBox2PopUp.Text = ter.Info ?? "Sin Información";
                }
            }
        }

        private void Border_MouseLeave(object sender, MouseEventArgs e)
        {
            popUpMoreInfo.IsOpen = false;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            popUpMoreInfo.IsOpen = !popUpMoreInfo.IsOpen;
        }

        readonly SolidColorBrush _borderActive = new SolidColorBrush(Colors.Gray);
        readonly SolidColorBrush _borderInactive = new SolidColorBrush(Colors.Transparent);

        private void LayoutRoot_MouseEnter(object sender, MouseEventArgs e)
        {
            BackBorder.BorderBrush = _borderActive;
        }

        private void LayoutRoot_MouseLeave(object sender, MouseEventArgs e)
        {
            BackBorder.BorderBrush = _borderInactive;
        }
    }
}
