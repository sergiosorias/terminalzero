using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ZeroMasterData.Entities;

namespace ZeroMasterData.Pages.Controls
{
    /// <summary>
    /// Interaction logic for ProductGroupDetail.xaml
    /// </summary>
    public partial class ProductGroupDetail : NavigationBasePage
    {
        private MasterDataEntities DataProvider;
        public ProductGroupDetail(MasterDataEntities entities)
        {
            InitializeComponent();
            DataProvider = entities;
            ProductGroupNew = ProductGroup.CreateProductGroup(
                    DataProvider.ProductGroups.Count(),
                     true);
        }

        public ProductGroupDetail(MasterDataEntities entities,  ProductGroup Data)
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
