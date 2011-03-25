using System;
using System.Windows;
using System.Windows.Controls;

namespace ZeroBarcode.Pages.Controls
{
    /// <summary>
    /// Interaction logic for BarcodeGenerator.xaml
    /// </summary>
    public partial class BarcodeGenerator : UserControl
    {
        public event EventHandler PreviewClick;
        
        private void OnPreviewClick(EventArgs e)
        {
            EventHandler handler = PreviewClick;
            if (handler != null) handler(this, e);
        }

        public BarcodeGenerator()
        {
            InitializeComponent();
        }

        private void DatePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if (date.SelectedDate != null)
            {
                string chain = string.Format("{0:0000000}{1:00}{2:00}0", date.SelectedDate.Value.Year, date.SelectedDate.Value.Month, date.SelectedDate.Value.Day);
                BarcodeText.Text = EANBarcode.Instance.EAN13(chain) + EANBarcode.Instance.AddOn(chain);
                Button_Click(null, null);
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            date.SelectedDate = DateTime.Now;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OnPreviewClick(EventArgs.Empty);
            }
            catch (PrintDialogException ex)
            {
                MessageBox.Show(ex.ToString(), "Error");
            }
            catch 
            {

            }

        }

        private void slFontSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Button_Click(null, null);
        }

        private void slMarginUD_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (BarcodeText!= null)
            BarcodeText.Margin = new Thickness(
                BarcodeText.Margin.Left,
                e.NewValue,
                BarcodeText.Margin.Right,
                e.NewValue);
        }

        private void slMarginRL_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(BarcodeText!=null)
            BarcodeText.Margin = new Thickness(
                e.NewValue,
                BarcodeText.Margin.Top,
                e.NewValue,
                BarcodeText.Margin.Bottom);
        }

        
    }
}
