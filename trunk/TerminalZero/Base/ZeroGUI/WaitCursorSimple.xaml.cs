using System.Windows;
using System.Windows.Controls;

namespace ZeroGUI
{
    /// <summary>
    /// Interaction logic for WaitCursorSimple.xaml
    /// </summary>
    public partial class WaitCursorSimple : ContentControl
    {
        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(WaitCursorSimple), new UIPropertyMetadata("Cargando..."));

        
        public WaitCursorSimple()
        {
            InitializeComponent();
        }
    }
}
