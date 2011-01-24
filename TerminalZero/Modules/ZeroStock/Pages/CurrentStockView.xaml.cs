using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using ZeroCommonClasses.Interfaces;
using ZeroStock.Entities;

namespace ZeroStock.Pages
{
    /// <summary>
    /// Interaction logic for CurrentStockView.xaml
    /// </summary>
    public partial class CurrentStockView : UserControl
    {
        private readonly ITerminal _terminal;

        public CurrentStockView(ITerminal terminal)
        {
            _terminal = terminal;
            InitializeComponent();
        }

        Entities.StockEntities MyEntities;
        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            // Do not load your data at design time.
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                MyEntities = new Entities.StockEntities();
                if(_terminal.Manager.ValidateRule("ValidateTerminalZero"))
                {
                    terminalFilterContent.Visibility = Visibility.Visible;
                    cbTerminals.ItemsSource = MyEntities.TerminalToes;
                    cbTerminals.SelectedItem = MyEntities.TerminalToes.First(t => t.Code == _terminal.TerminalCode);
                    cbTerminals.SelectionChanged += cbTerminals_SelectionChanged;
                }
                FilterPerTerminal(_terminal.TerminalCode);
            }
        }

        void cvs3_Filter(object sender, FilterEventArgs e)
        {
            
        }

        private void SearchBox_Search(object sender, ZeroGUI.SearchCriteriaEventArgs e)
        {
            CollectionViewSource cvs1 = Resources["cvs1"] as CollectionViewSource;
            cvs1.Source = MyEntities.StockSummaries.Where(s => s.Name.Contains(e.Criteria));
            CollectionViewSource cvs2 = Resources["cvs2"] as CollectionViewSource;
            cvs2.Source = MyEntities.StockCreateSummaries.Where(s => s.Name.Contains(e.Criteria));
            CollectionViewSource cvs3 = Resources["cvs3"] as CollectionViewSource;
            cvs3.Source = MyEntities.StockModifySummaries.Where(s => s.Name.Contains(e.Criteria));
            UpdateLayout();
        }

        private void cbTerminals_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FilterPerTerminal((int)cbTerminals.SelectedValue);
        }

        private void FilterPerTerminal(int tCode)
        {
            CollectionViewSource cvs1 = Resources["cvs1"] as CollectionViewSource;
            cvs1.Source = MyEntities.StockSummaries.Where(s => s.TerminalToCode == tCode);
            CollectionViewSource cvs2 = Resources["cvs2"] as CollectionViewSource;
            cvs2.Source = MyEntities.StockCreateSummaries.Where(s => s.TerminalToCode == tCode);
            CollectionViewSource cvs3 = Resources["cvs3"] as CollectionViewSource;
            cvs3.Source = MyEntities.StockModifySummaries.Where(s => s.TerminalToCode == tCode);
            UpdateLayout();
        }
        
    }
}
