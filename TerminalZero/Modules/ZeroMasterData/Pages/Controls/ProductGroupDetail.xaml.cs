using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;

namespace ZeroMasterData.Pages.Controls
{
    /// <summary>
    /// Interaction logic for ProductGroupDetail.xaml
    /// </summary>
    public partial class ProductGroupDetail : NavigationBasePage
    {
        public ProductGroupDetail()
        {
            InitializeComponent();
            ProductGroupNew = ProductGroup.CreateProductGroup(
                    BusinessContext.Instance.Model.ProductGroups.Count(),
                     true);
        }

        public ProductGroupDetail(ProductGroup Data) :this()
        {
            ControlMode = ControlMode.Update;
            ProductGroupNew = Data;
        }

        public ProductGroup ProductGroupNew { get; private set; }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsInDesignMode)
            {
                grid1.DataContext = ProductGroupNew;
            }
        }
        
    }
}
