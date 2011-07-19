using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Objects;
using System.Linq;
using System.Windows.Input;
using ZeroBusiness;
using ZeroBusiness.Entities.Configuration;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroGUI;
using ZeroSales.Pages;
using ZeroSales.Presentation.Controls;
using Terminal = ZeroCommonClasses.Terminal;

namespace ZeroSales.Presentation
{
    public class SaleReportViewModel : ViewModelGui
    {
        public class SaleReportItem
        {
            public SaleHeader SaleHeader { get; set; }
            public ZeroActionDelegate ViewDetailsCommand { get; set; }

            public SaleReportItem()
            {
                ViewDetailsCommand = new ZeroActionDelegate(OpenDetailView);
            }

            private void OpenDetailView(object obj)
            {
                var vm = new SaleReportItemViewModel(SaleHeader);
                Terminal.Instance.CurrentClient.ShowDialog(vm.View, null);
            }
        }

        public class SaleReportSummary
        {
            public double TotalAmount { get; set; }
            public int TotalCount { get; set; }
            public ObservableCollection<SalePaymentReportSummary> SalesPaymentSummary { get; set; }
        }

        public class SalePaymentReportSummary
        {
            public SalePaymentReportSummary(PaymentInstrument paymentInstrument)
            {
                PaymentInstrument = paymentInstrument;
                TotalAmount = TotalCount = 0;
            }
            public PaymentInstrument PaymentInstrument { private get; set; }
            public double TotalAmount { get; set; }
            public int TotalCount { get; set; }
        }

        public SaleReportViewModel()
            :base(new SaleReportView())
        {
            SelectedDate = DateTime.Now.Date;
            IsByDate = true;
            LoadSalesForCurrentUser();
            CustomerSelectionCommand.Executed += CustomerSelectionCommandFinished;
        }

        #region Properties

        private bool isByDate;

        public bool IsByDate
        {
            get { return isByDate; }
            set
            {
                if (isByDate != value)
                {
                    isByDate = value;
                    SelectedCustomer = null;
                    OnPropertyChanged("IsByDate");
                }
            }
        }
        
        private DateTime selectedDate;

        public DateTime SelectedDate
        {
            get { return selectedDate; }
            set
            {
                if (selectedDate != value)
                {
                    selectedDate = value;
                    OnPropertyChanged("SelectedDate");
                }
            }
        }

        private Customer selectedCustomer;

        public Customer SelectedCustomer
        {
            get { return selectedCustomer; }
            set
            {
                if (selectedCustomer != value)
                {
                    selectedCustomer = value;
                    OnPropertyChanged("SelectedCustomer");
                }
            }
        }
        
        private SaleReportItem selectedItem;

