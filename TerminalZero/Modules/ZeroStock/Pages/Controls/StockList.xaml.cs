using System;
using System.Data.Objects.DataClasses;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using ZeroCommonClasses.Interfaces;

namespace ZeroStock.Pages.Controls
{
    /// <summary>
    /// Interaction logic for StockGrid.xaml
    /// </summary>
    public partial class StockList : ZeroGUI.ListNavigationControl
    {
        public StockList()
        {
            InitializeComponent();
            InitializeList(stockItemsDataGrid);
        }

        public override void AddItem(EntityObject item)
        {
            base.AddItem(item);
            var stockItem = item as Entities.StockItem;
            if (stockItem!=null && !stockItem.ProductReference.IsLoaded)
            {
                stockItem.ProductReference.Load();
            }
        }
    }
}
