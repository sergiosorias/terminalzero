using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Objects;
using System.Data.Objects.SqlClient;
using System.Linq;
using System.Text;
using System.Windows.Input;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroGUI;
using ZeroSales.Pages;

namespace ZeroSales.Presentation
{
    public class SaleStatisticsViewModel : ViewModelGui
    {
        public SaleStatisticsViewModel()
            : base(new SaleStatistics())
        {
            toDate = DateTime.Now;
            fromDate = ToDate.AddDays(-10);
            GenerateColection();
        }

        private void GenerateColection()
        {
            var result = from s in ZeroBusiness.Manager.Data.BusinessContext.Instance.Model.SaleHeaders
                         join t in ZeroBusiness.Manager.Data.BusinessContext.Instance.Model.TerminalToes on s.TerminalCode equals t.Code
                         where s.Date >= FromDate && s.Date <= ToDate
                         group s by new {s.TerminalCode, t.Description, Date = EntityFunctions.TruncateTime(s.Date)} into g
                         select new { Code = g.Key.TerminalCode, Name = g.Key.Description, Sale = new SaleSummary {Date = g.Key.Date.Value, SalesCount = g.Count(), ValueSum = g.Sum(h=>h.PriceSumValue)}};

            var reports = from g in result
                       group g by new {g.Name, g.Code} into r
                       select new TerminalReport {TerminalCode = r.Key.Code, TerminalName = r.Key.Name, Items = r.Select(s => s.Sale).OrderBy(s=>s.Date)};

            TotalTerminalSalesCollection = new ObservableCollection<TerminalReport>(reports);
            
            MaxFromDate = FromDate;
            MaxToDate = ToDate.AddDays(1);
            filterChange = false;
            UpdateCommand.RaiseCanExecuteChanged();
        }

        #region Properties
        private ObservableCollection<TerminalReport> totalTerminalSalesCollection;

        public ObservableCollection<TerminalReport> TotalTerminalSalesCollection
        {
            get { return totalTerminalSalesCollection; }
            set
            {
                if (totalTerminalSalesCollection != value)
                {
                    totalTerminalSalesCollection = value;
                    OnPropertyChanged("TotalTerminalSalesCollection");
                }
            }
        }

        private bool filterChange = false;

        private DateTime toDate;

        public DateTime ToDate
        {
            get { return toDate; }
            set
            {
                if (toDate != value)
                {
                    toDate = value;
                    filterChange = true;
                    UpdateCommand.RaiseCanExecuteChanged();
                    OnPropertyChanged("ToDate");
                }
            }
        }

        private DateTime fromDate;

        public DateTime FromDate
        {
            get { return fromDate; }
            set
            {
                if (fromDate != value)
                {
                    fromDate = value;
                    filterChange = true;
                    UpdateCommand.RaiseCanExecuteChanged();
                    OnPropertyChanged("FromDate");
                }
            }
        }

        private DateTime maxToDate;

        public DateTime MaxToDate
        {
            get { return maxToDate; }
            set
            {
                if (maxToDate != value)
                {
                    maxToDate = value;
                    OnPropertyChanged("MaxToDate");
                }
            }
        }

        private DateTime maxFromDate;

        public DateTime MaxFromDate
        {
            get { return maxFromDate; }
            set
            {
                if (maxFromDate != value)
                {
                    maxFromDate = value;
                    OnPropertyChanged("MaxFromDate");
                }
            }
        }

        #endregion

        private ZeroActionDelegate updateCommand;

        public ZeroActionDelegate UpdateCommand
        {
            get { return updateCommand ?? (updateCommand = new ZeroActionDelegate((o) => GenerateColection(),(o)=>filterChange)); }
            set
            {
                if (updateCommand != value)
                {
                    updateCommand = value;
                    OnPropertyChanged("UpdateCommand");
                }
            }
        }



    }
}
