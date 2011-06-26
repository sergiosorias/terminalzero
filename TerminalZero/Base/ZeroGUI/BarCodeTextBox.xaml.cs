using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using ZeroCommonClasses.GlobalObjects.Barcode;
using ZeroGUI.Classes;
using UserControl = System.Windows.Controls.UserControl;

namespace ZeroGUI
{
    /// <summary>
    /// Interaction logic for BarCodeTextBox.xaml
    /// </summary>
    public partial class BarCodeTextBox : UserControl
    {
        public event EventHandler<BarCodeValidationEventArgs> BarcodeValidating;

        protected void OnBarcodeValidating(BarCodeValidationEventArgs args)
        {
            if (BarcodeValidating != null)
            {
                BarcodeValidating(this, args);
            }
        }

        public event EventHandler<BarCodeEventArgs> BarcodeReceived;

        protected void OnBarcodeReceived(BarCodeEventArgs args)
        {
            if (BarcodeReceived != null)
            {
                BarcodeReceived(this, args);
            }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(BarCodeTextBox), new PropertyMetadata(""));

        public string Composition
        {
            get { return (string)GetValue(CompositionProperty); }
            set { SetValue(CompositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Composition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty CompositionProperty =
            DependencyProperty.Register("Composition", typeof(string), typeof(BarCodeTextBox), null);

        public string Mask
        {
            get { return (string)GetValue(MaskProperty); }
            set { SetValue(MaskProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Mask.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MaskProperty =
            DependencyProperty.Register("Mask", typeof(string), typeof(BarCodeTextBox), null);

        public ICommand BarcodeReceivedCommand
        {
            get { return (ICommand)GetValue(BarcodeReceivedCommandProperty); }
            set { SetValue(BarcodeReceivedCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for BarcodeReceivedCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty BarcodeReceivedCommandProperty =
            DependencyProperty.Register("BarcodeReceivedCommand", typeof(ICommand), typeof(BarCodeTextBox), null);

        public BarCodeTextBox()
        {
            InitializeComponent();
        }

        private void barCode_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (Text != null && Composition != null && Text.Length == Composition.Length && !Validation.GetHasError(barCode))
                {
                    string barcodeText = Text;
                    Text = "";
                    Dispatcher.BeginInvoke(new Update(() =>
                    {
                        var args = new BarCodeEventArgs(barcodeText, BarCodePart.BuildComposition(Composition, barcodeText));
                        if (BarcodeReceivedCommand != null)
                        {
                            BarcodeReceivedCommand.Execute(args);
                        }
                        OnBarcodeReceived(args);
                    }), null);
                }
                else
                {
                    e.Handled = true;     
                }
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            barCode.DataContext = this;
        }

        private void IsBarCodeRule_Validating(object sender, ValidationResultEventArgs e)
        {
            if (Composition.Length == e.Value.ToString().Length)
            {
                BarCodeValidationEventArgs args = new BarCodeValidationEventArgs(e.Value.ToString(), BarCodePart.BuildComposition(Composition, e.Value.ToString()));
                OnBarcodeValidating(args);
                if (BarcodeReceivedCommand != null)
                {
                    BarcodeReceivedCommand.CanExecute(args);
                }
                e.IsValid = args.Parts.TrueForAll(p => p.IsValid);
                e.ErrorContent = args.Error;
            }
        }

        public void SetFocus()
        {
            bool b = barCode.Focus();
        }
    }

    
}
