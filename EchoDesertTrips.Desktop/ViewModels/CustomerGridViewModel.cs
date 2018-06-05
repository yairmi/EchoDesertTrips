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

namespace EchoDesertTrips.Desktop.ViewModels
{
    public class CustomerGridViewModel : ViewModelBase
    {

        private readonly IServiceFactory _serviceFactory;
        private readonly IMessageDialogService _messageDialogService;
        private readonly ReservationWrapper _currentReservation;
        private bool _editZeroCustomerId = false;
        //Remove CustomerWrapper
        public CustomerGridViewModel(IServiceFactory serviceFactory,
            IMessageDialogService messageBoxDialogService,
            ReservationWrapper currentReservation)
        {
            _serviceFactory = serviceFactory;
            _messageDialogService = messageBoxDialogService;
            RowEditEndingCommand = new DelegateCommand<CustomerWrapper>(OnRowEditEndingCommand);
            RowSomeEventCommand = new DelegateCommand<CustomerWrapper>(OnRowSomeEventCommand);
            DeleteCustomerCommand = new DelegateCommand<CustomerWrapper>(OnDeleteCustomerCommand);
            EditCustomerCommand = new DelegateCommand<CustomerWrapper>(OnEditCustomerCommand);
            AddCustomerCommand = new DelegateCommand<object>(OnAddCustomerCommand);
            Customers = new ObservableCollection<CustomerWrapper>();
            _currentReservation = currentReservation;
            Customers = currentReservation.Customers;
            //_totalCustomers = 0;
        }

        public CustomerGridViewModel()
        {
            
        }


        public DelegateCommand<CustomerWrapper> DeleteCustomerCommand { get; set; }

        private void OnDeleteCustomerCommand(CustomerWrapper customer)
        {
            var result = _messageDialogService.ShowOkCancelDialog((string)Application.Current.FindResource("ShortAreYouSureMessage"), "Question");
            if (result == MessageDialogResult.CANCEL)
                return;
            Customers.Remove(customer);
        }
        //Remove CustomerWrapper
        public DelegateCommand<CustomerWrapper> RowSomeEventCommand { get; set; }

        private void OnRowSomeEventCommand(CustomerWrapper customerWrraper)
        {
            if (customerWrraper.IsDirty)
            {
                if (customerWrraper.IsValid)
                {
                }
            }
        }
        //Remove CustomerWrapper
        private CustomerWrapper _lastUpdatedCustomer;

        private void OnAddCustomerCommand(object obj)
        {
            if (GetCustomerLeft(_currentReservation) > 0)
            {
                CurrentCustomerViewModel = new EditCustomerGridViewModel(_serviceFactory, _messageDialogService, null, _currentReservation);
                RegisterEvents();
                ShowPopupWindow("Add PAX");
            }
            else
            {
                _messageDialogService.ShowInfoDialog((string)Application.Current.FindResource("YouMustAddCustomersToToursHotels"), "Information");
            }
        }

        private void OnEditCustomerCommand(CustomerWrapper customer)
        {
            if (customer.CustomerId == 0)
            {
                customer.CustomerId = Customers.Max(x => x.CustomerId) + 1;
                _editZeroCustomerId = true;
            }
            else
                _editZeroCustomerId = false;
            CurrentCustomerViewModel = new EditCustomerGridViewModel(_serviceFactory, _messageDialogService, customer, _currentReservation);
            RegisterEvents();
            ShowPopupWindow("Edit PAX");
        }

        public DelegateCommand<CustomerWrapper> EditCustomerCommand { get; }
        public DelegateCommand<object> AddCustomerCommand { get; }

        private EditCustomerGridViewModel _editCustomerViewModel;

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

        public DelegateCommand<CustomerWrapper> RowEditEndingCommand { get; set; }

        private void OnRowEditEndingCommand(CustomerWrapper customer)
        {
            if (customer.IsDirty)
            {
                _lastUpdatedCustomer = customer;
                ValidateModel();
                if (customer.IsValid)
                {
                    /*WithClient<ICustomerService>(_serviceFactory.CreateClient<ICustomerService>(), customerClient =>
                    {
                        var cw = new CustomerWrapper();
                        var customer = new Customer();
                        cw.UpdateCustomer(customerWrraper, customer);
                        var savedCustomer = customerClient.UpdateCustomer(customer); //Update or Add
                        customerWrraper.CustomerId = savedCustomer.CustomerId;
                    });*/
                }
            }
        }
        //Remove CustomerWrapper
        private ObservableCollection<CustomerWrapper> _customers;

        public ObservableCollection<CustomerWrapper> Customers
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
        }

        private void CurrentCustomerViewModel_CustomerUpdated(object sender, CustomerEventArgs e)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<CustomerWrapper, CustomerWrapper>();
            });

            var iMapper = config.CreateMapper();
            var customerWrapper = iMapper.Map<CustomerWrapper, CustomerWrapper>(e.Customer);

            if (!e.IsNew || _editZeroCustomerId == true)
            {
                //This is done in order to update the Grid. Remember that in EditTripViewModel the updated trip
                //Is a temporary object and it is not part of the Grid collection trips.
                var customer = Customers.FirstOrDefault(item => item.CustomerId == e.Customer.CustomerId);
                if (customer != null)
                {
                    var index = Customers.IndexOf(customer);
                    Customers[index] = customerWrapper;
                    if (_editZeroCustomerId == true)
                        customerWrapper.CustomerId = 0;
                }
            }
            else
            {
                Customers.Add(customerWrapper);
            }
            if (GetCustomerLeft(_currentReservation) <= 0)
            {
                _childWindow.Close();
                CurrentCustomerViewModel = null;
            }
        }

        private void CurrentCustomerViewModel_CustomerCancelled(object sender, CustomerEventArgs e)
        {
            CurrentCustomerViewModel = null;
            _childWindow.Close();
        }

        private PopupWindow _childWindow;

        protected void ShowPopupWindow(string title)
        {
            _childWindow = new PopupWindow
            {
                Title = title,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Content = CurrentCustomerViewModel
            };
            _childWindow.ShowDialog();
        }

        private void RegisterEvents()
        {
            CurrentCustomerViewModel.CustomerUpdated -= CurrentCustomerViewModel_CustomerUpdated;
            CurrentCustomerViewModel.CustomerUpdated += CurrentCustomerViewModel_CustomerUpdated;
            CurrentCustomerViewModel.CustomerCancelled -= CurrentCustomerViewModel_CustomerCancelled;
            CurrentCustomerViewModel.CustomerCancelled += CurrentCustomerViewModel_CustomerCancelled;
        }
    }
    public class CustomerValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value,
            System.Globalization.CultureInfo cultureInfo)
        {
            CustomerWrapper customer = (value as BindingGroup).Items[0] as CustomerWrapper;
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
