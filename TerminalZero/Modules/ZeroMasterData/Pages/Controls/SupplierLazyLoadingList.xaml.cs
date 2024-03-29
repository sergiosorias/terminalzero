﻿using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroGUI;

namespace ZeroMasterData.Pages.Controls
{
    /// <summary>
    /// Interaction logic for SupplierGrid.xaml
    /// </summary>
    public partial class SupplierLazyLoadingList : LazyLoadingListControl
    {
        public SupplierLazyLoadingList()
        {
            InitializeComponent();
        }

        private void ClickeableItemButton_Click(object sender, RoutedEventArgs e)
        {
            var detail = new SupplierDetail((int)((Button)sender).DataContext);
            bool? ret = ZeroMessageBox.Show(detail, "Editar proveedor");
            if (ret.HasValue && ret.Value)
            {
                
            }
        }
        
        
    }
}
