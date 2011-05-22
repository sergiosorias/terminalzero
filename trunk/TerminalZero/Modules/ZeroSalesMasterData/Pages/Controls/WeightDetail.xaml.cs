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
        public Weight CurrentWeigth { get; private set; }

        public WeightDetail()
        {
            InitializeComponent();
        }

        public WeightDetail(Weight data)
            : this()
        {
            CurrentWeigth = data;
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentWeigth == null)
            {
                CurrentWeigth = new Weight(1000);
            }
            grid1.DataContext = CurrentWeigth;
        }
    }
}
