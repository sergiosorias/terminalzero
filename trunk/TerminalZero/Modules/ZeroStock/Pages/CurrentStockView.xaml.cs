using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ZeroBusiness;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ZeroStock.Entities;

namespace ZeroStock.Pages
{
    /// <summary>
    /// Interaction logic for CurrentStockView.xaml
    /// </summary>
    public partial class CurrentStockView : NavigationBasePage
    {
        private readonly ITerminal _terminal;

        public CurrentStockView(ITerminal terminal)
        {
            _terminal = terminal;
            InitializeComponent();
        }

        StockEntities MyEntities;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            // Do not load your data at design time.
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                MyEntities = new StockEntities();
                if (_terminal.Manager.ValidateRule(Rules.IsTerminalZero))
                {
                    terminalFilterContent.Visibility = Visibility.Visible;
                    var expter = MyEntities.GetExportTerminal(_terminal.TerminalCode).ToList();
                    cbTerminals.ItemsSource = expter;
                    cbTerminals.SelectedItem = expter.First(t => t.Code == _terminal.TerminalCode);
                    cbTerminals.SelectionChanged += cbTerminals_SelectionChanged;
                }
                FilterPerTerminal(_terminal.TerminalCode);
            }
        }

        private void SearchBox_Search(object sender, SearchCriteriaEventArgs e)
        {
            int tCode = _terminal.TerminalCode;
            if (cbTerminals.SelectedValue != null) tCode = ((int)cbTerminals.SelectedValue);
            var cvs1 = Resources["cvs1"] as CollectionViewSource;
            cvs1.Source = MyEntities.StockSummaries.Where(s => s.TerminalToCode == tCode && s.Name.Contains(e.Criteria));
            var cvs2 = Resources["cvs2"] as CollectionViewSource;
            cvs2.Source = MyEntities.StockCreateSummaries.Where(s => s.TerminalToCode == tCode && s.Name.Contains(e.Criteria));
            var cvs3 = Resources["cvs3"] as CollectionViewSource;
            cvs3.Source = MyEntities.StockModifySummaries.Where(s => s.TerminalToCode == tCode && s.Name.Contains(e.Criteria));
        }

        private void cbTerminals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cbTerminals.SelectedValue != null) FilterPerTerminal((int)cbTerminals.SelectedValue);
        }

        private void FilterPerTerminal(int tCode)
        {
            var cvs1 = Resources["cvs1"] as CollectionViewSource;
            cvs1.Source = MyEntities.StockSummaries.Where(s => s.TerminalToCode == tCode);
            var cvs2 = Resources["cvs2"] as CollectionViewSource;
            cvs2.Source = MyEntities.StockCreateSummaries.Where(s => s.TerminalToCode == tCode);
            var cvs3 = Resources["cvs3"] as CollectionViewSource;
            cvs3.Source = MyEntities.StockModifySummaries.Where(s => s.TerminalToCode == tCode);
            UpdateLayout();
        }

        
    }
}
