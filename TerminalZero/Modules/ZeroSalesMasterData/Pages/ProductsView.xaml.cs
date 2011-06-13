using System;
using System.Collections.ObjectModel;
using ZeroBusiness.Entities.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ZeroMasterData.Presentation;

namespace ZeroMasterData.Pages
{
    /// <summary>
    /// Interaction logic for Product.xaml
    /// </summary>
    public partial class ProductsView : NavigationBasePage
    {
        public ProductsView()
        {
            InitializeComponent();
        }
        
        protected override void OnControlModeChanged(ControlMode newMode)
        {
 	         base.OnControlModeChanged(newMode);
             productList.ControlMode = ControlMode;
        }

        private void SearchBox_Search(object sender, SearchCriteriaEventArgs e)
        {
            e.Matches = productList.ApplyFilter(e.Criteria);
        }
        
    }
}
