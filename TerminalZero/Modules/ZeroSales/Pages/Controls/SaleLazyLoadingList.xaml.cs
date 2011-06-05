using System;
using System.ComponentModel;
using System.Data.Objects.DataClasses;
using System.Windows;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;

namespace ZeroSales.Pages.Controls
{
    /// <summary>
    /// Interaction logic for SaleGrid.xaml
    /// </summary>
    public partial class SaleLazyLoadingList : LazyLoadingListControlUpgrade
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
    }
}
