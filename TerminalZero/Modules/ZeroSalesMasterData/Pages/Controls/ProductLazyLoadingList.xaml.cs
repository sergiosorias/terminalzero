using System.ComponentModel;
using System.Data.Objects.DataClasses;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;

namespace ZeroMasterData.Pages.Controls
{
    /// <summary>
    /// Interaction logic for ProductGrid.xaml
    /// </summary>
    public partial class ProductLazyLoadingList : LazyLoadingListControlUpgrade
    {
        public ProductLazyLoadingList()
        {
            InitializeComponent();
        }

        protected override void OnControlModeChanged(ControlMode newMode)
        {
            base.OnControlModeChanged(newMode);
            activeProductcollumn.Visibility = editItemCollum.Visibility = newMode == ControlMode.ReadOnly ? Visibility.Hidden : Visibility.Visible;
            
        }
        
        
    }
}
