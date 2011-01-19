using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZeroCommonClasses.Interfaces;
using System.Data.Objects.DataClasses;

namespace ZeroMasterData.Pages.Controls
{
    /// <summary>
    /// Interaction logic for SupplierDetail.xaml
    /// </summary>
    public partial class SupplierDetail : UserControl, IZeroPage
    {
        private Entities.Supplier _SupplierNew;
        public Entities.Supplier CurrentSupplier 
        {
            get
            {
                return _SupplierNew;
            }
            
        }

        Entities.MasterDataEntities DataProvider;
        public SupplierDetail(Entities.MasterDataEntities dataProvider)
        {
            InitializeComponent();
            DataProvider = dataProvider;
        }

        public SupplierDetail(Entities.MasterDataEntities dataProvider,int supplierCode)
            : this(dataProvider)
        {
            _SupplierNew = DataProvider.Suppliers.First(s => s.Code == supplierCode);
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
                        _SupplierNew = Entities.Supplier.CreateSupplier(
                            DataProvider.Suppliers.Count()
                            , true);
                        paymentInstrumentCodeComboBox.SelectedIndex = 0;
                        break;
                    case Mode.Update:
                        if (!_SupplierNew.TaxPositionReference.IsLoaded)
                            _SupplierNew.TaxPositionReference.Load();
                        taxPositionCodeComboBox.SelectedItem = _SupplierNew.TaxPosition;
                        paymentInstrumentCodeComboBox.SelectedItem = _SupplierNew.PaymentInstrument;
                        break;
                    case Mode.Delete:
                        break;
                    case Mode.ReadOnly:
                        break;
                    default:
                        break;
                }
                

                grid1.DataContext = _SupplierNew;
            }
        }
        
        #region IZeroPage Members

        public bool CanAccept(object parameter)
        {
            bool ret = true;

            if (ret)
            {
                switch (Mode)
                {
                    case Mode.New:
                        DataProvider.AddToSuppliers(CurrentSupplier);
                        break;
                    case Mode.Update:
                        DataProvider.Suppliers.ApplyCurrentValues(CurrentSupplier);
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

        public bool CanCancel(object parameter)
        {
            EntityObject obj = CurrentSupplier as EntityObject;
            if (obj.EntityState == System.Data.EntityState.Modified)
                DataProvider.Refresh(System.Data.Objects.RefreshMode.StoreWins, CurrentSupplier);

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
            CurrentSupplier.PaymentInstrument = (Entities.PaymentInstrument)paymentInstrumentCodeComboBox.SelectedItem;
        }

        private void taxPositionCodeComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentSupplier.TaxPosition = (Entities.TaxPosition)taxPositionCodeComboBox.SelectedItem;
        }
    }
}
