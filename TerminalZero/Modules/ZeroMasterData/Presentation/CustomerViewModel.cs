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

        private CustomerDetailViewModel selectedItem;

        public CustomerDetailViewModel SelectedItem
        {
            get { return selectedItem; }
            set
            {
                if (selectedItem != value)
                {
                    selectedItem = value;
                    OnPropertyChanged("SelectedItem");
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
                    BusinessContext.Instance.Model.Customers.Select(
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
            var query = from customer in BusinessContext.Instance.Model.Customers
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
            viewmodel.Customer = Customer.CreateCustomer(BusinessContext.Instance.Model.GetNextCustomerCode(), 0, true);
            if (viewmodel.View.ShowInModalWindow())
            {
                try
                {
                    BusinessContext.Instance.Model.AddToCustomers(viewmodel.Customer);
                    CustomerList.Add(new CustomerDetailViewModel {Customer = viewmodel.Customer });
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
