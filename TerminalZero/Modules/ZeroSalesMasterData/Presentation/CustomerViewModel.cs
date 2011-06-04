using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using ZeroBusiness.Entities.Data;
using ZeroBusiness.Manager.Data;
using ZeroCommonClasses.GlobalObjects.Actions;
using ZeroCommonClasses.Interfaces;
using ZeroGUI;
using ZeroGUI.Reporting;
using ZeroMasterData.Pages;
using ZeroMasterData.Pages.Controls;
using ZeroMasterData.Properties;

namespace ZeroMasterData.Presentation
{
    public class CustomerViewModel : ViewModelGui
    {
        #region Binding Properties
        private ObservableCollection<CustomerDetailViewModel> customerList;

        public ObservableCollection<CustomerDetailViewModel> CustomerList
        {
            get { return customerList; }
            set
            {
                if (customerList != value)
                {
                    customerList = value;
                    OnPropertyChanged("CustomerList");
                }
            }
        }

        private Customer selectedCustomer;

        public Customer SelectedCustomer
        {
            get { return selectedCustomer; }
            set
            {
                if (selectedCustomer != value)
                {
                    selectedCustomer = value;
                    OnPropertyChanged("SelectedCustomer");
                }
            }
        }
        #endregion

        #region Commands
        private ICommand searchCommand;

        public ICommand SearchCommand
        {
            get { return searchCommand; }
            set
            {
                if (searchCommand != value)
                {
                    searchCommand = value;
                    OnPropertyChanged("SearchCommand");
                }
            }
        }

        private ICommand createCommand;

        public ICommand CreateCommand
        {
            get { return createCommand; }
            set
            {
                if (createCommand != value)
                {
                    createCommand = value;
                    OnPropertyChanged("CreateCommand");
                }
            }
        }
        #endregion

        public CustomerViewModel()
            : base(new CustomerView())
        {
            CustomerList = new ObservableCollection<CustomerDetailViewModel>(
                    BusinessContext.Instance.ModelManager.Customers.Select(
                                                    customer => new CustomerDetailViewModel
                                                    {
                                                        Customer = customer
                                                    }
                                                  ));
            SearchCommand = new ZeroActionDelegate(SearchCustomer);
            CreateCommand = new ZeroActionDelegate(CreateCustomer, (o) => View.ControlMode.HasFlag(ControlMode.Update));
        }

        protected override void PrintCommandExecution(object parameter)
        {
            base.PrintCommandExecution(parameter);
            var query = from customer in BusinessContext.Instance.ModelManager.Customers
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

        private void CreateCustomer(object parameter)
        {
            var viewmodel = new CustomerDetailViewModel(new CustomerDetail());
            viewmodel.Customer = Customer.CreateCustomer(BusinessContext.Instance.ModelManager.GetNextCustomerCode(), 0, true);
            if (viewmodel.View.ShowInModalWindow().GetValueOrDefault())
            {
                try
                {
                    BusinessContext.Instance.ModelManager.AddToCustomers(viewmodel.Customer);
                    ((CustomerView)View).customerGrid.AddItem(viewmodel.Customer);
                }
                catch (Exception wx)
                {
                    MessageBox.Show(wx.ToString(),"Error",MessageBoxButton.OK,MessageBoxImage.Error);
                    Trace.TraceError("Error updating Customer {0}", viewmodel.Customer.Name1);
                }
            }
        }

        private void SearchCustomer(object parameter)
        {
            var e = parameter as SearchCriteriaEventArgs;
            if (e != null) ((CustomerView)View).customerGrid.ApplyFilter(e.Criteria);
        }
    }
}
