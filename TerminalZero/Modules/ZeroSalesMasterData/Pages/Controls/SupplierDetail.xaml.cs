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
    public partial class SupplierDetail : NavigationBasePage
    {
        public Supplier CurrentSupplier
        {
            get;
            private set;
        }

        public SupplierDetail()
        {
            InitializeComponent();

        }

        public SupplierDetail(int supplierCode)
            : this()
        {
            CurrentSupplier = BusinessContext.Instance.Manager.Suppliers.First(s => s.Code == supplierCode);
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
                        CurrentSupplier = Supplier.CreateSupplier(
                            BusinessContext.Instance.Manager.Suppliers.Count()
                            , true);
                        break;
                    case ControlMode.Update:
                        if (!CurrentSupplier.TaxPositionReference.IsLoaded)
                            CurrentSupplier.TaxPositionReference.Load();
                        taxPositionCodeComboBox.SelectedItem = CurrentSupplier.TaxPosition;
                        paymentInstrumentCodeComboBox.SelectedItem = CurrentSupplier.PaymentInstrument;
                        break;
                    case ControlMode.Delete:
                        break;
                    case ControlMode.ReadOnly:
                        break;
                    default:
                        break;
                }


                grid1.DataContext = CurrentSupplier;
            }
        }

        public override bool CanAccept(object parameter)
        {
            bool ret = base.CanAccept(parameter);

            if (ret)
            {
                switch (ControlMode)
                {
                    case ControlMode.New:
                        BusinessContext.Instance.Manager.AddToSuppliers(CurrentSupplier);
                        break;
                    case ControlMode.Update:
                        BusinessContext.Instance.Manager.Suppliers.ApplyCurrentValues(CurrentSupplier);
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
            EntityObject obj = CurrentSupplier;
            if (obj.EntityState == EntityState.Modified)
                BusinessContext.Instance.Manager.Refresh(RefreshMode.StoreWins, CurrentSupplier);

            return true;
        }

        private void paymentInstrumentCodeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentSupplier.PaymentInstrument = (PaymentInstrument)paymentInstrumentCodeComboBox.SelectedItem;
        }

        private void taxPositionCodeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentSupplier.TaxPosition = (TaxPosition)taxPositionCodeComboBox.SelectedItem;
        }
    }
}
