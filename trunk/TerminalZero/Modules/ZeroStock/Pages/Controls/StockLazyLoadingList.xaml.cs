using System.Data.Objects.DataClasses;
using ZeroBusiness.Entities.Data;
using ZeroGUI;

namespace ZeroStock.Pages.Controls
{
    /// <summary>
    /// Interaction logic for StockGrid.xaml
    /// </summary>
    public partial class StockLazyLoadingList : LazyLoadingListControlUpgrade
    {
        public StockLazyLoadingList()
        {
            InitializeComponent();
        }
    }
}
