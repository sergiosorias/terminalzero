using System;
using System.ComponentModel;
using System.Data.Objects.DataClasses;
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
    public partial class SaleList : ZeroGUI.ListNavigationControl
    {
        public SaleList()
        {
            InitializeComponent();
            InitializeList(saleItemsDataGrid);
        }

        protected override void OnControlModeChanged(ControlMode newMode)
        {
            base.OnControlModeChanged(newMode);
            removeColumn.Visibility = (newMode == ControlMode.ReadOnly)
                                              ? Visibility.Collapsed
                                              : Visibility.Visible;
                
        }

        public override void AddItem(EntityObject item)
        {
            base.AddItem(item);
            var saleitem = item as SaleItem;
            if (saleitem != null && !saleitem.ProductReference.IsLoaded)
                    saleitem.ProductReference.Load();
        }
        
        private void saleItemsDataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.D && e.KeyboardDevice.Modifiers == ModifierKeys.Control)
            {
                RemoveItem(saleItemsDataGrid.SelectedItem as EntityObject);
            }
        }

        private void ClickeableItemButton_Click(object sender, RoutedEventArgs e)
        {
            RemoveItem(saleItemsDataGrid.SelectedItem as EntityObject);
        }
    }
}
