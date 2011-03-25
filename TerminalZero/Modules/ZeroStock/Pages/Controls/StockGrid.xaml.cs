using System;
using System.Windows;
using System.Windows.Controls;
using System.ComponentModel;
using ZeroCommonClasses.Interfaces;

namespace ZeroStock.Pages.Controls
{
    /// <summary>
    /// Interaction logic for StockGrid.xaml
    /// </summary>
    public partial class StockGrid : ZeroGUI.ZeroBasePage
    {
        public StockGrid()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                switch (ControlMode)
                {
                    case ControlMode.New:
                        break;
                    case ControlMode.Update:
                        break;
                    case ControlMode.Delete:
                        break;
                    case ControlMode.ReadOnly:
                        break;
                    default:
                        break;
                }
            }
        }

        public void Add(Entities.StockItem item)
        {
            if (!item.ProductReference.IsLoaded)
            {
                item.ProductReference.Load();
            }
            stockItemsDataGrid.Items.Add(item);    
        }
    }
}
