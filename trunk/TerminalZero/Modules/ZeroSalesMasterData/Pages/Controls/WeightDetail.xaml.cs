using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZeroCommonClasses.Interfaces;
using ZeroMasterData.Entities;

namespace ZeroMasterData.Pages.Controls
{
    /// <summary>
    /// Interaction logic for WeightDetail.xaml
    /// </summary>
    public partial class WeightDetail : UserControl, IZeroPage
    {
        public Entities.Weight WeigthNew { get; private set; }

        MasterDataEntities DataProvider;
        public WeightDetail(MasterDataEntities dataProvider)
        {
            InitializeComponent();
            DataProvider = dataProvider;
        }

        public WeightDetail(MasterDataEntities dataProvider, Entities.Weight Data)
            : this(dataProvider)
        {
            WeigthNew = Data;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (WeigthNew == null)
            {
                WeigthNew = Entities.Weight.CreateWeight(DataProvider.Weights.Count()
                    , true, 0);
            }
            grid1.DataContext = WeigthNew;
            quantityTextBox.Text = "";
        }

        #region IZeroPage Members

        public bool CanAccept(object parameter)
        {
            bool ret = true;

            string msg = "";
            if (string.IsNullOrWhiteSpace(nameTextBox.Text))
            {
                msg = "Por favor ingrese un nombre para la unidad de medida.";
                ret = false;
            }

            if (string.IsNullOrWhiteSpace(quantityTextBox.Text))
            {
                msg += "Por favor ingrese una cantidad.";
                ret = false;
            }

            if (!ret)
                System.Windows.MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            return ret;
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

        public bool CanCancel(object parameter)
        {
            return true;
        }

        #endregion

        private void quantityTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            quantityTextBox.SelectAll();
            
        }

    }
}
