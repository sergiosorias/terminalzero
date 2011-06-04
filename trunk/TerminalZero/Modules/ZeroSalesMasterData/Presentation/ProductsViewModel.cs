using System;
using System.Collections.ObjectModel;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ZeroBusiness;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroGUI;
using ZeroGUI.Reporting;
using ZeroMasterData.Pages;

namespace ZeroMasterData.Presentation
{
    public class ProductsViewModel : ViewModelGui
    {
        public ZeroAction NewProductCommand
        {
            get
            {
                return Terminal.Instance.Session.Actions[Actions.OpenNewProductsMessage];
            }
        }

        private ObservableCollection<Product> productList;

        public ObservableCollection<Product> ProductList
        {
            get { return productList; }
            set
            {
                if (productList != value)
                {
                    productList = value;
                    OnPropertyChanged("ProductList");
                }
            }
        }

        private Product selectedProduct;

        public Product SelectedProduct
        {
            get { return selectedProduct; }
            set
            {
                if (selectedProduct != value)
                {
                    selectedProduct = value;
                    OnPropertyChanged("SelectedProduct");
                }
            }
        }
        
        protected override void PrintCommandExecution(object parameter)
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

        public ProductsViewModel()
            :base(new ProductsView())
        {
            ProductList = new ObservableCollection<Product>(BusinessContext.Instance.ModelManager.Products);
        }
    }
}
