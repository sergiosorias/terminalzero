using System;
using System.Windows;
using System.Windows.Controls;
using ZeroBusiness;
using ZeroBusiness.Entities.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;

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
            ZeroAction
            newProduct = Terminal.Instance.Session.Actions[Actions.OpenNewProductsMessage];
            CommandBar.NewBtnCommand = newProduct;
            newProduct.Finished += NewBtnClick;

            CommandBar.Print += PrintBtnClicked;
        }

        private void NewBtnClick(object sender, EventArgs e)
        {
            object obj = Terminal.Instance.Session[typeof (Product)];
            if(obj != null)
            {
                productList.AddItem(obj as Product);
            }
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

        private void PrintBtnClicked(object sender, RoutedEventArgs e)
        {
            var pd = new PrintDialog();
            if(pd.ShowDialog().GetValueOrDefault())
                pd.PrintVisual(productList, "Lista de productos");
        }
        
    }
}
