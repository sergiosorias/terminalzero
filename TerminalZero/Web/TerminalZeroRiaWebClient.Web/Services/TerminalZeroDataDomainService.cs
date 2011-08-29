
using System.Linq;
using System.ServiceModel.DomainServices.EntityFramework;
using System.ServiceModel.DomainServices.Hosting;
using System.ServiceModel.DomainServices.Server;
using TerminalZeroRiaWebClient.Web.Classes;
using TerminalZeroRiaWebClient.Web.Models;
using ZeroBusiness.Entities.Data;

namespace TerminalZeroRiaWebClient.Web.Services
{
    // Implements application logic using the DataModelManager context.
    // TODO: Add your application logic to these methods or in additional methods.
    // TODO: Wire up authentication (Windows/ASP.NET Forms) and uncomment the following to disable anonymous access
    // Also consider adding roles to restrict access as appropriate.
    [RequiresAuthentication(ErrorMessage="Unauthorized access to data, please LOG IN")]
    [RequiresRole(Security.AdministratorRoleId, Security.CientAdministratorRoleId, ErrorMessage = "You do not have enough rights to access data")]
    [EnableClientAccess]
    public class TerminalZeroDataDomainService : LinqToEntitiesDomainService<DataModelManager>
    {
        public IQueryable<StockHeader> GetStockHeaders()
        {
            return ObjectContext.StockHeaders;
        }

        public IQueryable<StockItem> GetStockItems()
        {
            return ObjectContext.StockItems;
        }

        public IQueryable<Weight> GetWeights()
        {
            return ObjectContext.Weights;
        }

        public IQueryable<DeliveryDocumentHeader> GetDeliveryDocumentHeaders()
        {
            return ObjectContext.DeliveryDocumentHeaders;
        }

        public IQueryable<DeliveryDocumentItem> GetDeliveryDocumentItems()
        {
            return ObjectContext.DeliveryDocumentItems;
        }

        public IQueryable<DeliveryNoteHeader> GetDeliveryNoteHeaders()
        {
            return ObjectContext.DeliveryNoteHeaders;
        }

        public IQueryable<DeliveryNoteItem> GetDeliveryNoteItems()
        {
            return ObjectContext.DeliveryNoteItems;
        }

        public IQueryable<PaymentInstrument> GetPaymentInstruments()
        {
            return ObjectContext.PaymentInstruments;
        }

        public IQueryable<Price> GetPrices()
        {
            return ObjectContext.Prices;
        }

        public IQueryable<Customer> GetCustomers()
        {
            return ObjectContext.Customers;
        }

        public IQueryable<Product> GetProducts()
        {
            return ObjectContext.Products;
        }

        public IQueryable<ProductGroup> GetProductGroups()
        {
            return ObjectContext.ProductGroups;
        }

        public IQueryable<SaleType> GetSaleTypes()
        {
            return ObjectContext.SaleTypes;
        }

        public IQueryable<StockCreateSummary> GetStockCreateSummaries()
        {
            return ObjectContext.StockCreateSummaries;
        }

        public IQueryable<StockModifySummary> GetStockModifySummaries()
        {
            return ObjectContext.StockModifySummaries;
        }

        public IQueryable<StockSummary> GetStockSummaries()
        {
            return ObjectContext.StockSummaries;
        }

        public IQueryable<StockType> GetStockTypes()
        {
            return ObjectContext.StockTypes;
        }

        public IQueryable<Supplier> GetSuppliers()
        {
            return ObjectContext.Suppliers;
        }

        public IQueryable<Tax> GetTaxes()
        {
            return ObjectContext.Taxes;
        }
        
        public IQueryable<TaxPosition> GetTaxPositions()
        {
            return ObjectContext.TaxPositions;
        }
    }
}


