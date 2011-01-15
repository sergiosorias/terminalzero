using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
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
                Terminal ter = DataContext as Terminal;
                if (ter != null)
                {
                    if (!ter.IsSyncronized.GetValueOrDefault())
                    {
                        LinearGradientBrush b = BackBorder.Background as LinearGradientBrush;
                        b.GradientStops[2].Color = (Color)Resources["invalidRed"];
                        btnMore.Background = new SolidColorBrush(b.GradientStops[2].Color);
                        BackBorder.UpdateLayout();
                        
                    }

                    TerminalProperty prop = ter.TerminalProperties.FirstOrDefault(tp => tp.Code == "SYNC_EVERY");
                    if (prop != null)
                    {
                        MessageBoxPopUp.Text += string.Format("Conexiones cada {0} minutos.",  prop.Value);
                    }
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

        SolidColorBrush borderActive = new SolidColorBrush(Colors.Gray);
        SolidColorBrush borderInactive = new SolidColorBrush(Colors.Transparent);
        private void LayoutRoot_MouseEnter(object sender, MouseEventArgs e)
        {
            BackBorder.BorderBrush = borderActive;
        }

        private void LayoutRoot_MouseLeave(object sender, MouseEventArgs e)
        {
            BackBorder.BorderBrush = borderInactive;
        }
    }
}
