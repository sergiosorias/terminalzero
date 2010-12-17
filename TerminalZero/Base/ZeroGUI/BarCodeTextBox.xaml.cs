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
using ZeroCommonClasses.GlobalObjects.Barcode;
using ZeroGUI.Classes;
using System.Text.RegularExpressions;

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

        public BarCodeTextBox()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            barCode.DataContext = this;
        }

        private void barCode_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (Text != null &&
                Composition != null &&
                Text.Length == Composition.Length
                && !Validation.GetHasError(barCode))
            {
                OnBarcodeReceived(new BarCodeEventArgs(Text, null));
                Text = "";
            }
        }

        private void IsBarCodeRule_Validating(object sender, ValidationResultEventArgs e)
        {
            BarCodeValidationEventArgs args = new BarCodeValidationEventArgs(BarCodePart.BuildComposition(Composition, e.Value.ToString()));
            if (!(args.Parts[0].IsValid = args.Parts[0].Code > 0 && args.Parts[0].Code <= 12))
                args.Error = "Mes incorrecto";
            else if (!(args.Parts[1].IsValid = args.Parts[1].Code > 0 && args.Parts[1].Code <= 31))
                args.Error = "Día incorrecto";
            else
                OnBarcodeValidating(args);

            e.IsValid = args.Parts.TrueForAll(p => p.IsValid);
            e.ErrorContent = args.Error;

        }
    }

}
