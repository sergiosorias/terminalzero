using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Windows.Input;
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
            Today = DateTime.Now.Date;
            LoadSalesForCurrentUser();
        }

        #region Properties

        private DateTime today;

        public DateTime Today
        {
            get { return today; }
            set
            {
                if (today != value)
                {
                    today = value;
                    OnPropertyChanged("Today");
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
                return updateCommand ?? (updateCommand = new ZeroActionDelegate((o) => LoadSalesForCurrentUser()));
            }
            set{ updateCommand = value;}
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
            IQueryable<SaleHeader> query = BusinessContext.Instance.Model.SaleHeaders.Where(
                sh => sh.UserCode == User.Code && EntityFunctions.TruncateTime(sh.Date) == today);

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
