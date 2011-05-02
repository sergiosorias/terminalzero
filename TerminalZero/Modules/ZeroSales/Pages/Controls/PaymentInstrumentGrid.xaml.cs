using System.Collections;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace ZeroSales.Pages.Controls
{
    /// <summary>
    /// Interaction logic for PaymentInstrumentGrid.xaml
    /// </summary>
    public partial class PaymentInstrumentGrid : UserControl
    {
        public PaymentInstrumentGrid()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            // Do not load your data at design time.
             if (!DesignerProperties.GetIsInDesignMode(this))
             {
                 LoadList(DataContext as IEnumerable);
             }
            
        }

        private void LoadList(IEnumerable entities)
        {
            var myCollectionViewSource = (CollectionViewSource)Resources["paymentInstrumentsViewSource"];
            myCollectionViewSource.Source = entities;
        }
    }
}
