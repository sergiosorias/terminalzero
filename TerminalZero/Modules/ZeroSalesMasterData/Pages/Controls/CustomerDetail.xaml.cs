using System.ComponentModel;
using System.Data;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ZeroMasterData.Entities;

namespace ZeroMasterData.Pages.Controls
{
    /// <summary>
    /// Interaction logic for SupplierDetail.xaml
    /// </summary>
    public partial class CustomerDetail : ZeroBasePage
    {
        private Customer _CustomerNew;
        public Customer CurrentCustomer 
        {
            get
            {
                return _CustomerNew;
            }
            
        }

        MasterDataEntities DataProvider;
        public CustomerDetail(MasterDataEntities dataProvider)
        {
            ControlMode = ControlMode.New;
            InitializeComponent();
            DataProvider = dataProvider;
            DataContext = this;
        }

        public CustomerDetail(MasterDataEntities dataProvider, int customerCode)
            : this(dataProvider)
        {
            _CustomerNew = DataProvider.Customers.First(s => s.Code == customerCode);
            ControlMode = ControlMode.Update;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                taxPositionCodeComboBox.ItemsSource = DataProvider.TaxPositions;
                paymentInstrumentCodeComboBox.ItemsSource = DataProvider.PaymentInstruments;
                switch (ControlMode)
                {
                    case ControlMode.New:
                        _CustomerNew = Customer.CreateCustomer(
                            DataProvider.GetNextCustomerCode() ,0
                            , true);
                        paymentInstrumentCodeComboBox.SelectedIndex = 0;
                        break;
                    case ControlMode.Update:
                        if (!_CustomerNew.TaxPositionReference.IsLoaded)
                            _CustomerNew.TaxPositionReference.Load();
                        taxPositionCodeComboBox.SelectedItem = _CustomerNew.TaxPosition;
                        paymentInstrumentCodeComboBox.SelectedItem = _CustomerNew.PaymentInstrument;
                        break;
                    case ControlMode.Delete:
                        break;
                    case ControlMode.ReadOnly:
                        break;
                    default:
                        break;
                }
                
                grid1.DataContext = _CustomerNew;
            }
        }
        
        #region IZeroPage Members
        
        public override bool CanAccept(object parameter)
        {
            bool ret = true;

            if (ret)
            {
                switch (ControlMode)
                {
                    case ControlMode.New:
                        DataProvider.AddToCustomers(CurrentCustomer);
                        break;
                    case ControlMode.Update:
                        DataProvider.Customers.ApplyCurrentValues(CurrentCustomer);
                        break;
                    case ControlMode.Delete:
                        break;
                    case ControlMode.ReadOnly:
                        break;
                    default:
                        break;
                }

                DataProvider.SaveChanges();
            }

            return ret;
        }

        public override bool CanCancel(object parameter)
        {
            EntityObject obj = CurrentCustomer;
            if(obj!=null && obj.EntityState == EntityState.Modified)
                DataProvider.Refresh(RefreshMode.StoreWins, CurrentCustomer);

            return true;
        }

        #endregion

        private void paymentInstrumentCodeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentCustomer.PaymentInstrument = (PaymentInstrument)paymentInstrumentCodeComboBox.SelectedItem;
        }

        private void taxPositionCodeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentCustomer.TaxPosition = (TaxPosition)taxPositionCodeComboBox.SelectedItem;
        }
    }
}
