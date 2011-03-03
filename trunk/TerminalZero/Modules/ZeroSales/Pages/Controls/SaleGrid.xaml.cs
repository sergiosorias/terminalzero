using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZeroCommonClasses.Interfaces;
using ZeroSales.Entities;

namespace ZeroSales.Pages.Controls
{
    /// <summary>
    /// Interaction logic for SaleGrid.xaml
    /// </summary>
    public partial class SaleGrid : UserControl, IZeroPage
    {
        public event RoutedEventHandler Removing;

        private void OnRemoving(object sender, RoutedEventArgs e)
        {
            if(ZeroGUI.ZeroMessageBox.Show(Properties.Resources.ItemDeletingQuestion,Properties.Resources.Delete,SizeToContent.WidthAndHeight, ResizeMode.NoResize, MessageBoxButton.YesNo).GetValueOrDefault())
            {
                RoutedEventHandler handler = Removing;
                if (handler != null) handler(saleItemsDataGrid.SelectedItem, e);    
            }
        }

        public SaleGrid()
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
                        removeColumn.Visibility = Visibility.Collapsed;
                        break;
                    default:
                        break;
                }
            }
        }

        public void Add(SaleItem item)
        {
            if (!item.ProductReference.IsLoaded)
            {
                item.ProductReference.Load();
            }
            saleItemsDataGrid.Items.Add(item);
        }

        public void Remove(SaleItem item)
        {
            saleItemsDataGrid.Items.Remove(item);
        }

        public void Clear()
        {
            saleItemsDataGrid.Items.Clear();
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

        private void saleItemsDataGrid_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.D && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                OnRemoving(null, null);
            }
        }
    }
}
