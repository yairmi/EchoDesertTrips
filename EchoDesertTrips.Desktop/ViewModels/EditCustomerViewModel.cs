using Core.Common.Contracts;
using Core.Common.UI.Core;
using System;
using System.Collections.Generic;
using Core.Common.Core;
using System.Collections.ObjectModel;
using System.Linq;
using System.ComponentModel.Composition;
using EchoDesertTrips.Client.Entities;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Desktop.Support;

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class EditCustomerViewModel : ViewModelBase
    {
        public event EventHandler<CustomerEventArgs> CustomerUpdated;
        public event EventHandler CustomerCancelled;
        private readonly IServiceFactory _serviceFactory;
        private Customer _customer;

        [ImportingConstructor]
        public EditCustomerViewModel(IServiceFactory serviceFactory/*, Customer Customer*/)
        {
            _serviceFactory = serviceFactory;
            SaveCommand = new DelegateCommand<object>(OnSaveCommand, OnSaveCommandCanExecute);
            CancelCommand = new DelegateCommand<object>(OnCancelCommand);
            CustomerViewModel = new CustomerViewModel(_serviceFactory);

        }

        private bool OnSaveCommandCanExecute(object obj)
        {
            return CustomerViewModel.Customer.IsDirty;
        }

        private void OnCancelCommand(object obj)
        {
            CustomerCancelled?.Invoke(this, EventArgs.Empty);
        }

        private void OnSaveCommand(object obj)
        {
            //ValidateModel();
            CustomerViewModel.ValidateCurrentModel();
            if (CustomerViewModel.IsValid)
            {
                //ICustomerService
                WithClient<ICustomerService>(_serviceFactory.CreateClient<ICustomerService>(), customerClient =>
                {
                    var isNewCustomer = (CustomerViewModel.Customer.CustomerId == 0);
                    //var cw = new CustomerWrapper();
                    //cw.UpdateCustomer(CustomerViewModel.Customer, Customer);
                    var savedCustomer = customerClient.UpdateCustomer(/*CustomerViewModel.Customer*/Customer); //Update or Add
                    if (savedCustomer != null)
                    {
                        CustomerViewModel.Customer.CustomerId = savedCustomer.CustomerId;
                        CustomerUpdated?.Invoke(this, new CustomerEventArgs(CustomerViewModel.Customer, isNewCustomer));
                    }
                });
            }
        }

        public DelegateCommand<object> SaveCommand { get; private set; }
        public DelegateCommand<object> CancelCommand { get; private set; }

        public CustomerViewModel CustomerViewModel { get; set; }

        public Customer Customer
        {
            get
            {
                return _customer;
            }
            set
            {
                _customer = value;
                CustomerViewModel.SetCustomer(_customer);
            }
        }

        /*private CustomerWrapper _customer;

        public CustomerWrapper Customer
        {
            get { return _customer; }
            set
            {
                OnPropertyChanged(() => Customer, false);
            }
        }

        private Customer _editedCustomer;
        public void SetCustomer(Customer customer)
        {
            _editedCustomer = customer;
            Customer = CustomerWrapper.Clone(customer);
        }*/
    }
}