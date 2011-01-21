using System;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;

namespace ZeroMasterData.Pages.Controls
{
    /// <summary>
    /// Interaction logic for ProductDetail.xaml
    /// </summary>
    public partial class ProductDetail : UserControl, IZeroPage
    {
        public ProductDetail(Entities.MasterDataEntities dataProvider)
        {
            InitializeComponent();
            DataProvider = dataProvider;
        }

        public ProductDetail(Entities.MasterDataEntities dataProvider, int productCode)
            : this(dataProvider)
        {
            CurrentProduct = DataProvider.Products.First(p => p.Code == productCode);
            Mode = ZeroCommonClasses.Interfaces.Mode.Update;
        }

        Entities.MasterDataEntities DataProvider;
        public Entities.Product CurrentProduct { get; private set; }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
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
            switch (Mode)
            {
                case Mode.New:
                    taxesComboBox.SelectedIndex = 1;
                    break;
                case Mode.Update:
                    if (!CurrentProduct.TaxReference.IsLoaded)
                        CurrentProduct.TaxReference.Load();
                    taxesComboBox.SelectedItem = CurrentProduct.Tax;
                    break;
                case Mode.Delete:
                    break;
                case Mode.ReadOnly:
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

            if (Mode == ZeroCommonClasses.Interfaces.Mode.Update
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
                if (Mode == ZeroCommonClasses.Interfaces.Mode.Update && CurrentProduct.Group1 == item.Code)
                {
                    groupBox.SelectedItem = item;
                }
            }
        }

        private void LoadProduct()
        {
            Entities.Price P = null;

            switch (Mode)
            {
                case Mode.New:
                    CurrentProduct = Entities.Product.CreateProduct(
                        DataProvider.Products.Count()
                        , true, DataProvider.GetNextProductCode(), true);

                    P = Entities.Price.CreatePrice(
                        DataProvider.Prices.Count(),
                        true, 0);
                    break;
                case Mode.Update:
                    if (CurrentProduct.PriceCode.HasValue)
                        P = DataProvider.Prices.First(p => p.Code == CurrentProduct.PriceCode);
                    else
                    {
                        P = Entities.Price.CreatePrice(
                        DataProvider.Prices.Count(),
                        true, 0);
                    }
                    masterCodeTextBox.IsReadOnly = true;
                    masterCodeTextBox.IsReadOnlyCaretVisible = true;
                    break;
                case Mode.Delete:
                    break;
                case Mode.ReadOnly:
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
            ProductGroupDetail pgd = new ProductGroupDetail(DataProvider);
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
            WeightDetail pgd = new WeightDetail(DataProvider);
            bool? res = ZeroMessageBox.Show(pgd, Properties.Resources.NewMeasurementUnit);
            if (res.HasValue && res.Value)
            {
                DataProvider.Weights.AddObject(pgd.WeigthNew);
                DataProvider.SaveChanges();
                weightBox.Items.Add(pgd.WeigthNew);
                weightBox.SelectedItem = pgd.WeigthNew;
            }
        }

        #region IEntitiyValidate Members

        public bool CanAccept(object parameter)
        {
            bool ret = true; ;
            string msg = string.Empty;
            if (CurrentProduct.MasterCode == 0)
            {
                msg = "Por favor ingrese un código de producto.";
                masterCodeTextBox.SelectAll();
                masterCodeTextBox.Focus();
            }
            else if(DataProvider.Products.FirstOrDefault(pr=>pr.MasterCode.Equals(CurrentProduct.MasterCode))!=null)
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
                    switch (System.Windows.MessageBox.Show("¿Esta seguro que desea dejar el valor en cero?", "Precaución", MessageBoxButton.YesNo, MessageBoxImage.Question))
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
                System.Windows.MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                ret = false;
            }

            if (ret)
            {
                try
                {
                    switch (Mode)
                    {
                        case Mode.New:
                            DataProvider.Products.AddObject(CurrentProduct);
                            break;
                        case Mode.Update:
                            DataProvider.Products.ApplyCurrentValues(CurrentProduct);
                            break;
                        case Mode.Delete:
                            break;
                        case Mode.ReadOnly:
                            break;
                        default:
                            break;
                    }

                    DataProvider.SaveChanges();
                }
                catch (Exception wx)
                {
                    global::System.Windows.Forms.MessageBox.Show(wx.ToString());
                }

            }


            return ret;
        }

        public bool CanCancel(object parameter)
        {
            EntityObject obj = CurrentProduct as EntityObject;
            if (obj.EntityState == System.Data.EntityState.Modified)
                DataProvider.Refresh(System.Data.Objects.RefreshMode.StoreWins, CurrentProduct);
            return true;
        }

        private Mode _Mode = Mode.New;

        public Mode Mode
        {
            get
            {
                return _Mode;
            }
            set
            {
                _Mode = value;
            }
        }

        #endregion

        private void ClickeableItemButton_Click(object sender, RoutedEventArgs e)
        {
            int t = (int)((Button)sender).DataContext;
            ProductGroupDetail pgd = new ProductGroupDetail(DataProvider);
            bool? res = ZeroMessageBox.Show(pgd, Properties.Resources.EditGroup);
            if (res.HasValue && res.Value)
            {
                DataProvider.SaveChanges();
            }

        }

        private void weightBoxItemButton_Click(object sender, RoutedEventArgs e)
        {
            int t = (int)((Button)sender).DataContext;
            WeightDetail pgd = new WeightDetail(DataProvider,
                DataProvider.Weights.First(w => w.Code == t));
            bool? res = ZeroMessageBox.Show(pgd, Properties.Resources.EditMeasurementUnit);
            if (res.HasValue && res.Value)
            {
                DataProvider.SaveChanges();
            }

        }

        private void weightBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CurrentProduct.Price1 != null)
                CurrentProduct.Price1.SaleWeightCode = ((Entities.Weight)weightBox.SelectedItem).Code;
        }

        private void groupBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentProduct.ProductGroup = (Entities.ProductGroup)groupBox.SelectedItem;
        }

        private void taxesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentProduct.Tax1Code = ((int)taxesComboBox.SelectedValue);
        }
    }


}
