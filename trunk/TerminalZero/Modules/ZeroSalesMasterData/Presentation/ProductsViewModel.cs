using System.Collections.ObjectModel;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Windows;
using ZeroBusiness;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.Interfaces;
using ZeroCommonClasses.MVVMSupport;
using ZeroGUI;
using ZeroGUI.Reporting;
using ZeroMasterData.Pages;
using System.Windows.Input;
using ZeroMasterData.Pages.Controls;

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

        private ObservableCollection<ProductExtended> productList;

        public ObservableCollection<ProductExtended> ProductList
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

        private ProductExtended selectedProduct;

        public ProductExtended SelectedProduct
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

            ProductList = new ObservableCollection<ProductExtended>(BusinessContext.Instance.ModelManager.Products.Select(p => new ProductExtended { Product = p }));
            NewProductCommand.Finished +=
                (o, e) =>
                    {
                        var param = Terminal.Instance.Session[typeof (Product)];
                        if(param !=null)
                        ProductList.Add(new ProductExtended
                                            {Product = (Product) param.Value});
                    };

        }


        public class ProductExtended : ViewModelBase, ISelectable
        {
            public Product Product { get; set; }
            
            private ICommand updateProductCommand;

            public ICommand UpdateProductCommand
            {
                get
                {
                    return updateProductCommand??(updateProductCommand = new ZeroActionDelegate(this.UpdateProduct));
                }
                set
                {
                    if (updateProductCommand != value)
                    {
                        updateProductCommand = value;
                        OnPropertyChanged("UpdateProductCommand");
                    }
                }
            }

            private void UpdateProduct(object parameter)
            {
                ProductDetailViewModel viewModel = new ProductDetailViewModel();
                viewModel.Product = Product;
                viewModel.View.ControlMode = ZeroCommonClasses.Interfaces.ControlMode.Update;
                ZeroMessageBox.Show(viewModel.View, Properties.Resources.ProductEdit);
            }
            
            #region ISelectable Members

            public bool Contains(string data)
            {
                return Product.Contains(data);
            }

            public bool Contains(System.DateTime data)
            {
                return Product.Contains(data);
            }

            #endregion
        }
    }

    
}
