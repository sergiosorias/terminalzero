using System;
using System.ComponentModel;
using System.Data;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ZeroMasterData.Entities;

namespace ZeroMasterData.Pages.Controls
{
    /// <summary>
    /// Interaction logic for ProductDetail.xaml
    /// </summary>
    public partial class ProductDetail : ZeroBasePage
    {
        public ProductDetail(MasterDataEntities dataProvider)
        {
            ControlMode = ControlMode.New;
            InitializeComponent();
            DataProvider = dataProvider;
        }

        public ProductDetail(MasterDataEntities dataProvider, int productCode)
            : this(dataProvider)
        {
            CurrentProduct = DataProvider.Products.First(p => p.Code == productCode);
            ControlMode = ControlMode.Update;
        }

        MasterDataEntities DataProvider;
        public Product CurrentProduct { get; private set; }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                LoadProduct();
                LoadTaxes();
                LoadPriceWeights();
                LoadProductGroup();
            }
        }

        private void LoadTaxes()
        {
            taxesComboBox.ItemsSource = DataProvider.Taxes;
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
            foreach (var item in DataProvider.Weights)
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
            foreach (var item in DataProvider.ProductGroups)
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
                        DataProvider.Products.Count()
                        , true, DataProvider.GetNextProductCode(), true);

                    P = Price.CreatePrice(
                        DataProvider.Prices.Count(),
                        true, 0);
                    break;
                case ControlMode.Update:
                    if (CurrentProduct.PriceCode.HasValue)
                        P = DataProvider.Prices.First(p => p.Code == CurrentProduct.PriceCode);
                    else
                    {
                        P = Price.CreatePrice(
                        DataProvider.Prices.Count(),
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
            var pgd = new ProductGroupDetail(DataProvider);
            bool? res = ZeroMessageBox.Show(pgd, Properties.Resources.NewGroup);
            if (res.HasValue && res.Value)
            {
                DataProvider.ProductGroups.AddObject(pgd.ProductGroupNew);
                DataProvider.SaveChanges();
                groupBox.Items.Add(pgd.ProductGroupNew);
                groupBox.SelectedItem = pgd.ProductGroupNew;
            }
        }

        private void weightBtn_Click(object sender, RoutedEventArgs e)
        {
            var pgd = new WeightDetail(DataProvider);
            bool? res = ZeroMessageBox.Show(pgd, Properties.Resources.NewMeasurementUnit);
            if (res.HasValue && res.Value)
            {
                DataProvider.Weights.AddObject(pgd.WeigthNew);
                DataProvider.SaveChanges();
                weightBox.Items.Add(pgd.WeigthNew);
                weightBox.SelectedItem = pgd.WeigthNew;
            }
        }

        public override bool CanAccept(object parameter)
        {
            bool ret = true; ;
            string msg = string.Empty;
            if (CurrentProduct.MasterCode == 0)
            {
                msg = "Por favor ingrese un código de producto.";
                masterCodeTextBox.SelectAll();
                masterCodeTextBox.Focus();
            }
            else if(ControlMode == ControlMode.New && DataProvider.Products.FirstOrDefault(pr=>pr.MasterCode.Equals(CurrentProduct.MasterCode))!=null)
            {
                msg = "Codigo de product existente!\n Por favor ingrese otro código.";
                masterCodeTextBox.SelectAll();
                masterCodeTextBox.Focus();
            }

            if (groupBox.SelectedIndex < 0)
            {
                msg += "\nPor favor ingrese un grupo para este product.";
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
                    switch (MessageBox.Show("¿Esta seguro que desea dejar el valor en cero?", "Precaución", MessageBoxButton.YesNo, MessageBoxImage.Question))
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
                            DataProvider.Products.AddObject(CurrentProduct);
                            break;
                        case ControlMode.Update:
                            DataProvider.Products.ApplyCurrentValues(CurrentProduct);
                            break;
                        case ControlMode.Delete:
                            break;
                        case ControlMode.ReadOnly:
                            break;
                        default:
                            break;
                    }

                    DataProvider.SaveChanges();
                }
                catch (Exception wx)
                {
                    System.Windows.Forms.MessageBox.Show(wx.ToString());
                }

            }


            return ret;
        }

        public override bool CanCancel(object parameter)
        {
            EntityObject obj = CurrentProduct;
            if (obj.EntityState == EntityState.Modified)
                DataProvider.Refresh(RefreshMode.StoreWins, CurrentProduct);
            return true;
        }

        private void ClickeableItemButton_Click(object sender, RoutedEventArgs e)
        {
            var t = (int)((Button)sender).DataContext;
            ProductGroup pgroup = DataProvider.ProductGroups.First(pg => pg.Code == t);
            var pgd = new ProductGroupDetail(DataProvider, pgroup);
            bool? res = ZeroMessageBox.Show(pgd, Properties.Resources.EditGroup);
            if (res.HasValue && res.Value)
            {
                DataProvider.SaveChanges();
            }
            else
            {
                DataProvider.Refresh(RefreshMode.StoreWins, pgroup);
            }

        }

        private void weightBoxItemButton_Click(object sender, RoutedEventArgs e)
        {
            var t = (int)((Button)sender).DataContext;
            var algo = DataProvider.Weights.First(w => w.Code == t);
            var pgd = new WeightDetail(DataProvider,
                algo);
            bool? res = ZeroMessageBox.Show(pgd, Properties.Resources.EditMeasurementUnit);
            if (res.HasValue && res.Value)
            {
                DataProvider.SaveChanges();
            }
            else
            {
                DataProvider.Refresh(RefreshMode.StoreWins, algo);
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
