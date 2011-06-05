using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.GlobalObjects.Barcode;
using ZeroGUI;
using ZeroGUI.Classes;
using ZeroSales.Presentation;
using ZeroSales.Printer;

namespace ZeroSales.Pages
{
    /// <summary>
    /// Interaction logic for CreateSaleView.xaml
    /// </summary>
    public partial class CreateSaleView : NavigationBasePage
    {
        public CreateSaleView()
        {
            InitializeComponent();
            CommandBar.TabIndex = 3;
        }
    }
}
