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
using ZeroCommonClasses.Interfaces;
using ZeroMasterData.Entities;

namespace ZeroMasterData.Pages.Controls
{
    /// <summary>
    /// Interaction logic for ProductGroupDetail.xaml
    /// </summary>
    public partial class ProductGroupDetail : UserControl , IZeroPage
    {
        private MasterDataEntities DataProvider;
        public ProductGroupDetail(MasterDataEntities entities)
        {
            InitializeComponent();
            Mode = ZeroCommonClasses.Interfaces.Mode.New;
            DataProvider = entities;
            ProductGroupNew = ProductGroup.CreateProductGroup(
                    DataProvider.ProductGroups.Count(),
                     true);
        }

        public ProductGroupDetail(MasterDataEntities entities,  ProductGroup Data)
            : this(entities)
        {
            Mode = ZeroCommonClasses.Interfaces.Mode.Update;
            ProductGroupNew = Data;
        }

        public ProductGroup ProductGroupNew { get; private set; }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                grid1.DataContext = ProductGroupNew;
            }
        }

        #region Implementation of IZeroPage

        public Mode Mode
        {
            get; set; 
        }

        public bool CanAccept(object parameter)
        {
            return true;
        }

        public bool CanCancel(object parameter)
        {
            return true;
        }

        #endregion
    }
}
