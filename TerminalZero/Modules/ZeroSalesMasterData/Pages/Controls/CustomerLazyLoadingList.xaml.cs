using System.Windows;
using System.Windows.Controls;
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

        protected override void OnControlModeChanged(ZeroCommonClasses.Interfaces.ControlMode newMode)
        {
            base.OnControlModeChanged(newMode);
            if (ControlMode.HasFlag(ZeroCommonClasses.Interfaces.ControlMode.Selection))
            {
                name2Column.Visibility =
                    streetColumn.Visibility =
                    numberColumn.Visibility =
                    cityColumn.Visibility =
                    e_Mail1Column.Visibility = System.Windows.Visibility.Collapsed;

                MaxHeight = 400;
            }
        }
    }
}
