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
using System.ComponentModel.Composition;
using Core.Common.UI.CustomEventArgs;
using Core.Common.UI.PubSubEvent;

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
            DeleteCustomerCommand = new DelegateCommand<Customer>(OnDeleteCustomerCommand);
            EditCustomerCommand = new DelegateCommand<Customer>(OnEditCustomerCommand);
            _eventAggregator.GetEvent<ReservationEditedEvent>().Subscribe(ReservationEdited);
            _eventAggregator.GetEvent<CustomerUpdatedEvent>().Subscribe(CustomerUpdated);
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
            Reservation.PropertyDeleted = true;
        }

        private void OnEditCustomerCommand(Customer customer)
        {
            customer.bInEdit = true;
            CurrentCustomerViewModel = _editCustomerGridViewModel;
            _eventAggregator.GetEvent<CustomerEditedEvent>().Publish(customer);
        }

        public DelegateCommand<Customer> EditCustomerCommand { get; }
        [Import]
        private EditCustomerGridViewModel _editCustomerGridViewModel { get;set;}

        public EditCustomerGridViewModel CurrentCustomerViewModel
        {
            get { return _editCustomerGridViewModel; }
            set
            {
                if (_editCustomerGridViewModel != value)
                {
                    _editCustomerGridViewModel = value;
                    OnPropertyChanged(() => CurrentCustomerViewModel, false);
                }
            }
        }

        public ObservableCollection<Customer> Customers { get; set; }
        //{
        //    get { return _customers; }
        //    private set
        //    {
        //        _customers = value;
        //        OnPropertyChanged(() => Customers, false);
        //    }
        //}

        protected override void OnViewLoaded()
        {
            Customers = Reservation.Customers;
            CurrentCustomerViewModel = _editCustomerGridViewModel;
        }

        public override void OnViewUnloaded()
        {
            ;
        }

        private void ReservationEdited(EditReservationEventArgs e)
        {
            Reservation = e.Reservation;
            ViewMode = e.ViewMode;
        }

        private void CustomerUpdated(CustomerEventArgs e)
        {
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
        }
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
