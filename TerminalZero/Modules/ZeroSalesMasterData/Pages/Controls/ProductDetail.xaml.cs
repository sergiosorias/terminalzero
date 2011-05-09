using System;
using System.ComponentModel;
using System.Data;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.MasterData;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;

namespace ZeroMasterData.Pages.Controls
{
    /// <summary>
    /// Interaction logic for ProductDetail.xaml
    /// </summary>
    public partial class ProductDetail : NavigationBasePage
    {
        public Product CurrentProduct { get; private set; }

        public ProductDetail()
        {
            InitializeComponent();
        }

        public ProductDetail(int productCode) : this()
        {
            CurrentProduct = Context.Instance.Manager.Products.First(p => p.Code == productCode);
            ControlMode = ControlMode.Update;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                LoadProduct();
                LoadTaxes();
                LoadPriceWeights();
                LoadProductGroup();
                valueTextBox.Text = "";
            }
        }

        private void LoadTaxes()
        {
            taxesComboBox.ItemsSource = Context.Instance.Manager.Taxes;
            switch (ControlMode)
            {
                case ControlMode.New:
                    taxesComboBox.SelectedIndex = 1;
                    break;
                case ControlMode.Update:
                    if (!CurrentProduct.TaxReference.IsLoaded)
                        CurrentProduct.TaxReference.Load();
                    taxesComboBox.SelectedItem = CurrentProduct.Tax;
                    break;
                case ControlMode.Delete:
                    break;
                case ControlMode.ReadOnly:
                    break;
                default:
                    break;
            }
        }

        private void LoadPriceWeights()
        {
            foreach (var item in Context.Instance.Manager.Weights)
            {
                weightBox.Items.Add(item);
            }

            if (ControlMode == ControlMode.Update
                && CurrentProduct.PriceCode.HasValue)
            {
                if (!CurrentProduct.PriceReference.IsLoaded)
                    CurrentProduct.PriceReference.Load();
                if (!CurrentProduct.Price1.WeightReference.IsLoaded)
                    CurrentProduct.Price1.WeightReference.Load();

                weightBox.SelectedItem = CurrentProduct.Price1.Weight;

            }

        }

        private void LoadProductGroup()
        {
            foreach (var item in Context.Instance.Manager.ProductGroups)
            {
                groupBox.Items.Add(item);
                if (ControlMode == ControlMode.Update && CurrentProduct.Group1 == item.Code)
                {
                    groupBox.SelectedItem = item;
                }
            }
        }

        private void LoadProduct()
        {
            Price P = null;

            switch (ControlMode)
            {
                case ControlMode.New:
                    CurrentProduct = Product.CreateProduct(
                        Context.Instance.Manager.Products.Count()
                        , true, true);
                    
                    P = Price.CreatePrice(
                        Context.Instance.Manager.Prices.Count(),
                        true, 0);
                    break;
                case ControlMode.Update:
                    if (CurrentProduct.PriceCode.HasValue)
                        P = Context.Instance.Manager.Prices.First(p => p.Code == CurrentProduct.PriceCode);
                    else
                    {
                        P = Price.CreatePrice(
                        Context.Instance.Manager.Prices.Count(),
                        true, 0);
                    }
                    masterCodeTextBox.IsReadOnly = true;
                    masterCodeTextBox.IsReadOnlyCaretVisible = true;
                    break;
                case ControlMode.Delete:
                    break;
                case ControlMode.ReadOnly:
                    break;
            }

            CurrentProduct.Price1 = P;
            gridPrice.DataContext = P;
            grid1.DataContext = CurrentProduct;
        }

