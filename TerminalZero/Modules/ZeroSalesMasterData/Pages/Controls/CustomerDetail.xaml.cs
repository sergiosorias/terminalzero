using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ZeroCommonClasses.Interfaces;
using System.Data.Objects.DataClasses;

namespace ZeroMasterData.Pages.Controls
{
    /// <summary>
    /// Interaction logic for SupplierDetail.xaml
    /// </summary>
    public partial class CustomerDetail : UserControl, IZeroPage
    {
        private Entities.Customer _CustomerNew;
        public Entities.Customer CurrentCustomer 
        {
            get
            {
                return _CustomerNew;
            }
            
        }

        Entities.MasterDataEntities DataProvider;
        public CustomerDetail(Entities.MasterDataEntities dataProvider)
        {
            InitializeComponent();
            DataProvider = dataProvider;
        }

        public CustomerDetail(Entities.MasterDataEntities dataProvider, int supplierCode)
            : this(dataProvider)
        {
            _CustomerNew = DataProvider.Customers.First(s => s.Code == supplierCode);
            Mode = ZeroCommonClasses.Interfaces.Mode.Update;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            {
                taxPositionCodeComboBox.ItemsSource = DataProvider.TaxPositions;
                paymentInstrumentCodeComboBox.ItemsSource = DataProvider.PaymentInstruments;
                switch (Mode)
                {
                    case Mode.New:
                        _CustomerNew = Entities.Customer.CreateCustomer(
                            DataProvider.GetNextCustomerCode() ,0
                            , true);
                        paymentInstrumentCodeComboBox.SelectedIndex = 0;
                        break;
                    case Mode.Update:
                        if (!_CustomerNew.TaxPositionReference.IsLoaded)
                            _CustomerNew.TaxPositionReference.Load();
                        taxPositionCodeComboBox.SelectedItem = _CustomerNew.TaxPosition;
                        paymentInstrumentCodeComboBox.SelectedItem = _CustomerNew.PaymentInstrument;
                        break;
                    case Mode.Delete:
                        break;
                    case Mode.ReadOnly:
                        break;
                    default:
                        break;
                }
                

                grid1.DataContext = _CustomerNew;
            }
        }
        
        #region IZeroPage Members
        
        public bool CanAccept()
        {
            bool ret = true;

            if (ret)
            {
                switch (Mode)
                {
                    case Mode.New:
                        DataProvider.AddToCustomers(CurrentCustomer);
                        break;
                    case Mode.Update:
                        DataProvider.Customers.ApplyCurrentValues(CurrentCustomer);
                        break;
                    case Mode.Delete:
                        break;
                    case Mode.ReadOnly:
                        break;
                    default:
                        break;
                }

                DataProvider.SaveChanges();
            }

            return ret;
        }

        public bool CanCancel()
        {
            EntityObject obj = CurrentCustomer as EntityObject;
            if(obj.EntityState == System.Data.EntityState.Modified)
                DataProvider.Refresh(System.Data.Objects.RefreshMode.StoreWins, CurrentCustomer);

            return true;
        }
        private Mode _Mode = Mode.New;

        public Mode Mode
        {
            get
            {
                return _Mode;
            }
            set
            {
                _Mode = value;
            }
        }

        #endregion

        private void paymentInstrumentCodeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentCustomer.PaymentInstrument = (Entities.PaymentInstrument)paymentInstrumentCodeComboBox.SelectedItem;
        }

        private void taxPositionCodeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentCustomer.TaxPosition = (Entities.TaxPosition)taxPositionCodeComboBox.SelectedItem;
        }
    }
}
