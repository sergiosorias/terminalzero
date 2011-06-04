using System;
using ZeroBusiness.Entities.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ZeroMasterData.Presentation;

namespace ZeroMasterData.Pages
{
    /// <summary>
    /// Interaction logic for Product.xaml
    /// </summary>
    public partial class ProductsView : NavigationBasePage
    {
        public ProductsView()
        {
            InitializeComponent();
        }
        
        protected override void OnControlModeChanged(ControlMode newMode)
        {
 	         base.OnControlModeChanged(newMode);
             productList.ControlMode = ControlMode;
        }

        private void SearchBox_Search(object sender, SearchCriteriaEventArgs e)
        {
            e.Matches = productList.ApplyFilter(e.Criteria);
        }

        private void NewBtnClick(object sender, EventArgs e)
        {
            var obj = Terminal.Instance.Session[typeof(Product)];
            if (obj != null)
            {
                productList.AddItem(obj.Value as Product);
            }
        }

        private void NavigationBasePage_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            var viewModel = (ProductsViewModel)DataContext;
            viewModel.NewProductCommand.Finished += NewBtnClick;
        }

       

       
        
    }
}
