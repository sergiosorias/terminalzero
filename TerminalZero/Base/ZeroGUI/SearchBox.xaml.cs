using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ZeroGUI
{
    /// <summary>
    /// Interaction logic for SearchBox.xaml
    /// </summary>
    public partial class SearchBox : UserControl
    {
        public event EventHandler<SearchCriteriaEventArgs> Search;
        
        public bool ShowResultCount
        {
            get { return (bool)GetValue(ShowResultCountProperty); }
            set { SetValue(ShowResultCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowResultCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowResultCountProperty =
            DependencyProperty.Register("ShowResultCount", typeof(bool), typeof(SearchBox), null);
        
        public int MinCriteriaCharCount
        {
            get { return (int)GetValue(MinCriteriaCharCountProperty); }
            set { SetValue(MinCriteriaCharCountProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MinCriteriaCharCount.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MinCriteriaCharCountProperty =
            DependencyProperty.Register("MinCriteriaCharCount", typeof(int), typeof(SearchBox), new PropertyMetadata(3));

        
        public ICommand SearchCommand
        {
            get { return (ICommand)GetValue(SearchCommandProperty); }
            set { SetValue(SearchCommandProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SearchCommand.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SearchCommandProperty =
            DependencyProperty.Register("SearchCommand", typeof(ICommand), typeof(SearchBox), null);
        

        private Timer searchTimer;
        private Timer cleanResTimer;

        public SearchBox()
        {
            InitializeComponent();
            cleanResTimer = new Timer(cleanResTimer_Elapsed, null, 5000, 5000);
            searchTimer = new Timer(searchTimer_Elapsed, null, Timeout.Infinite, Timeout.Infinite);
           
        }

        protected void OnSearch()
        {
            var searchCriteriaEventArgs = new SearchCriteriaEventArgs(string.IsNullOrEmpty(txtSearchCriteria.Text) ? null : txtSearchCriteria.Text);
            if (Search != null)
            {
                Search(this, searchCriteriaEventArgs);
                
            }
            if(SearchCommand!=null)
            {
                if (SearchCommand.CanExecute(null))
                {
                    SearchCommand.Execute(searchCriteriaEventArgs);
                }
            }

            if (ShowResultCount)
            {
                if (searchCriteriaEventArgs.Matches > 0)
                {
                    quantity.Text = string.Format("{0} encontrados", searchCriteriaEventArgs.Matches);
                }
                else
                {
                    quantity.Text = string.Format("No hay resultados");
                }
                quantityPopup.IsOpen = true;
                CreateResTimer();
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new Update(OnSearch),null);
        }
        
        private void txtSearchCriteria_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnSearch_Click(null, null);
            }
            else if (e.Key != Key.Tab && (txtSearchCriteria.Text.Length >= MinCriteriaCharCount || txtSearchCriteria.Text.Length == 0))
            {
                searchTimer.Change(Timeout.Infinite, Timeout.Infinite);
                CreateSearchTimer();
            }
        }

        private void CreateResTimer()
        {
            cleanResTimer.Change(5000,Timeout.Infinite);
        }

        private void CreateSearchTimer()
        {
            searchTimer.Change(50, Timeout.Infinite);
        }

        void cleanResTimer_Elapsed(object o)
        {
            Dispatcher.BeginInvoke(new Update(
                () => { quantityPopup.IsOpen = false; quantity.Text = ""; }
                ), null);
            
        }

        void searchTimer_Elapsed(object o)
        {
            btnSearch_Click(null, null);
        }
    }

    public class SearchCriteriaEventArgs : EventArgs
    {
        public SearchCriteriaEventArgs(string criteria)
        {
            Criteria = criteria;
        }

        public string Criteria { get; private set; }
        public bool Cancel { get; set; }
        public int Matches { get; set; }
                
    }
}
