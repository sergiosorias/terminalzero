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
using ZeroBusiness.Entities.Data;
using ZeroGUI;

namespace ZeroSales.Pages.Controls
{
    /// <summary>
    /// Interaction logic for SalePaymentItemLazyLoadingList.xaml
    /// </summary>
    public partial class SalePaymentItemLazyLoadingList : LazyLoadingListControl 
    {
        public SalePaymentItemLazyLoadingList()
        {
            InitializeComponent();
        }

        private void ClickeableItemButton_Click(object sender, RoutedEventArgs e)
        {
            TryRemoveItem(SelectedItem as SalePaymentItem);
        }
    }
}
