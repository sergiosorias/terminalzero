using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media.Animation;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroGUI;
using Button = System.Windows.Controls.Button;

namespace TerminalZeroClient.Pages
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : NavigationBasePage
    {
        public Home()
        {
            InitializeComponent();
            
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                IEnumerable<ZeroAction> actions = Terminal.Instance.Manager.GetShorcutActions();
                IEnumerator<ZeroAction> actionsEnumerator = actions.GetEnumerator();
                
                StartLoadButton(actionsEnumerator);
                

                //foreach (ZeroAction item in )
                //{
                //    Button b = new Button();
                //    b.Content = item.Alias;
                //    b.Style = (Style) App.Current.Resources["homeButtonStyle"];
                //    b.Command = item;
                    
                //    ButtonsContent.Children.Add(b);
                //}

            }
        }

        private delegate void RunButtonLoad(IEnumerator<ZeroAction> actions);

        private void StartLoadButton(IEnumerator<ZeroAction> actionsEnumerator)
        {
            if (actionsEnumerator.MoveNext())
            {
                Button b = new Button();
                b.Content = actionsEnumerator.Current.Alias;
                b.Style = (Style) App.Current.Resources["homeButtonStyle"];
                b.Command = actionsEnumerator.Current;
                ButtonsContent.Children.Add(b);
                var fade = new DoubleAnimation() {From = 0, To = 1, Duration = TimeSpan.FromMilliseconds(200),};
                var elastic = new PowerEase();
                elastic.EasingMode = EasingMode.EaseOut;
                fade.EasingFunction = elastic;
                Storyboard.SetTarget(fade, b);
                Storyboard.SetTargetProperty(fade, new PropertyPath(OpacityProperty));
                var sb = new Storyboard();
                sb.Children.Add(fade);
                sb.Completed += (sender, e) =>
                {
                    Dispatcher.Invoke(new RunButtonLoad(StartLoadButton), actionsEnumerator);
                };
                sb.Begin();
                

            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            
        }
    }
}
