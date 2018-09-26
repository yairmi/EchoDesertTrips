using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Common.Contracts;
using Core.Common.Core;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Entities;
using EchoDesertTrips.Desktop.Support;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows;
using System.Linq;
using AutoMapper;
using System.ComponentModel.Composition;
using EchoDesertTrips.Desktop.CustomEventArgs;

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class CustomerGridViewModel : ViewModelBase
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IMessageDialogService _messageDialogService;

        [ImportingConstructor]
        public CustomerGridViewModel(IServiceFactory serviceFactory,
            IMessageDialogService messageBoxDialogService)
        {
            _serviceFactory = serviceFactory;
            _messageDialogService = messageBoxDialogService;
            RowEditEndingCommand = new DelegateCommand<Customer>(OnRowEditEndingCommand);
            RowSomeEventCommand = new DelegateCommand<Customer>(OnRowSomeEventCommand);
            DeleteCustomerCommand = new DelegateCommand<Customer>(OnDeleteCustomerCommand);
            EditCustomerCommand = new DelegateCommand<Customer>(OnEditCustomerCommand);
            Customers = new ObservableCollection<Customer>();
        }

        public CustomerGridViewModel()
        {

        }

        public override string ViewTitle => "PAX";

        public DelegateCommand<Customer> DeleteCustomerCommand { get; set; }

        private void OnDeleteCustomerCommand(Customer customer)
        {
            var result = _messageDialogService.ShowOkCancelDialog((string)Application.Current.FindResource("ShortAreYouSureMessage"), "Question");
            if (result == MessageDialogResult.CANCEL)
                return;
            Customers.Remove(customer);
        }
        //Remove CustomerWrapper
        public DelegateCommand<Customer> RowSomeEventCommand { get; set; }

        private void OnRowSomeEventCommand(Customer customerWrraper)
        {
            if (customerWrraper.IsDirty)
            {
                if (customerWrraper.IsValid)
                {
                }
            }
        }
        //Remove CustomerWrapper
        private Customer _lastUpdatedCustomer;

        private void OnEditCustomerCommand(Customer customer)
        {
            customer.bInEdit = true;
            _editCustomerViewModel.SetCustomer(customer);
            _editCustomerViewModel.Reservation = Reservation;
            CurrentCustomerViewModel = _editCustomerViewModel;
            RegisterEvents();
        }

        public DelegateCommand<Customer> EditCustomerCommand { get; }
        [Import]
        private EditCustomerGridViewModel _editCustomerViewModel { get;set;}

        public EditCustomerGridViewModel CurrentCustomerViewModel
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

        public DelegateCommand<Customer> RowEditEndingCommand { get; set; }

        private void OnRowEditEndingCommand(Customer customer)
        {
            if (customer.IsDirty)
            {
                _lastUpdatedCustomer = customer;
                ValidateModel();
                if (customer.IsValid)
                {
                }
            }
        }

        private ObservableCollection<Customer> _customers;

        public ObservableCollection<Customer> Customers
        {
            get { return _customers; }
            private set
            {
                _customers = value;
                OnPropertyChanged(()=> Customers, false);
            }
        }

        //Provide ViewModelBase model or what properties inside 
        //The ViewModel. ValidateModel will preform a check on all registered models/properties
        protected override void AddModels(List<ObjectBase> models)
        {
            models.Add(_lastUpdatedCustomer);
        }

        public void ValidateCurrentModel()
        {
            ValidateModel();
        }

        protected override void OnViewLoaded()
        {
            Customers = Reservation.Customers;
            _editCustomerViewModel.Reservation = Reservation;
            _editCustomerViewModel.SetCustomer(null);
            _editCustomerViewModel.CustomersLeft = ReservationHelper.GetCustomerLeft(Reservation);
            CurrentCustomerViewModel = _editCustomerViewModel;
            RegisterEvents();
        }

        private void CurrentCustomerViewModel_CustomerUpdated(object sender, CustomerEventArgs e)
        {
            //var config = new MapperConfiguration(cfg => {
            //    cfg.CreateMap<CustomerWrapper, CustomerWrapper>();
            //});

            //var iMapper = config.CreateMapper();
            //var customerWrapper = iMapper.Map<CustomerWrapper, CustomerWrapper>(e.Customer);
            var customer_e = e.Customer;
            if (!e.IsNew)
            {
                //This is done in order to update the Grid. Remember that in EditTripViewModel the updated trip
                //Is a temporary object and it is not part of the Grid collection trips.
                customer_e.bInEdit = false;
                var customer = Customers.FirstOrDefault(item => item.bInEdit == true);
                if (customer != null)
                {
                    var index = Customers.IndexOf(customer);
                    Customers[index] = customer_e;
                }
            }
            else
            {
                Customers.Add(customer_e);
            }
            //CustomerUpdatedFinished?.Invoke(this, new EventArgs());
        }

        //private void CurrentCustomerViewModel_CustomerCancelled(object sender, CustomerEventArgs e)
        //{
        //    CurrentCustomerViewModel = null;
        //}

        private void RegisterEvents()
        {
            CurrentCustomerViewModel.CustomerUpdated -= CurrentCustomerViewModel_CustomerUpdated;
            CurrentCustomerViewModel.CustomerUpdated += CurrentCustomerViewModel_CustomerUpdated;
            //CurrentCustomerViewModel.CustomerCancelled -= CurrentCustomerViewModel_CustomerCancelled;
            //CurrentCustomerViewModel.CustomerCancelled += CurrentCustomerViewModel_CustomerCancelled;
        }

        public event EventHandler CustomerUpdatedFinished;
    }
    public class CustomerValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value,
            System.Globalization.CultureInfo cultureInfo)
        {
            Customer customer = (value as BindingGroup).Items[0] as Customer;
            if (customer.FirstName == string.Empty)
            {
                return new ValidationResult(false,
                    "Customer First Name should not be empty.");
            }
            else
                if (customer.LastName == string.Empty)
            {
                return new ValidationResult(false,
                    "Customer Last Name should not be empty.");
            }
            else
                if (customer.DateOfBirdth > DateTime.Now)
            {
                return new ValidationResult(false,
                     "Date Of Birdth should not be after Today.");
            }
            else
                if (customer.PassportNumber == string.Empty)
            {
                return new ValidationResult(false,
                    "Passport number should not be empty.");
            }
            else
                if (customer.IssueData < customer.DateOfBirdth)
            {
                return new ValidationResult(false,
                    "Passport Issue Date should not be before Date Of Birdth.");
            }
            else 
                if (customer.IssueData > customer.ExpireyDate)
            {
                return new ValidationResult(false,
                     "Passport Issue Date should not be After Passport Expiry Date.");
            }
            else
                if (customer.ExpireyDate < customer.DateOfBirdth)
            {
                return new ValidationResult(false,
                    "Passport Expiry Date should not be before Date Of Birdth.");
            }
            else
                if (customer.ExpireyDate < customer.IssueData)
            {
                return new ValidationResult(false,
                     "Passport Expiry Date should not be before Passport Issue Date.");
            }
            else
                if (customer.Phone1 == string.Empty)
            {
                return new ValidationResult(false,
                     "Phone Number should not be empty.");
            }
            else
                if (PhoneNumberValidation.IsPhoneNumber(customer.Phone1) == false)
            {
                return new ValidationResult(false,
                     "Phone Number is not in the correct format.");
            }
            else
            {
                var validationResult = ValidationResult.ValidResult;
                return validationResult;
            }
        }
    }
}
