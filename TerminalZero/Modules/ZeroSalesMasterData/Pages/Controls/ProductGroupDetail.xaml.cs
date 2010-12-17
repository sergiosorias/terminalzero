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
using ZeroMasterData.Entities;

namespace ZeroMasterData.Pages.Controls
{
    /// <summary>
    /// Interaction logic for ProductGroupDetail.xaml
    /// </summary>
    public partial class ProductGroupDetail : UserControl
    {
        private MasterDataEntities DataProvider;
        public ProductGroupDetail(MasterDataEntities entities)
        {
            InitializeComponent();
            DataProvider = entities;
        }

        public Entities.ProductGroup ProductGroupNew { get; private set; }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                ProductGroupNew = Entities.ProductGroup.CreateProductGroup(
                    DataProvider.ProductGroups.Count(), 
                     true);
                grid1.DataContext = ProductGroupNew;
            }
        }
    }
}
