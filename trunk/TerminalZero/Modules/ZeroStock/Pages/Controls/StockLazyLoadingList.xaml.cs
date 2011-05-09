using System.Data.Objects.DataClasses;
using ZeroBusiness.Entities.Data;
using ZeroGUI;

namespace ZeroStock.Pages.Controls
{
    /// <summary>
    /// Interaction logic for StockGrid.xaml
    /// </summary>
    public partial class StockLazyLoadingList : LazyLoadingListControl
    {
        public StockLazyLoadingList()
        {
            InitializeComponent();
        }

        public override void AddItem(EntityObject item)
        {
            base.AddItem(item);
            var stockItem = item as StockItem;
            if (stockItem!=null && !stockItem.ProductReference.IsLoaded)
            {
                stockItem.ProductReference.Load();
            }
        }
    }
}
