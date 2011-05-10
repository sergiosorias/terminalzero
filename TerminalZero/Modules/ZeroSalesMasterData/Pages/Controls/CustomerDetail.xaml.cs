using System;
using System.ComponentModel;
using System.Data;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;

namespace ZeroMasterData.Pages.Controls
{
    /// <summary>
    /// Interaction logic for SupplierDetail.xaml
    /// </summary>
    public partial class CustomerDetail : NavigationBasePage
    {
        public Customer CurrentCustomer
        {
            get;
            private set;

        }

        public CustomerDetail()
        {
            ControlMode = ControlMode.New;
            InitializeComponent();
            DataContext = this;
        }

        public CustomerDetail(int customerCode)
            : this()
        {
            CurrentCustomer = BusinessContext.Instance.Manager.Customers.First(s => s.Code == customerCode);
            ControlMode = ControlMode.Update;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                taxPositionCodeComboBox.ItemsSource = BusinessContext.Instance.Manager.TaxPositions;
                paymentInstrumentCodeComboBox.ItemsSource = BusinessContext.Instance.Manager.PaymentInstruments;
                switch (ControlMode)
                {
                    case ControlMode.New:
                        CurrentCustomer = Customer.CreateCustomer(
                            BusinessContext.Instance.Manager.GetNextCustomerCode(), 0
                            , true);
                        paymentInstrumentCodeComboBox.SelectedIndex = 0;
                        Header = "Cliente Nuevo";
                        break;
                    case ControlMode.Update:
                        if (!CurrentCustomer.TaxPositionReference.IsLoaded)
                            CurrentCustomer.TaxPositionReference.Load();
                        taxPositionCodeComboBox.SelectedItem = CurrentCustomer.TaxPosition;
                        paymentInstrumentCodeComboBox.SelectedItem = CurrentCustomer.PaymentInstrument;
                        Header = "Editar Cliente";
                        break;
                }

                grid1.DataContext = CurrentCustomer;
            }
        }

        #region IZeroPage Members

        public override bool CanAccept(object parameter)
        {
            bool ret = base.CanAccept(parameter);

            if (ret)
            {
                switch (ControlMode)
                {
                    case ControlMode.New:
                        BusinessContext.Instance.Manager.AddToCustomers(CurrentCustomer);
                        break;
                    case ControlMode.Update:
                        BusinessContext.Instance.Manager.Customers.ApplyCurrentValues(CurrentCustomer);
                        break;
                    case ControlMode.Delete:
                        break;
                    case ControlMode.ReadOnly:
                        break;
                    default:
                        break;
                }

                BusinessContext.Instance.Manager.SaveChanges();
            }

            return ret;
        }

        public override bool CanCancel(object parameter)
        {
            EntityObject obj = CurrentCustomer;
            if (obj != null && obj.EntityState == EntityState.Modified)
                BusinessContext.Instance.Manager.Refresh(RefreshMode.StoreWins, CurrentCustomer);

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
