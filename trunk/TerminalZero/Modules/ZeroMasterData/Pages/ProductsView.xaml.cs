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
            Loaded += new System.Windows.RoutedEventHandler(ProductsView_Loaded);
        }

        void ProductsView_Loaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if(ViewModel!=null)
            {
                CommandBar.AppendButton("Actualizar Precios", ((ProductsViewModel) ViewModel).UpdatePricesCommand);
                ((ProductsViewModel) ViewModel).UpdateProductCommand.Predicate = o => ControlMode == ControlMode.Update;
            }

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
