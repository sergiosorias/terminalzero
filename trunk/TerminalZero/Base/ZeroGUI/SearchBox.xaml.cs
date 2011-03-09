using System;
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
            DependencyProperty.Register("MinCriteriaCharCount", typeof(int), typeof(SearchBox), new PropertyMetadata(0));

        private System.Threading.Timer searchTimer = null;
        private System.Threading.Timer cleanResTimer = null;

        public SearchBox()
        {
            InitializeComponent();
            MinCriteriaCharCount = 3;
        }

        protected void OnSearch()
        {
            if (Search != null)
            {
                SearchCriteriaEventArgs search = new SearchCriteriaEventArgs(txtSearchCriteria.Text);
                Search(this, search);
                
                if (ShowResultCount)
                {
                    if (search.Matches > 0)
                    {
                        quantity.Text = string.Format("{0} encontrados", search.Matches);
                    }
                    else
                    {
                        quantity.Text = string.Format("No hay resultados");
                    }
                    quantityPopup.IsOpen = true;
                    CreateResTimer();
                }
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
                if(searchTimer!=null)
                    searchTimer.Dispose();
                CreateSearchTimer();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            
        }

        private void CreateResTimer()
        {
            cleanResTimer = new System.Threading.Timer(cleanResTimer_Elapsed, null, 5000, 5000);
        }

        private void CreateSearchTimer()
        {
            searchTimer = new System.Threading.Timer(searchTimer_Elapsed, null, 10, 200);
        }

        void cleanResTimer_Elapsed(object o)
        {
            cleanResTimer.Dispose();
            Dispatcher.BeginInvoke(new Update(
                () => { quantityPopup.IsOpen = false; quantity.Text = ""; }
                ), null);
            
        }

        void searchTimer_Elapsed(object o)
        {
            searchTimer.Dispose();
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