        private void masterCodeTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            masterCodeTextBox.SelectAll();
        }

        private void groupBtn_Click(object sender, RoutedEventArgs e)
        {
            var pgd = new ProductGroupDetail();
            bool? res = ZeroMessageBox.Show(pgd, Properties.Resources.NewGroup);
            if (res.HasValue && res.Value)
            {
                Context.Instance.Manager.ProductGroups.AddObject(pgd.ProductGroupNew);
                Context.Instance.Manager.SaveChanges();
                groupBox.Items.Add(pgd.ProductGroupNew);
                groupBox.SelectedItem = pgd.ProductGroupNew;
            }
        }

        private void weightBtn_Click(object sender, RoutedEventArgs e)
        {
            var pgd = new WeightDetail();
            bool? res = ZeroMessageBox.Show(pgd, Properties.Resources.NewMeasurementUnit);
            if (res.HasValue && res.Value)
            {
                Context.Instance.Manager.Weights.AddObject(pgd.WeigthNew);
                Context.Instance.Manager.SaveChanges();
                weightBox.Items.Add(pgd.WeigthNew);
                weightBox.SelectedItem = pgd.WeigthNew;
            }
        }

        public override bool CanAccept(object parameter)
        {
            bool ret = base.CanAccept(parameter);
            string msg = string.Empty;
            if (ret)
            {
                if (ControlMode == ControlMode.New &&
                    Context.Instance.Manager.Products.FirstOrDefault(pr => pr.MasterCode.Equals(CurrentProduct.MasterCode)) != null)
                {
                    msg = "Codigo de product existente!\n Por favor ingrese otro código.";
                    masterCodeTextBox.SelectAll();
                    masterCodeTextBox.Focus();
                }

                if (byWeightCheckBox.IsChecked.HasValue && byWeightCheckBox.IsChecked.Value
                    && weightBox.SelectedIndex < 0)
                {
                    msg += "\nPor favor ingrese una unidad de medida para el producto.";
                }

                if (string.IsNullOrWhiteSpace(valueTextBox.Text))
                {
                    msg += "\nPor favor ingrese un valor de producto.";
                }

                double p = -1;
                if (!double.TryParse(valueTextBox.Text, out p))
                {
                    msg += "\nPor favor ingrese un numero como valor.";
                }

                if (string.IsNullOrWhiteSpace(msg))
                {
                    if (p == 0)
                    {
                        switch (
                            MessageBox.Show("¿Esta seguro que desea dejar el valor en cero?", "Precaución",
                                            MessageBoxButton.YesNo, MessageBoxImage.Question))
                        {
                            case MessageBoxResult.Cancel:
                            case MessageBoxResult.No:
                            case MessageBoxResult.None:
                                valueTextBox.SelectAll();
                                valueTextBox.Focus();
                                ret = false;
                                break;
                            case MessageBoxResult.OK:
                            case MessageBoxResult.Yes:
                                ret = true;
                                break;
                        }
                    }
                }
                else
                {
                    MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    ret = false;
                }

                if (ret)
                {
                    try
                    {
                        switch (ControlMode)
                        {
                            case ControlMode.New:
                                Context.Instance.Manager.Products.AddObject(CurrentProduct);
                                Header = "Producto Nuevo";
                                break;
                            case ControlMode.Update:
                                Context.Instance.Manager.Products.ApplyCurrentValues(CurrentProduct);
                                Header = "Editar Producto";
                                break;
                            case ControlMode.Delete:
                                break;
                            case ControlMode.ReadOnly:
                                break;
                            default:
                                break;
                        }

                        Context.Instance.Manager.SaveChanges();
                    }
                    catch (Exception wx)
                    {
                        System.Windows.Forms.MessageBox.Show(wx.ToString());
                    }

                }
            }


            return ret;
        }

        public override bool CanCancel(object parameter)
        {
            EntityObject obj = CurrentProduct;
            if (obj.EntityState == EntityState.Modified)

                Context.Instance.Manager.Refresh(RefreshMode.StoreWins, CurrentProduct);
            return true;
        }

        private void ClickeableItemButton_Click(object sender, RoutedEventArgs e)
        {
            var t = (int)((Button)sender).DataContext;
            ProductGroup pgroup =
            Context.Instance.Manager.ProductGroups.First(pg => pg.Code == t);
            var pgd = new ProductGroupDetail(pgroup);
            bool? res = ZeroMessageBox.Show(pgd, Properties.Resources.EditGroup);
            if (res.HasValue && res.Value)
            {
                Context.Instance.Manager.SaveChanges();
            }
            else
            {
                Context.Instance.Manager.Refresh(RefreshMode.StoreWins, pgroup);
            }

        }

        private void weightBoxItemButton_Click(object sender, RoutedEventArgs e)
        {
            var t = (int)((Button)sender).DataContext;
            var algo =
            Context.Instance.Manager.Weights.First(w => w.Code == t);
            var pgd = new WeightDetail(algo);
            bool? res = ZeroMessageBox.Show(pgd, Properties.Resources.EditMeasurementUnit);
            if (res.HasValue && res.Value)
            {
                Context.Instance.Manager.SaveChanges();
            }
            else
            {
                Context.Instance.Manager.Refresh(RefreshMode.StoreWins, algo);
            }

        }

        private void weightBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentProduct.Price1 != null)
                CurrentProduct.Price1.SaleWeightCode = ((Weight)weightBox.SelectedItem).Code;
        }

        private void groupBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentProduct.ProductGroup = (ProductGroup)groupBox.SelectedItem;
        }

        private void taxesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentProduct.Tax1Code = ((int)taxesComboBox.SelectedValue);
        }
    }


}
