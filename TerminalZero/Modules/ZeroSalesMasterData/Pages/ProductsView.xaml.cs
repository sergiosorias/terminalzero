using System;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using ZeroBusiness;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ZeroGUI.Reporting;

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
            ZeroAction newProduct = Terminal.Instance.Session.Actions[Actions.OpenNewProductsMessage];
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
            ReportBuilder.Create("Lista de productos",
                                  BusinessContext.Instance.ModelManager.Products.OrderBy(product => product.Name)
                                      .Select(product =>
                                          new
                                              {
                                                  Codigo = product.MasterCode,
                                                  Nombre = product.Name,
                                                  Precio = "$ " + SqlFunctions.StringConvert(product.Price1.Value),
                                                  Activo = product.Enable
                                              }).ToList(),
                                  new ReportColumnInfo("Código", new GridLength(150)),
                                  new ReportColumnInfo("Nombre", new GridLength(100, GridUnitType.Star)),
                                  new ReportColumnInfo("Precio", new GridLength(100)));

            //ReportBuilder.Create("algo", productList);
        }
        
    }
}
