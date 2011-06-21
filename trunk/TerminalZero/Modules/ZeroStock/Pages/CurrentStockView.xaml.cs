using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ZeroBusiness;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses;
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
                if (Terminal.Instance.Session.Rules.IsValid(Rules.IsTerminalZero))
                {
                    terminalFilterContent.Visibility = Visibility.Visible;
                    var expter = BusinessContext.Instance.Model.GetExportTerminal(Terminal.Instance.TerminalCode).ToList();
                    cbTerminals.ItemsSource = expter;
                    cbTerminals.SelectedItem = expter.First(t => t.Code == Terminal.Instance.TerminalCode);
                    cbTerminals.SelectionChanged += cbTerminals_SelectionChanged;
                }
                FilterPerTerminal(Terminal.Instance.TerminalCode);
            }
        }

        private void SearchBox_Search(object sender, SearchCriteriaEventArgs e)
        {
            string newCriteria = e.Criteria ?? "";
            
            int tCode = Terminal.Instance.TerminalCode;
            if (cbTerminals.SelectedValue != null) tCode = ((int)cbTerminals.SelectedValue);
            var cvs1 = Resources["cvs1"] as CollectionViewSource;
            cvs1.Source = BusinessContext.Instance.Model.StockSummaries.Where(s => s.TerminalToCode == tCode && s.Name.Contains(newCriteria));
            var cvs2 = Resources["cvs2"] as CollectionViewSource;
            cvs2.Source = BusinessContext.Instance.Model.StockCreateSummaries.Where(s => s.TerminalToCode == tCode && s.Name.Contains(newCriteria));
            var cvs3 = Resources["cvs3"] as CollectionViewSource;
            cvs3.Source = BusinessContext.Instance.Model.StockModifySummaries.Where(s => s.TerminalToCode == tCode && s.Name.Contains(newCriteria));
        }

        private void cbTerminals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbTerminals.SelectedValue != null) FilterPerTerminal((int)cbTerminals.SelectedValue);
        }

        private void FilterPerTerminal(int tCode)
        {
            var cvs1 = Resources["cvs1"] as CollectionViewSource;
            cvs1.Source = BusinessContext.Instance.Model.StockSummaries.Where(s => s.TerminalToCode == tCode);
            var cvs2 = Resources["cvs2"] as CollectionViewSource;
            cvs2.Source = BusinessContext.Instance.Model.StockCreateSummaries.Where(s => s.TerminalToCode == tCode);
            var cvs3 = Resources["cvs3"] as CollectionViewSource;
            cvs3.Source = BusinessContext.Instance.Model.StockModifySummaries.Where(s => s.TerminalToCode == tCode);
            UpdateLayout();
        }

        
    }
}
