using System.Linq;
using System.Windows;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroGUI;

namespace ZeroMasterData.Pages.Controls
{
    /// <summary>
    /// Interaction logic for WeightDetail.xaml
    /// </summary>
    public partial class WeightDetail : NavigationBasePage
    {
        public Weight WeigthNew { get; private set; }

        public WeightDetail()
        {
            InitializeComponent();
        }

        public WeightDetail(Weight Data)
            : this()
        {
            WeigthNew = Data;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (WeigthNew == null)
            {
                WeigthNew = Weight.CreateWeight(BusinessContext.Instance.Manager.Weights.Count()
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
