using System;
using System.Collections.ObjectModel;
using System.Net;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using SLFramework.Services;
using SLFramework.ViewModel;
using TerminalZeroRiaWebClient.Web.Services;
using ZeroBusiness.Entities.Data;
using System.Linq;

namespace TerminalZeroRiaWebClient.ViewModels
{
    public class DataViewerViewModel : ViewModel
    {
        public interface IQueryableTable
        {
            EntityQuery Query { get; }
            LoadOperation LoadOperation { get; }
        }

        public class QueryableTableTemplate<T> : IQueryableTable 
            where T : Entity
        {
            public string Name { get; private set; }
            public EntityQuery<T> TableQuery { get; private set; }
            private DomainContext context;

            public QueryableTableTemplate(string name, EntityQuery<T> query, DomainContext context)
            {
                Name = name;
                TableQuery = query;
                this.context = context;
            }

            public EntityQuery Query
            {
                get { return TableQuery; }
            }

            public LoadOperation LoadOperation
            {
                get { return context.Load<T>(TableQuery); }
            }
        }

        private TerminalZeroDataDomainContext context;
        #region Properties

        private PagedCollectionView itemCollection;

        public PagedCollectionView ItemCollection
        {
            get { return itemCollection; }
            set
            {
                if (itemCollection != value)
                {
                    itemCollection = value;
                    OnPropertyChanged("ItemCollection");
                }
            }
        }

        private IQueryableTable selectedTable;

        public IQueryableTable SelectedTable
        {
            get { return selectedTable; }
            set
            {
                if (selectedTable != value)
                {
                    selectedTable = value;
                    OnPropertyChanged("SelectedTable");
                }
            }
        }

        private ObservableCollection<IQueryableTable> tableCollection;

        public ObservableCollection<IQueryableTable> TableCollection
        {
            get { return tableCollection; }
            set
            {
                if (tableCollection != value)
                {
                    tableCollection = value;
                    OnPropertyChanged("TableCollection");
                }
            }
        }

        private DelegateCommand refresh;

        public DelegateCommand Refresh
        {
            get { return refresh??(refresh = new DelegateCommand(RefreshAction)); }
            set
            {
                if (refresh != value)
                {
                    refresh = value;
                    OnPropertyChanged("Refresh");
                }
            }
        }

        private void RefreshAction(object obj)
        {
            if(SelectedTable!=null)
            {
                AppViewModel.Instance.OpenBusyIndicator();
                Context.ResponseValidator.Hadle(
                selectedTable.LoadOperation,
                (successOperation) =>
                    {
                        ItemCollection = new PagedCollectionView(successOperation.Entities);
                    },null,
                        (finalOperation) =>
                        {
                            AppViewModel.Instance.CloseBusyIndicator();
                        });
            }
        }

        #endregion

        protected override void Initialize()
        {
            base.Initialize();
            context = new TerminalZeroDataDomainContext();
            TableCollection = new ObservableCollection<IQueryableTable>();
            TableCollection.Add(new QueryableTableTemplate<Customer>("Customer", context.GetCustomersQuery(), context));
            TableCollection.Add(new QueryableTableTemplate<Product>("Products", context.GetProductsQuery(), context));
            TableCollection.Add(new QueryableTableTemplate<ProductGroup>("Product Group", context.GetProductGroupsQuery(), context));
            TableCollection.Add(new QueryableTableTemplate<Price>("Price", context.GetPricesQuery(), context));
            TableCollection.Add(new QueryableTableTemplate<PaymentInstrument>("Payment Instrument", context.GetPaymentInstrumentsQuery(), context));
            TableCollection.Add(new QueryableTableTemplate<SaleType>("Sale Type", context.GetSaleTypesQuery(), context));
            TableCollection.Add(new QueryableTableTemplate<StockCreateSummary>("Stock Create Summary", context.GetStockCreateSummariesQuery(), context));
            TableCollection.Add(new QueryableTableTemplate<StockModifySummary>("Stock Modify Summary", context.GetStockModifySummariesQuery(), context));
            TableCollection.Add(new QueryableTableTemplate<StockSummary>("Stock Summary", context.GetStockSummariesQuery(), context));
            TableCollection.Add(new QueryableTableTemplate<StockHeader>("Stock Header", context.GetStockHeadersQuery(), context));
            TableCollection.Add(new QueryableTableTemplate<StockItem>("Stock Item", context.GetStockItemsQuery(), context));
            TableCollection.Add(new QueryableTableTemplate<Supplier>("Suppliers", context.GetSuppliersQuery(), context));
            TableCollection.Add(new QueryableTableTemplate<Tax>("Taxes", context.GetTaxesQuery(), context));
            TableCollection.Add(new QueryableTableTemplate<TaxPosition>("Tax Positions", context.GetTaxPositionsQuery(), context));

            SelectedTable = TableCollection.FirstOrDefault();


        }
    }
}
