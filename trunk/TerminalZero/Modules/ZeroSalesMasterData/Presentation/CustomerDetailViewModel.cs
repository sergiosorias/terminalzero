using System.Collections.ObjectModel;
using System.Windows.Input;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ZeroMasterData.Pages.Controls;

namespace ZeroMasterData.Presentation
{
    public class CustomerDetailViewModel : ViewModelGui, ISelectable
    {
        public ControlMode ControlMode { get; set; }

        public Customer Customer { get; set; }

        public ICommand EditCommand
        {
            get
            {
                return editCommand ?? (editCommand = new ZeroActionDelegate(OpenCustomerEdit));
            }
            set
            {
                if (editCommand != value)
                {
                    editCommand = value;
                    OnPropertyChanged("EditCommand");
                }
            }
        }

        private ICommand editCommand;

        private ObservableCollection<TaxPosition> taxPositionList;

        public ObservableCollection<TaxPosition> TaxPositionList
        {
            get
            {
                return taxPositionList ?? (taxPositionList = new ObservableCollection<TaxPosition>(BusinessContext.Instance.ModelManager.TaxPositions));
            }
            set
            {
                if (taxPositionList != value)
                {
                    taxPositionList = value;
                    OnPropertyChanged("TaxPositionList");
                }
            }
        }

        private ObservableCollection<PaymentInstrument> paymentInstrumentList;

        public ObservableCollection<PaymentInstrument> PaymentInstrumentList
        {
            get
            {
                return paymentInstrumentList ?? (paymentInstrumentList = new ObservableCollection<PaymentInstrument>(BusinessContext.Instance.ModelManager.PaymentInstruments));
            }
            set
            {
                if (paymentInstrumentList != value)
                {
                    paymentInstrumentList = value;
                    OnPropertyChanged("PaymentInstrumentList");
                }
            }
        }
        
        public CustomerDetailViewModel(NavigationBasePage view)
            : base(view)
        {
            ControlMode = ControlMode.New;
        }

        public CustomerDetailViewModel()
            :this(null)
        {
            
        }

        private void OpenCustomerEdit(object parameter)
        {
            View = new CustomerDetail {ControlMode = ControlMode.Update};
            
            if(ZeroMessageBox.Show(View, string.Format("Editar cliente {0}", Customer.Name1)).GetValueOrDefault())
            {
                BusinessContext.Instance.ModelManager.Customers.ApplyCurrentValues(Customer);
            }
            else
            {
                BusinessContext.Instance.ModelManager.Refresh(System.Data.Objects.RefreshMode.StoreWins, Customer);
            }
            
        }

        protected override void PrintCommandExecution(object parameter)
        {
            base.PrintCommandExecution(parameter);
        }
        
        #region ISelectable Members

        public bool Contains(string data)
        {
            return Customer.Contains(data);
        }

        public bool Contains(System.DateTime data)
        {
            return Customer.Contains(data);
        }

        #endregion
    }
}