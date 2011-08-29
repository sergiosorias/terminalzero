using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;

namespace TerminalZeroRiaWebClient.Views
{
    public partial class DataViewer : Page
    {
        public DataViewer()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {

        }

        private void SearchBox_Search(object sender, ZeroGUI.SearchCriteriaEventArgs e)
        {
            var pcv = dataGrid1.ItemsSource as System.Windows.Data.PagedCollectionView;
            string criteria = string.IsNullOrWhiteSpace(e.Criteria) ? "": e.Criteria.ToUpper();
            
            if (pcv != null)
            {
                pcv.Filter = new Predicate<object>(I =>
                {
                    bool ret = false;
                    Type TT = I.GetType();
                    foreach (var item in TT.GetProperties())
                    {
                        if (item.GetType() != typeof(DateTime) && item.CanRead)
                        {
                            object obj = item.GetValue(I, null);
                            if (obj != null)
                            {
                                ret = obj.ToString().IndexOf(criteria,0,StringComparison.InvariantCultureIgnoreCase)>=0;
                                if (ret)
                                    break;
                            }
                        }
                    }
                    return ret;
                });
                e.Matches = pcv.ItemCount;
            }
        }

    }
}
