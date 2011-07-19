using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZeroCommonClasses.GlobalObjects.Barcode;
using ZeroGUI.Classes;

namespace ZeroGUI
{
    /// <summary>
    /// Interaction logic for BarCodeTextBox.xaml
    /// </summary>
    public partial class BarCodeTextBox : TextBox
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
        
        public FrameworkElement NextControl
        {
            get { return (FrameworkElement)GetValue(NextControlProperty); }
            set { SetValue(NextControlProperty, value); }
        }

        // Using a DependencyProperty as the backing store for NextControl.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty NextControlProperty =
            DependencyProperty.Register("NextControl", typeof(FrameworkElement), typeof(BarCodeTextBox), new UIPropertyMetadata(null));

        private bool isValid;
        private List<BarCodePart> compositionParts;

        public BarCodeTextBox()
        {
            InitializeComponent();
        }

        private void barCode_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrEmpty(Text) && !string.IsNullOrEmpty(Composition) && Text.Length == Composition.Length)
            {
                ValidateText();
                if (e.Key == Key.Enter)
                {
                    if (isValid)
                    {
                        SendNewBarcode();
                    }
                    else
                    {
                        e.Handled = true;
                    }
                }
            }
        }

        private void SendNewBarcode()
        {
            string barcodeText = Text;
            Text = "";
                        
            Dispatcher.BeginInvoke(new Action(() =>
            {
                var args = new BarCodeEventArgs(barcodeText, compositionParts);
                if (BarcodeReceivedCommand != null)
                {
                    BarcodeReceivedCommand.Execute(args);
                }
                OnBarcodeReceived(args);
                if (NextControl != null)
                    NextControl.Focus();
                
            }), null);
        }

        private void ValidateText()
        {
            isValid = true;
            compositionParts = BarCodePart.BuildComposition(Composition, Text);
            var validating = new BarCodeValidationEventArgs(Text, compositionParts);
            OnBarcodeValidating(validating);
            if (BarcodeReceivedCommand != null)
            {
                BarcodeReceivedCommand.CanExecute(validating);
            }
            if (!validating.Parts.TrueForAll(p => p.IsValid))
            {
                isValid = false;
            }
        }
    }

    
}
