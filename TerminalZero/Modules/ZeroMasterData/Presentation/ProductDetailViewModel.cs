using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Text;
using System.Windows;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ZeroMasterData.Pages.Controls;
using ZeroMasterData.Properties;

namespace ZeroMasterData.Presentation
{
    public class ProductDetailViewModel : ViewModelGui
    {
        private Product product;

        public Product Product
        {
            get
            {
                if(product==null)
                {
                    product = Product.CreateProduct(BusinessContext.Instance.Model.Products.Count(), true, true);
                    product.Price1 = Price.CreatePrice(BusinessContext.Instance.Model.Prices.Count(), true, 0);
                }
                return product;
            }
            set
            {
                if (product != value)
                {
                    product = value;
                    OnPropertyChanged("Product");
                }
            }
        }

        public string Header
        {
            get { return Resources.NewProduct; }
        }
        
        public ProductDetailViewModel()
            : base(new ProductDetail() )
        {
            
        }

        public override bool CanAccept(object parameter)
        {
            bool ret = base.CanAccept(parameter);
            string msg = string.Empty;
            if (ret)
            {
                if (View.ControlMode == ControlMode.New &&
                    BusinessContext.Instance.Model.Products.FirstOrDefault(pr => pr.MasterCode.Equals(Product.MasterCode)) != null)
                {
                    msg = "Codigo de product existente!\n Por favor ingrese otro código.";
                }

                if (Product.ByWeight && Product.Price1.Weight == null)
                {
                    msg += "\nPor favor ingrese una unidad de medida para el producto.";
                }

                if (string.IsNullOrWhiteSpace(msg))
                {
                    if (Product.Price1.Value == 0)
                    {
                        switch (
                            MessageBox.Show("¿Esta seguro que desea dejar el valor en cero?", "Precaución",
                                            MessageBoxButton.YesNo, MessageBoxImage.Question))
                        {
                            case MessageBoxResult.Cancel:
                            case MessageBoxResult.No:
                            case MessageBoxResult.None:
                                ret = false;
                                break;
                        }
                    }
                }
                else
                {
                    MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    ret = false;
                }
            }
            if(ret)
            {
                BusinessContext.Instance.Model.SaveChanges();
            }
            return ret;
        }

        public override bool CanCancel(object parameter)
        {
            bool ret = base.CanCancel(parameter);
            if (ret)
            {
                EntityObject obj = Product;
                if (obj.EntityState == EntityState.Modified)
                    BusinessContext.Instance.Model.Refresh(RefreshMode.StoreWins, Product);
            }
            return ret;
        }

    }
    
}
