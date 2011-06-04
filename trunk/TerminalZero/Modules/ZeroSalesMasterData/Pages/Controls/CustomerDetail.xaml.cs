using System.Data;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ZeroMasterData.Presentation;

namespace ZeroMasterData.Pages.Controls
{
    /// <summary>
    /// Interaction logic for SupplierDetail.xaml
    /// </summary>
    public partial class CustomerDetail : NavigationBasePage
    {
        public CustomerDetail()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsInDesignMode)
            {
                switch (ControlMode)
                {
                    case ControlMode.New:
                        Header = "Cliente Nuevo";
                        break;
                    case ControlMode.Update:
                        Header = "Editar Cliente";
                        break;
                }
            }
        }
    }
}
