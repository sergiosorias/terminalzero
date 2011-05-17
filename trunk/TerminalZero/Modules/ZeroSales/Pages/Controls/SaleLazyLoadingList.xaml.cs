using System;
using System.ComponentModel;
using System.Data.Objects.DataClasses;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using ZeroBusiness.Entities.Data;
using ZeroCommonClasses.Interfaces;

namespace ZeroSales.Pages.Controls
{
    /// <summary>
    /// Interaction logic for SaleGrid.xaml
    /// </summary>
    public partial class SaleLazyLoadingList : ZeroGUI.LazyLoadingListControl
    {
        public SaleLazyLoadingList()
        {
            InitializeComponent();
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
            var saleitem = item as SaleItem;
            if (saleitem != null && !saleitem.ProductReference.IsLoaded)
                saleitem.ProductReference.Load();
            base.AddItem(item);
        }

        protected override void OnKeyboardDeleteKeysPressed()
        {
            base.OnKeyboardDeleteKeysPressed();
            TryRemoveItem(SelectedItem as EntityObject);
        }

        private void ClickeableItemButton_Click(object sender, RoutedEventArgs e)
        {
            TryRemoveItem(SelectedItem as EntityObject);
        }
    }
}
