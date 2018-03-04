using Core.Common;
using Core.Common.Contracts;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using EchoDesertTrips.Desktop.Support;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.ServiceModel;

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CustomersViewModel : ViewModelBase
    {
        private readonly IServiceFactory _serviceFactory;
        private ObservableCollection<Customer> _customers;
        private EditCustomerViewModel _editCustomerViewModel;

        [ImportingConstructor]
        public CustomersViewModel(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
            EditCustomerCommand = new DelegateCommand<Customer>(OnEditCommand);
            DeleteCustomerCommand = new DelegateCommand<Customer>(OnDeleteCommand);
            AddCustomerCommand = new DelegateCommand<object>(OnAddCommand);
        }

        [Import]
        public EditCustomerViewModel EditCustomerViewModel { get; set; }

        public override string ViewTitle
        {
            get { return "Customers"; }
        }

        public ObservableCollection<Customer> Customers
        {
            get
            {
                return _customers;
            }

            set
            {
                if (value != _customers)
                {
                    _customers = value;
                    OnPropertyChanged(() => Customers, false);
                }
            }
        }

        public EditCustomerViewModel CurrentCustomerViewModel
        {
            get { return _editCustomerViewModel; }
            set
            {
                if (_editCustomerViewModel != value)
                {
                    _editCustomerViewModel = value;
                    OnPropertyChanged(() => CurrentCustomerViewModel, false);
                }
            }
        }

        public DelegateCommand<Customer> EditCustomerCommand { get; private set; }
        public DelegateCommand<object> AddCustomerCommand { get; private set; }
        public DelegateCommand<Customer> DeleteCustomerCommand { get; private set; }

        public event CancelEventHandler ConfirmDelete;
        public event EventHandler<ErrorMessageEventArgs> ErrorOccured;

        protected override void OnViewLoaded()
        {
            _customers = new ObservableCollection<Customer>();
            //ICustomerService
            WithClient<ICustomerService>(_serviceFactory.CreateClient<ICustomerService>(), inventoryClient =>
            {
                Customer[] customers = inventoryClient.GetAllCustomers();
                if (customers != null)
                {
                    foreach (var customer in customers)
                    {
                        _customers.Add(customer);
                    }
                }
            });
        }

        private void OnAddCommand(object arg)
        {
            EditCustomerViewModel.Customer = new Customer();
            EditCustomerViewModel.CustomerUpdated -= CurrentCustomerViewModel_CustomerUpdated;
            EditCustomerViewModel.CustomerUpdated += CurrentCustomerViewModel_CustomerUpdated;
            EditCustomerViewModel.CustomerCancelled -= CurrentCustomerViewModel_CustomerCancelled;
            EditCustomerViewModel.CustomerCancelled += CurrentCustomerViewModel_CustomerCancelled;
            CurrentCustomerViewModel = EditCustomerViewModel;
        }

        private void OnEditCommand(Customer customer)
        {
            EditCustomerViewModel.Customer = customer;
            EditCustomerViewModel.CustomerUpdated -= CurrentCustomerViewModel_CustomerUpdated;
            EditCustomerViewModel.CustomerUpdated += CurrentCustomerViewModel_CustomerUpdated;
            EditCustomerViewModel.CustomerCancelled -= CurrentCustomerViewModel_CustomerCancelled;
            EditCustomerViewModel.CustomerCancelled += CurrentCustomerViewModel_CustomerCancelled;
            CurrentCustomerViewModel = EditCustomerViewModel;
        }

        private void OnDeleteCommand(Customer Customer)
        {
            //Below is the argument that is going to be carried by the CancelEventHandler
            var args = new CancelEventArgs();
            //Rasie the event
            if (ConfirmDelete != null)
                ConfirmDelete(this, args);
            //If the cancel property of args come back with false then continue with the deletion
            if (!args.Cancel)
            {
                //TODO : Add it to all methods
                try
                {
                    //ICustomerService
                    WithClient<ICustomerService>(_serviceFactory.CreateClient<ICustomerService>(), inventoryClient =>
                    {
                        inventoryClient.DeleteCustomer(Customer);
                        _customers.Remove(Customer);
                    });
                }
                catch (FaultException ex)
                {
                    if (ErrorOccured != null)
                        ErrorOccured(this, new ErrorMessageEventArgs(ex.Message));
                }
                catch (Exception ex)
                {
                    if (ErrorOccured != null)
                        ErrorOccured(this, new ErrorMessageEventArgs(ex.Message));
                }
            }
        }

        private void CurrentCustomerViewModel_CustomerUpdated(object sender, CustomerEventArgs e)
        {
            if (!e.IsNew)
            {
                //This is done in order to update the Grid. Remember that in EditCustomerViewModel the updated Customer
                //Is a temporary object and it is not part of the Grid collection Customers.
                var Customer = Customers.FirstOrDefault(item => item.CustomerId == e.Customer.CustomerId);
                if (Customer != null)
                {
                    Customer.CustomerId = e.Customer.CustomerId;
                    Customer.FirstName = e.Customer.FirstName;
                    Customer.LastName = e.Customer.LastName;
                    Customer.Phone1 = e.Customer.Phone1;
                    Customer.Phone2 = e.Customer.Phone2;
                    Customer.DateOfBirdth = e.Customer.DateOfBirdth;
                    Customer.PassportNumber = e.Customer.PassportNumber;
                    Customer.IssueData = e.Customer.IssueData;
                    Customer.ExpireyDate = e.Customer.ExpireyDate;
                    Customer.NationalityId = e.Customer.NationalityId;
                    Customer.Nationality = e.Customer.Nationality;
                    Customer.HasVisa = e.Customer.HasVisa;
                    Customer.IdentityId = e.Customer.IdentityId;
                }
            }
            else
            {
                //var cw = new CustomerWrapper();
                //var customer = new Customer();
                //cw.UpdateCustomer(e.Customer, customer);
                //Customers.Add(customer);
            }

            CurrentCustomerViewModel = null;
        }



        private void CurrentCustomerViewModel_CustomerCancelled(object sender, EventArgs e)
        {
            CurrentCustomerViewModel = null;
        }
    }
}
