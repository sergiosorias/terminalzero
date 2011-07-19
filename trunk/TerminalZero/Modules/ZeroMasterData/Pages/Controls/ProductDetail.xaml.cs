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
using ZeroCommonClasses;
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
            taxesComboBox.ItemsSource = BusinessContext.Instance.Model.Taxes;
        }

        private void LoadPriceWeights()
        {
            foreach (var item in BusinessContext.Instance.Model.Weights)
            {
                weightBox.Items.Add(item);
            }
        }

        private void LoadProductGroup()
        {
            foreach (var item in BusinessContext.Instance.Model.ProductGroups)
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
            var pgd = new ProductGroupDetailViewModel();
            bool? res = ZeroMessageBox.Show(pgd.View, Properties.Resources.NewGroup);
            if (res.HasValue && res.Value)
            {
                groupBox.Items.Add(pgd.CurrentProductGroup);
                ProductDetailViewModel.Product.ProductGroup = pgd.CurrentProductGroup;
            }
        }

        private void weightBtn_Click(object sender, RoutedEventArgs e)
        {
            var pgd = new WeightDetail();
            bool? res = ZeroMessageBox.Show(pgd, Properties.Resources.NewMeasurementUnit);
            if (res.HasValue && res.Value)
            {
                BusinessContext.Instance.Model.Weights.AddObject(pgd.CurrentWeigth);
                BusinessContext.Instance.Model.SaveChanges(SaveOptions.AcceptAllChangesAfterSave, true);
                weightBox.Items.Add(pgd.CurrentWeigth);
                ProductDetailViewModel.Product.Price1.Weight = pgd.CurrentWeigth;
            }
        }

        private void ClickeableItemButton_Click(object sender, RoutedEventArgs e)
        {
            var t = (int)((Button)sender).DataContext;
            ProductGroup pgroup = BusinessContext.Instance.Model.ProductGroups.First(pg => pg.Code == t);
            var pgd = new ProductGroupDetailViewModel(pgroup);
            Terminal.Instance.CurrentClient.ShowDialog(pgd.View, (o) => { if(o)ProductDetailViewModel.Product.ProductGroup = pgd.CurrentProductGroup; });
        }

        private void weightBoxItemButton_Click(object sender, RoutedEventArgs e)
        {
            var t = (int)((Button)sender).DataContext;
            var algo =
            BusinessContext.Instance.Model.Weights.First(w => w.Code == t);
            var pgd = new WeightDetail(algo);
            bool? res = ZeroMessageBox.Show(pgd, Properties.Resources.EditMeasurementUnit);
            if (res.HasValue && res.Value)
            {
                BusinessContext.Instance.Model.SaveChanges(SaveOptions.AcceptAllChangesAfterSave, true);
            }
            else
            {
                BusinessContext.Instance.Model.Refresh(RefreshMode.StoreWins, algo);
            }

        }
       
    }


}
