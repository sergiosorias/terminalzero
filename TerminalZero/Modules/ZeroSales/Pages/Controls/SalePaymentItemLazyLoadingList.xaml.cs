using ZeroGUI;

namespace ZeroSales.Pages.Controls
{
    /// <summary>
    /// Interaction logic for SalePaymentItemLazyLoadingList.xaml
    /// </summary>
    public partial class SalePaymentItemLazyLoadingList : LazyLoadingListControlUpgrade 
    {
        public SalePaymentItemLazyLoadingList()
        {
            InitializeComponent();
        }

        protected override void OnControlModeChanged(ZeroCommonClasses.Interfaces.ControlMode newMode)
        {
            base.OnControlModeChanged(newMode);
            if(newMode == ZeroCommonClasses.Interfaces.ControlMode.ReadOnly)
            {
                removeColumn.Visibility = System.Windows.Visibility.Collapsed;
            }
        }
    }
}
