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

        private System.Timers.Timer searchTimer = null;
        private System.Timers.Timer cleanResTimer = null;

        public SearchBox()
        {
            InitializeComponent();
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
                        quantity.Content = string.Format("{0} encontrados", search.Matches);
                    }
                    else
                    {
                        quantity.Content = string.Format("No hay resultados", search.Matches);
                    }
                    cleanResTimer.Start();
                }
            }
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {
            Dispatcher.BeginInvoke(new System.Windows.Forms.MethodInvoker(OnSearch),null);
        }
        
        private void txtSearchCriteria_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                btnSearch_Click(null, null);
            }
            else if (txtSearchCriteria.Text.Length > 2 || txtSearchCriteria.Text.Length == 0)
            {
                searchTimer.Stop();
                searchTimer.Start();
            }
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            searchTimer = new System.Timers.Timer(200);
            cleanResTimer = new System.Timers.Timer(5000);
            searchTimer.Elapsed += new System.Timers.ElapsedEventHandler(searchTimer_Elapsed);
            cleanResTimer.Elapsed += new System.Timers.ElapsedEventHandler(cleanResTimer_Elapsed);
        }

        void cleanResTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            cleanResTimer.Stop();
            Dispatcher.Invoke(new System.Windows.Forms.MethodInvoker(
                () => { quantity.Content = "";}
                ), null);
            
        }

        void searchTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            searchTimer.Stop();
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
