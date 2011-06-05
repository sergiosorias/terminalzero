using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZeroGUI
{
    /// <summary>
    /// Interaction logic for AutoCleanTextBlock.xaml
    /// </summary>
    public partial class AutoCleanTextBlock : TextBox
    {

        public AutoCleanTextBlock()
        {
            InitializeComponent();
        }

        protected override void OnTextChanged(TextChangedEventArgs e)
        {
            base.OnTextChanged(e);
            CreateCleanTimer();
        }

        private Timer cleanResTimer;

        private void CreateCleanTimer()
        {
            cleanResTimer = new Timer(cleanResTimer_Elapsed, null, 5000, 5000);
        }

        void cleanResTimer_Elapsed(object o)
        {
            cleanResTimer.Dispose();
            Dispatcher.BeginInvoke(new Update(
                () => { Text = ""; }
                ), null);

        }

        
    }
}
