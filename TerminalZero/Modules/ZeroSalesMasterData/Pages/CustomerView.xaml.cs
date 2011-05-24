using System;
using System.Diagnostics;
using System.Dynamic;
using System.Windows;
using ZeroBusiness.Entities.Data;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ZeroGUI.Reporting;
using System.Linq;
using ZeroMasterData.Pages.Controls;

namespace ZeroMasterData.Pages
{
    /// <summary>
    /// Interaction logic for SupplierLazyLoadingList.xaml
    /// </summary>
    public partial class CustomerView : NavigationBasePage
    {
        public CustomerView()
        {
            ControlMode = ControlMode.ReadOnly;
            InitializeComponent();
            CommandBar.New += toolbar_New;
            CommandBar.Print += CommandBar_Print;
        }

        private void SearchBox_Search(object sender, SearchCriteriaEventArgs e)
        {
            customerGrid.ApplyFilter(e.Criteria);
        }

        private void toolbar_New(object sender, RoutedEventArgs e)
        {
            var detail = new CustomerDetail();
            bool? ret = ZeroMessageBox.Show(detail, Properties.Resources.CustomerNew, ResizeMode.NoResize, MessageBoxButton.OKCancel);
            if (ret.HasValue && ret.Value)
            {
                try
                {
                    customerGrid.AddItem(detail.CurrentCustomer);
                }
                catch (Exception wx)
                {
                    MessageBox.Show(wx.ToString());
                    Trace.TraceError("Error updating Customer {0}", detail.CurrentCustomer);
                }
            }
        }

        private void CommandBar_Print(object sender, RoutedEventArgs e)
        {
            var query = from customer in ZeroBusiness.Manager.Data.BusinessContext.Instance.ModelManager.Customers
                        orderby customer.Name1
                        select new
                                   {
                                       Nombre = customer.Name1,
                                       CUIT = customer.LegalCode,
                                       Dirección = customer.Street + "  " + customer.Number,
                                       Teléfono = customer.Telephone1,
                                       Celular = customer.Telephone2
                                   };
            
            ReportBuilder.Create("Listado de Clientes", query.ToList());
        }

       
    }
}