        public SaleReportItem SelectedItem
        {
            get { return selectedItem; }
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    LoadCurrentSaleItems();
                    OnPropertyChanged("SelectedItem");
                }
            }
        }

        private ICommand updateCommand;

        public ICommand UpdateCommand
        {
            get {
                return updateCommand ?? (updateCommand = new ZeroActionDelegate((o) => LoadSalesForCurrentUser(), (o) => IsByDate || (!IsByDate && SelectedCustomer != null)));
            }
            set{ updateCommand = value;}
        }

        private ZeroAction customerSelectionCommand;

        public ZeroAction CustomerSelectionCommand
        {
            get
            {
                return customerSelectionCommand ??
                       (customerSelectionCommand =
                        Terminal.Instance.Session.Actions[Actions.OpenCustomersSelectionView]);
            }
            set
            {
                if (customerSelectionCommand != value)
                {
                    customerSelectionCommand = value;
                    OnPropertyChanged("CustomerSelectionCommand");
                }
            }
        }

        private void CustomerSelectionCommandFinished(object sender, EventArgs e)
        {
            var customer = Terminal.Instance.Session[typeof(Customer)];
            if (customer != null)
            {
                SelectedCustomer = (Customer)customer.Value;
            }
        }
        
        public User User { get { return Terminal.Instance.Session[typeof(User)].Value as User; } }
        
        private ObservableCollection<SaleReportItem> sales;

        public ObservableCollection<SaleReportItem> Sales
        {
            get { return sales; }
            set
            {
                if (sales != value)
                {
                    sales = value;
                    OnPropertyChanged("Sales");
                }
            }
        }

        private ObservableCollection<SaleLazyLoadingItemViewModel> selectedSaleItems;

        public ObservableCollection<SaleLazyLoadingItemViewModel> SelectedSaleItems
        {
            get { return selectedSaleItems; }
            set
            {
                if (selectedSaleItems != value)
                {
                    selectedSaleItems = value;
                    OnPropertyChanged("SelectedSaleItems");
                }
            }
        }

        private ObservableCollection<SalePaymentItemViewModel> salesPaymentSummary;

        public ObservableCollection<SalePaymentItemViewModel> SalesPaymentSummary
        {
            get { return salesPaymentSummary; }
            set
            {
                if (salesPaymentSummary != value)
                {
                    salesPaymentSummary = value;
                    OnPropertyChanged("SalesPaymentSummary");
                }
            }
        }
        
        private SaleReportSummary summary;

        public SaleReportSummary Summary
        {
            get { return summary; }
            set
            {
                if (summary != value)
                {
                    summary = value;
                    OnPropertyChanged("Summary");
                }
            }
        }
        

        #endregion

        private void LoadCurrentSaleItems()
        {
            if (SelectedItem != null)
            {
                SelectedSaleItems = new ObservableCollection<SaleLazyLoadingItemViewModel>(
                    SelectedItem.SaleHeader.SaleItems.Select(si => new SaleLazyLoadingItemViewModel(si, null)));

                SalesPaymentSummary = new ObservableCollection<SalePaymentItemViewModel>(SelectedItem.SaleHeader.SalePaymentHeader.SalePaymentItems.Select(pi => new SalePaymentItemViewModel { PaymentItem = pi }));
            }
            else
            {
                SelectedSaleItems.Clear();
                SalesPaymentSummary.Clear();
            }
        }

        private void LoadSalesForCurrentUser()
        {
            ObservableCollection<SaleReportItem> result;

            System.Linq.Expressions.Expression<Func<SaleHeader, bool>> predicate;

            if (IsByDate)
                predicate = sh => sh.UserCode == User.Code && EntityFunctions.TruncateTime(sh.Date) == SelectedDate;
            else
                predicate = sh => sh.CustomerCode != null && sh.CustomerCode == SelectedCustomer.Code;

            IQueryable<SaleHeader> query = BusinessContext.Instance.Model.SaleHeaders.Where(predicate);

            result = new ObservableCollection<SaleReportItem>(query.Select(sh => new SaleReportItem { SaleHeader = sh }));

            var payment = from spi in BusinessContext.Instance.Model.SalePaymentItems
                          join sh in query on spi.SalePaymentHeader equals sh.SalePaymentHeader
                          group spi by spi.PaymentInstrument into g
                          select g;

            var paymentListSummary = new Dictionary<int, SalePaymentReportSummary>();
            SalePaymentReportSummary paymentItem;
            foreach (IGrouping<PaymentInstrument, SalePaymentItem> items in payment)
            {
                paymentListSummary.Add(items.Key.Code, new SalePaymentReportSummary(items.Key));
                paymentItem = paymentListSummary[items.Key.Code];
                foreach (SalePaymentItem salePaymentItem in items)
                {
                    paymentItem.TotalCount++;
                    paymentItem.TotalAmount += salePaymentItem.Quantity;
                }
            }

            int count = query.Count();
            
            Summary = new SaleReportSummary
            {
                TotalAmount = count > 0? result.Sum(sh => sh.SaleHeader.PriceSumValue): count,
                TotalCount = count,
                SalesPaymentSummary = new ObservableCollection<SalePaymentReportSummary>(paymentListSummary.Values)
            };
            

            Sales = result;
        }
    }
}
