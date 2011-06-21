using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.DataVisualization.Charting;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZeroGUI;
using ZeroSales.Presentation;

namespace ZeroSales.Pages
{
    /// <summary>
    /// Interaction logic for SaleStatistics.xaml
    /// </summary>
    public partial class SaleStatistics : NavigationBasePage
    {
        private SaleStatisticsViewModel TypedViewModel
        {
            get { return (SaleStatisticsViewModel)ViewModel; }
        }

        public SaleStatistics()
        {
            InitializeComponent();
            Loaded += (o, e) =>
                          {
                              TypedViewModel.PropertyChanged += TypedViewModel_PropertyChanged;
                              CommandBar.AppendButton(Properties.Resources.Refresh, TypedViewModel.UpdateCommand);
                              LoadSeries();
                          };
        }

        private void TypedViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals("TotalTerminalSalesCollection"))
            {
                LoadSeries();
            }
        }

        private void LoadSeries()
        {
            chart.Series.Clear();
            foreach (TerminalReport saleReport in TypedViewModel.TotalTerminalSalesCollection)
            {
                var serie = new LineSeries
                                {
                                    Title = saleReport.TerminalName,
                                    ItemsSource = saleReport.Items,
                                    DependentValuePath = "SalesCount",
                                    IndependentValuePath = "Date",
                                };
                chart.Series.Add(serie);
            }
        }
    }
        
}
