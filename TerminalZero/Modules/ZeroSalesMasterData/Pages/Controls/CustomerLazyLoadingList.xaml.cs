using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroGUI;

namespace ZeroMasterData.Pages.Controls
{
    /// <summary>
    /// Interaction logic for SupplierGrid.xaml
    /// </summary>
    public partial class CustomerLazyLoadingList : LazyLoadingListControl
    {
        public CustomerLazyLoadingList()
        {
            InitializeComponent();
        }

        private void ClickeableItemButton_Click(object sender, RoutedEventArgs e)
        {
            var detail = new CustomerDetail((int)((Button)sender).DataContext);
            bool? ret = ZeroMessageBox.Show(detail, string.Format("Editar cliente {0}", (int)((Button)sender).DataContext));
            if (ret.HasValue && ret.Value)
            {
                
            }
        }

        private void LazyLoadingListControl_Loaded(object sender, RoutedEventArgs e)
        {
            StartListLoad(BusinessContext.Instance.Manager.Customers);
        }
    }
}
