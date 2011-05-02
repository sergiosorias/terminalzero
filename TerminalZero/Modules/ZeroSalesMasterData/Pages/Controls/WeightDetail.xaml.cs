using System.Linq;
using System.Windows;
using ZeroBusiness.Entities.Data;
using ZeroGUI;

namespace ZeroMasterData.Pages.Controls
{
    /// <summary>
    /// Interaction logic for WeightDetail.xaml
    /// </summary>
    public partial class WeightDetail : NavigationBasePage
    {
        public Weight WeigthNew { get; private set; }

        DataModelManager DataProvider;
        public WeightDetail(DataModelManager dataProvider)
        {
            InitializeComponent();
            DataProvider = dataProvider;
        }

        public WeightDetail(DataModelManager dataProvider, Weight Data)
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
            bool ret = base.CanAccept(parameter);

            //if (!ret)
            //    MessageBox.Show(msg, "Error", MessageBoxButton.OK, MessageBoxImage.Error);

            return ret;
        }
    }
}
