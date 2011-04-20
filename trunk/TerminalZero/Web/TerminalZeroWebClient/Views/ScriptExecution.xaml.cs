using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using TerminalZeroWebClient.Classes;

namespace TerminalZeroWebClient.Views
{
    public partial class ScriptExecution : Page
    {
        private DataService.Entities _entities;
        
        public ScriptExecution()
        {
            InitializeComponent();
            if (App.Current.IsRunningOutOfBrowser)
                urlOutOfBrowserContent.Visibility = Visibility.Visible;
        }
        
        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Uri uri = null;
            if (!App.Current.IsRunningOutOfBrowser)
            {
                uri = new Uri(Application.Current.Host.Source, "../Services/DatabaseDataService.svc");
                uriContent.Text = uri.ToString();
                _entities = new DataService.Entities(uri);
                ConfigureEntities();
            }
        }

        private void ConfigureEntities()
        {
            List<object> entitiesAllowed = new List<object>
            {
                new DataServiceEntity<DataService.Weight>("Weights", "Cantidades",_entities),
                new DataServiceEntity<DataService.Customer>("Customers", "Clientes",_entities),
                new DataServiceEntity<DataService.ProductGroup>("ProductGroups","Grupos", _entities),
                new DataServiceEntity<DataService.Tax>("Taxes", "Impuestos",_entities),
                new DataServiceEntity<DataService.Price>("Prices", "Precios",_entities),
                new DataServiceEntity<DataService.Supplier>("Suppliers","Proveedores", _entities),
                new DataServiceEntity<DataService.Product>("Products", "Productos",_entities),
                new DataServiceEntity<DataService.TaxPosition>("TaxPositions","Posicion IVA",_entities)
            };

            foreach (var item in entitiesAllowed)
            {
                var ent = item as IQueryableEntity;
                if (ent != null)
                {
                    ent.LoadCompleted += ent_LoadCompleted;
                }
            }
            entitiesGrid.ItemsSource = entitiesAllowed;            
            entitiesGrid.UpdateLayout();
        }

        private void ent_LoadCompleted(object sender, LoadCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                MessageBox.Show(e.Error.ToString(), "Error", MessageBoxButton.OK);
            }
            else
            {
                var pcv = new System.Windows.Data.PagedCollectionView(((IQueryableEntity) sender).Collection);
                dataGrid1.ItemsSource = pcv;
                dataGrid1.UpdateLayout();
            }
            waitCursor.Stop();
        }
        
        private void entitiesGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IQueryableEntity ent = entitiesGrid.SelectedItem as IQueryableEntity;
            if (ent != null)
            {
                waitCursor.Start();
                ent.LoadAsync();
            }
        }

        private void SearchBox_Search(object sender, ZeroGUI.SearchCriteriaEventArgs e)
        {
            var pcv = dataGrid1.ItemsSource as System.Windows.Data.PagedCollectionView;
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
                                ret = obj.ToString().ToUpper().Contains(e.Criteria.ToUpper());
                                if (ret)
                                    break;
                            }
                        }
                    } 
                    return ret; 
                });
            }

            e.Matches = pcv.ItemCount;
        }

        private void btnLoadContext_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Uri uri = new Uri("http://"+URLOutOfBrowser.Text + "/Services/DatabaseDataService.svc");
                uriContent.Text = uri.ToString();
                _entities = new DataService.Entities(uri);
                ConfigureEntities();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(),"Error",MessageBoxButton.OK);
            }
            
        }
                

    }
}
