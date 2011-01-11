using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ZeroStock.Pages
{
    /// <summary>
    /// Interaction logic for CurrentStockView.xaml
    /// </summary>
    public partial class CurrentStockView : UserControl
    {
        public CurrentStockView()
        {
            InitializeComponent();
        }
        Entities.StockEntities MyEntities;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                MyEntities = new Entities.StockEntities();
                stockSummariesDataGrid.ItemsSource = MyEntities.StockSummaries;
                stockCreateSummariesDataGrid.ItemsSource = MyEntities.StockCreateSummaries;
                stockModifySummariesDataGrid.ItemsSource = MyEntities.StockModifySummaries;
            }
        }

        private void SearchBox_Search(object sender, ZeroGUI.SearchCriteriaEventArgs e)
        {
            stockSummariesDataGrid.ItemsSource = MyEntities.StockSummaries.Where(s => s.Name.Contains(e.Criteria));
            stockCreateSummariesDataGrid.ItemsSource = MyEntities.StockCreateSummaries.Where(s => s.Name.Contains(e.Criteria));
            
        }
    }
}
