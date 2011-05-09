using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.MasterData;
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
                    Context.Instance.Manager.ProductGroups.Count(),
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
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                grid1.DataContext = ProductGroupNew;
            }
        }
        
    }
}
