using System.Collections.ObjectModel;
using System.Data.Objects;
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
        #region Commands
        public ZeroAction NewProductCommand
        {
            get
            {
                return Terminal.Instance.Session.Actions[Actions.OpenNewProductsMessage];
            }
        }

        private ZeroActionDelegate updatePricesCommand;

        public ZeroActionDelegate UpdatePricesCommand
        {
            get { return updatePricesCommand ?? (updatePricesCommand = new ZeroActionDelegate(OpenIncreaseProductMessage, (o) => Terminal.Instance.Session.Rules.IsValid(Rules.IsTerminalZero))); }
            set
            {
                if (updatePricesCommand != value)
                {
                    updatePricesCommand = value;
                    OnPropertyChanged("UpdatePricesCommand");
                }
            }
        }

        private void OpenIncreaseProductMessage(object parameter)
        {
            var viewModel = new ProductsUpdateViewModel();
            Terminal.Instance.CurrentClient.ShowDialog(viewModel.View,
            result =>
            {
                if (result)
                {
                    BusinessContext.Instance.Model.SaveChanges(SaveOptions.AcceptAllChangesAfterSave, true);
                }
            });
        }

        private ZeroActionDelegate updateProductCommand;

        public ZeroActionDelegate UpdateProductCommand
        {
            get { return updateProductCommand??(updatePricesCommand = new ZeroActionDelegate(o=>
                                                                                                 {
                                                                                                     if (SelectedProduct != null)
                                                                                                     {
                                                                                                         SelectedProduct.UpdateProductCommand.Execute(null);
                                                                                                     }
                                                                                                 }));
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

        #endregion

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
                BusinessContext.Instance.Model.Products.OrderBy(product => product.Name)
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

            ProductList = new ObservableCollection<ProductExtended>(BusinessContext.Instance.Model.Products.Select(p => new ProductExtended { Product = p }));
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

            private ZeroActionDelegate updateProductCommand;

            public ZeroActionDelegate UpdateProductCommand
            {
                get
                {
                    return updateProductCommand??(updateProductCommand = new ZeroActionDelegate(UpdateProduct));
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
                viewModel.View.ControlMode = ControlMode.Update;
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
