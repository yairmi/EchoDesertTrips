using AutoMapper;
using Core.Common.Contracts;
using Core.Common.Core;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Entities;
using EchoDesertTrips.Desktop.CustomEventArgs;
using EchoDesertTrips.Desktop.Support;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class EditCustomerGridViewModel : ViewModelBase
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IMessageDialogService _messageDialogService;

        [ImportingConstructor]
        public EditCustomerGridViewModel(IServiceFactory serviceFactory,
            IMessageDialogService messageDialogService)
        {
            log.Debug("EditCustomerGridViewModel ctor start");
            _serviceFactory = serviceFactory;
            _messageDialogService = messageDialogService;
            SaveCommand = new DelegateCommand<object>(OnSaveCommand, OnSaveCommandCanExecute);
            ClearCommand = new DelegateCommand<object>(OnClearCommand, OnClearCommandCanExecute);
            log.Debug("EditCustomerGridViewModel ctor end");
        }

        public DelegateCommand<object> SaveCommand { get; }
        public DelegateCommand<object> ClearCommand { get; }

        private bool OnSaveCommandCanExecute(object obj)
        {
            return IsCustomerDirty();
        }

        private bool OnClearCommandCanExecute(object obj)
        {
            return true;    //This is for the case that the user Edit an existing and made no change and after it he want to clear it.
        }

        //After pressing the 'Save' button
        private void OnSaveCommand(object obj)
        {
            if (IsCustomerDirty())
            {
                ValidateModel(true);
            }
            if (Customer.IsValid)
            {
                if (Customer.CustomerId == 0)
                {
                    bool bIsNew = _editedNewAddedCustomer == false;
                    CustomerUpdated?.Invoke(this, new CustomerEventArgs(Customer, bIsNew));
                }
                else
                {
                    CustomerUpdated?.Invoke(this, new CustomerEventArgs(Customer, false));
                }
                CreateCustomer();
            }
            CustomersLeft = ReservationHelper.GetCustomerLeft(Reservation);
            if (CustomersLeft <= 0)
            {
                if (Reservation.Tours.Count() > 0)
                {
                    Reservation.Adults = Reservation.Customers.ToList()
                        .Where((customer) => Reservation.Tours[0].StartDate.Subtract(customer.DateOfBirdth).TotalDays >= 4380).Count();
                    Reservation.Childs = Reservation.Customers.ToList()
                        .Where((customer) => Reservation.Tours[0].StartDate.Subtract(customer.DateOfBirdth).TotalDays < 4380 &&
                        Reservation.Tours[0].StartDate.Subtract(customer.DateOfBirdth).TotalDays >= 730).Count();
                    Reservation.Infants = Reservation.Customers.Count() - Reservation.Adults - Reservation.Childs;
                }
            }
        }

        private bool _editedNewAddedCustomer;

        public void CreateCustomer(Customer customer = null)
        {
            if (customer != null)
            {
                Customer = CustomerHelper.CloneCustomer(customer);
                _editedNewAddedCustomer = Customer.CustomerId == 0;
                ControllEnabled = true;
            }
            else
            {
                Customer = null;
                GC.Collect();
                log.Debug("GC Customer. Before WaitForPendingFinalizers");
                GC.WaitForPendingFinalizers();
                log.Debug("GC Customer. After WaitForPendingFinalizers");
                ControllEnabled = ReservationHelper.GetCustomerLeft(Reservation) > 0;
                if (_controllEnabled)
                {
                    Customer = new Customer();
                    Customer.CleanAll();
                }
                _editedNewAddedCustomer = false;
            }
        }

        private void OnClearCommand(object obj)
        {
            if (IsCustomerDirty())
            {
                var result = _messageDialogService.ShowOkCancelDialog((string)Application.Current.FindResource("AreYouSureMessage"), "Question");
                if (result == MessageDialogResult.CANCEL)
                    return;
            }
            CreateCustomer();
        }

        private bool _lastDertinessValue = false;
        private bool IsCustomerDirty()
        {
            var bDirty = Customer != null ? Customer.IsAnythingDirty() && (!ViewMode) : false;
            if (bDirty != _lastDertinessValue)
            {

                log.Debug("EditCustomerGridViewModel dirty = " + bDirty);
                _lastDertinessValue = bDirty;
            }
            return bDirty;
        }

        protected override void AddModels(List<ObjectBase> models)
        {
            models.Add(Customer);
        }

        protected override void OnViewLoaded()
        {
            ControllEnabled = ReservationHelper.GetCustomerLeft(Reservation) > 0;
            if (ControllEnabled)
                CreateCustomer();
        }

        private Customer _customer;

        public Customer Customer
        {
            get
            {
                return _customer;
            }
            set
            {
                _customer = value;
                OnPropertyChanged(() => Customer, true);
            }
        }

        private int _customerLeft;

        public int CustomersLeft
        {
            get
            {
                return _customerLeft;
            }
            set
            {
                if (_customerLeft != value)
                {
                    _customerLeft = value;
                    OnPropertyChanged(() => CustomersLeft);
                }
            }
        }

        private bool _controllEnabled;

        public bool ControllEnabled
        {
            get
            {
                return _controllEnabled;
            }
            set
            {
                if (_controllEnabled != value)
                {
                    _controllEnabled = value;
                    OnPropertyChanged(() => ControllEnabled);
                }
            }
        }

        public event EventHandler<CustomerEventArgs> CustomerUpdated;
    }
}
