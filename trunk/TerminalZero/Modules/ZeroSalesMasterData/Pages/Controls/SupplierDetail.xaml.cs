using System;
using System.ComponentModel;
using System.Data;
using System.Data.Objects;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZeroBusiness.Entities.Data;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;

namespace ZeroMasterData.Pages.Controls
{
    /// <summary>
    /// Interaction logic for SupplierDetail.xaml
    /// </summary>
    public partial class SupplierDetail : NavigationBasePage
    {
        private Supplier _SupplierNew;
        public Supplier CurrentSupplier 
        {
            get
            {
                return _SupplierNew;
            }
            
        }

        DataModelManager DataProvider;
        public SupplierDetail(DataModelManager dataProvider)
        {
            InitializeComponent();
            DataProvider = dataProvider;
        }

        public SupplierDetail(DataModelManager dataProvider,int supplierCode)
            : this(dataProvider)
        {
            _SupplierNew = DataProvider.Suppliers.First(s => s.Code == supplierCode);
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
                        _SupplierNew = Supplier.CreateSupplier(
                            DataProvider.Suppliers.Count()
                            , true);
                        break;
                    case ControlMode.Update:
                        if (!_SupplierNew.TaxPositionReference.IsLoaded)
                            _SupplierNew.TaxPositionReference.Load();
                        taxPositionCodeComboBox.SelectedItem = _SupplierNew.TaxPosition;
                        paymentInstrumentCodeComboBox.SelectedItem = _SupplierNew.PaymentInstrument;
                        break;
                    case ControlMode.Delete:
                        break;
                    case ControlMode.ReadOnly:
                        break;
                    default:
                        break;
                }
                

                grid1.DataContext = _SupplierNew;
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
                        DataProvider.AddToSuppliers(CurrentSupplier);
                        break;
                    case ControlMode.Update:
                        DataProvider.Suppliers.ApplyCurrentValues(CurrentSupplier);
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
            EntityObject obj = CurrentSupplier;
            if (obj.EntityState == EntityState.Modified)
                DataProvider.Refresh(RefreshMode.StoreWins, CurrentSupplier);

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
