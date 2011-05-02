using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using ZeroBusiness.Entities.Data;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;

namespace ZeroMasterData.Pages.Controls
{
    /// <summary>
    /// Interaction logic for ProductGroupDetail.xaml
    /// </summary>
    public partial class ProductGroupDetail : NavigationBasePage
    {
        private DataModelManager DataProvider;
        public ProductGroupDetail(DataModelManager entities)
        {
            InitializeComponent();
            DataProvider = entities;
            ProductGroupNew = ProductGroup.CreateProductGroup(
                    DataProvider.ProductGroups.Count(),
                     true);
        }

        public ProductGroupDetail(DataModelManager entities,  ProductGroup Data)
            : this(entities)
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
