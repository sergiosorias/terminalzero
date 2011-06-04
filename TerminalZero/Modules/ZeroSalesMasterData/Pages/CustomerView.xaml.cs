﻿using ZeroGUI;

namespace ZeroMasterData.Pages
{
    /// <summary>
    /// Interaction logic for SupplierLazyLoadingList.xaml
    /// </summary>
    public partial class CustomerView : NavigationBasePage
    {
        public CustomerView()
        {
            InitializeComponent();
        }

        protected override void OnControlModeChanged(ZeroCommonClasses.Interfaces.ControlMode newMode)
        {
            base.OnControlModeChanged(newMode);
            customerGrid.ControlMode = ControlMode;
        }
    
    }
}
