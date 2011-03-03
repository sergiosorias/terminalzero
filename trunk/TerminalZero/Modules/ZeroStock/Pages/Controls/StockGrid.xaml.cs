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
    public partial class StockGrid : UserControl, IZeroPage
    {
        public StockGrid()
        {
            Mode = Mode.New;
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                switch (Mode)
                {
                    case Mode.New:
                        break;
                    case Mode.Update:
                        break;
                    case Mode.Delete:
                        break;
                    case Mode.ReadOnly:
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
        
        #region IZeroPage Members

        public Mode Mode { get; set; }

        public bool CanAccept(object parameter)
        {
            throw new NotImplementedException();
        }

        public bool CanCancel(object parameter)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}
