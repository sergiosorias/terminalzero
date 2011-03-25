using System.Linq;
using System.Windows;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ZeroMasterData.Entities;

namespace ZeroMasterData.Pages.Controls
{
    /// <summary>
    /// Interaction logic for WeightDetail.xaml
    /// </summary>
    public partial class WeightDetail : ZeroBasePage
    {
        public Weight WeigthNew { get; private set; }

        MasterDataEntities DataProvider;
        public WeightDetail(MasterDataEntities dataProvider)
        {
            ControlMode = ControlMode.New;
            InitializeComponent();
            DataProvider = dataProvider;
        }

        public WeightDetail(MasterDataEntities dataProvider, Weight Data)
            : this(dataProvider)
        {
            WeigthNew = Data;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (WeigthNew == null)
            {
                WeigthNew = Weight.CreateWeight(DataProvider.Weights.Count()
                    , true, 0);
            }
            grid1.DataContext = WeigthNew;
            quantityTextBox.Text = "";
        }

        public override bool CanAccept(object parameter)
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
                MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            return ret;
        }

 

        private void quantityTextBox_GotFocus(object sender, RoutedEventArgs e)
        {
            quantityTextBox.SelectAll();
            
        }

    }
}
