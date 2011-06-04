using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ZeroBusiness;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;

namespace ZeroStock.Pages
{
    /// <summary>
    /// Interaction logic for CurrentStockView.xaml
    /// </summary>
    public partial class CurrentStockView : NavigationBasePage
    {
        public CurrentStockView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            // Do not load your data at design time.
            if (!IsInDesignMode)
            {
                if (ZeroCommonClasses.Terminal.Instance.Session.Rules.IsValid(Rules.IsTerminalZero))
                {
                    terminalFilterContent.Visibility = Visibility.Visible;
                    var expter = BusinessContext.Instance.ModelManager.GetExportTerminal(ZeroCommonClasses.Terminal.Instance.TerminalCode).ToList();
                    cbTerminals.ItemsSource = expter;
                    cbTerminals.SelectedItem = expter.First(t => t.Code == ZeroCommonClasses.Terminal.Instance.TerminalCode);
                    cbTerminals.SelectionChanged += cbTerminals_SelectionChanged;
                }
                FilterPerTerminal(ZeroCommonClasses.Terminal.Instance.TerminalCode);
            }
        }

        private void SearchBox_Search(object sender, SearchCriteriaEventArgs e)
        {
            int tCode = ZeroCommonClasses.Terminal.Instance.TerminalCode;
            if (cbTerminals.SelectedValue != null) tCode = ((int)cbTerminals.SelectedValue);
            var cvs1 = Resources["cvs1"] as CollectionViewSource;
            cvs1.Source = BusinessContext.Instance.ModelManager.StockSummaries.Where(s => s.TerminalToCode == tCode && s.Name.Contains(e.Criteria));
            var cvs2 = Resources["cvs2"] as CollectionViewSource;
            cvs2.Source = BusinessContext.Instance.ModelManager.StockCreateSummaries.Where(s => s.TerminalToCode == tCode && s.Name.Contains(e.Criteria));
            var cvs3 = Resources["cvs3"] as CollectionViewSource;
            cvs3.Source = BusinessContext.Instance.ModelManager.StockModifySummaries.Where(s => s.TerminalToCode == tCode && s.Name.Contains(e.Criteria));
        }

        private void cbTerminals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbTerminals.SelectedValue != null) FilterPerTerminal((int)cbTerminals.SelectedValue);
        }

        private void FilterPerTerminal(int tCode)
        {
            var cvs1 = Resources["cvs1"] as CollectionViewSource;
            cvs1.Source = BusinessContext.Instance.ModelManager.StockSummaries.Where(s => s.TerminalToCode == tCode);
            var cvs2 = Resources["cvs2"] as CollectionViewSource;
            cvs2.Source = BusinessContext.Instance.ModelManager.StockCreateSummaries.Where(s => s.TerminalToCode == tCode);
            var cvs3 = Resources["cvs3"] as CollectionViewSource;
            cvs3.Source = BusinessContext.Instance.ModelManager.StockModifySummaries.Where(s => s.TerminalToCode == tCode);
            UpdateLayout();
        }

        
    }
}
