using System;
using System.ComponentModel;
using System.Data;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ZeroMasterData.Presentation;

namespace ZeroMasterData.Pages.Controls
{
    /// <summary>
    /// Interaction logic for ProductDetail.xaml
    /// </summary>
    public partial class ProductDetail : NavigationBasePage
    {
        public ProductDetailViewModel ProductDetailViewModel
        {
            get { return (ProductDetailViewModel)ViewModel; }
        }

        public ProductDetail()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsInDesignMode)
            {
                LoadProduct();
                LoadTaxes();
                LoadPriceWeights();
                LoadProductGroup();
            }
        }

        private void LoadTaxes()
        {
            taxesComboBox.ItemsSource = BusinessContext.Instance.ModelManager.Taxes;
        }

        private void LoadPriceWeights()
        {
            foreach (var item in BusinessContext.Instance.ModelManager.Weights)
            {
                weightBox.Items.Add(item);
            }
        }

        private void LoadProductGroup()
        {
            foreach (var item in BusinessContext.Instance.ModelManager.ProductGroups)
            {
                groupBox.Items.Add(item);
            }
        }

        private void LoadProduct()
        {
            if(ControlMode == ControlMode.Update)
            {
                masterCodeTextBox.IsReadOnly = true;
                masterCodeTextBox.IsReadOnlyCaretVisible = true;
            }
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
                BusinessContext.Instance.ModelManager.ProductGroups.AddObject(pgd.ProductGroupNew);
                BusinessContext.Instance.ModelManager.SaveChanges();
                groupBox.Items.Add(pgd.ProductGroupNew);
                ProductDetailViewModel.Product.ProductGroup = pgd.ProductGroupNew;
            }
        }

        private void weightBtn_Click(object sender, RoutedEventArgs e)
        {
            var pgd = new WeightDetail();
            bool? res = ZeroMessageBox.Show(pgd, Properties.Resources.NewMeasurementUnit);
            if (res.HasValue && res.Value)
            {
                BusinessContext.Instance.ModelManager.Weights.AddObject(pgd.CurrentWeigth);
                BusinessContext.Instance.ModelManager.SaveChanges();
                weightBox.Items.Add(pgd.CurrentWeigth);
                ProductDetailViewModel.Product.Price1.Weight = pgd.CurrentWeigth;
            }
        }

        private void ClickeableItemButton_Click(object sender, RoutedEventArgs e)
        {
            var t = (int)((Button)sender).DataContext;
            ProductGroup pgroup =
            BusinessContext.Instance.ModelManager.ProductGroups.First(pg => pg.Code == t);
            var pgd = new ProductGroupDetail(pgroup);
            bool? res = ZeroMessageBox.Show(pgd, Properties.Resources.EditGroup);
            if (res.HasValue && res.Value)
            {
                BusinessContext.Instance.ModelManager.SaveChanges();
            }
            else
            {
                BusinessContext.Instance.ModelManager.Refresh(RefreshMode.StoreWins, pgroup);
            }

        }

        private void weightBoxItemButton_Click(object sender, RoutedEventArgs e)
        {
            var t = (int)((Button)sender).DataContext;
            var algo =
            BusinessContext.Instance.ModelManager.Weights.First(w => w.Code == t);
            var pgd = new WeightDetail(algo);
            bool? res = ZeroMessageBox.Show(pgd, Properties.Resources.EditMeasurementUnit);
            if (res.HasValue && res.Value)
            {
                BusinessContext.Instance.ModelManager.SaveChanges();
            }
            else
            {
                BusinessContext.Instance.ModelManager.Refresh(RefreshMode.StoreWins, algo);
            }

        }
       
    }


}
