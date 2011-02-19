using System;
using System.Collections.Generic;
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
            
        }
        
        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Uri uri = new Uri(Application.Current.Host.Source, "../Services/DatabaseDataService.svc");
            uriContent.Text = uri.ToString();
            _entities = new DataService.Entities(uri);
            ConfigureEntities();
        }

        private void ConfigureEntities()
        {
            List<object> entitiesAllowed = new List<object>();
            entitiesAllowed.Add(new DataServiceEntity<DataService.Weight>("Weights", "Cantidades", _entities));
            entitiesAllowed.Add(new DataServiceEntity<DataService.Customer> ("Customers", "Clientes" ,_entities));
            entitiesAllowed.Add(new DataServiceEntity<DataService.ProductGroup>("ProductGroups", "Grupos", _entities));
            entitiesAllowed.Add(new DataServiceEntity<DataService.Tax>("Taxes", "Impuestos", _entities));
            entitiesAllowed.Add(new DataServiceEntity<DataService.Weight>("Prices", "Precios", _entities));
            entitiesAllowed.Add(new DataServiceEntity<DataService.Supplier>("Suppliers", "Proveedores", _entities));
            entitiesAllowed.Add(new DataServiceEntity<DataService.Product>("Products", "Productos", _entities));
            entitiesAllowed.Add(new DataServiceEntity<DataService.TaxPosition>("TaxPositions", "Posicion IVA", _entities));
                                    
            foreach (var item in entitiesAllowed)
            {
                IQueryableEntity ent = item as IQueryableEntity;
                if (ent != null)
                {
                    ent.LoadCompleted += new EventHandler(ent_LoadCompleted);
                }
            }
            entitiesGrid.ItemsSource = entitiesAllowed;            
            entitiesGrid.UpdateLayout();
        }

        private void ent_LoadCompleted(object sender, EventArgs e)
        {
            var pcv = new System.Windows.Data.PagedCollectionView(((IQueryableEntity)sender).Collection);
            dataGrid1.ItemsSource = pcv;
            dataGrid1.UpdateLayout();
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
                

    }
}
