using Core.Common.Contracts;
using Core.Common.Core;
using static Core.Common.Core.Const;
using Core.Common.UI.Core;
using Core.Common.UI.CustomEventArgs;
using Core.Common.UI.PubSubEvent;
using EchoDesertTrips.Client.Entities;
using EchoDesertTrips.Desktop.Support;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;
using Core.Common.Utils;

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
            _serviceFactory = serviceFactory;
            _messageDialogService = messageDialogService;
            SaveCommand = new DelegateCommand<object>(OnSaveCommand, OnSaveCommandCanExecute);
            ClearCommand = new DelegateCommand<object>(OnClearCommand, OnClearCommandCanExecute);
            _eventAggregator.GetEvent<ReservationEditedEvent>().Subscribe(ReservationEdited);
            _eventAggregator.GetEvent<CustomerEditedEvent>().Subscribe(CustomerEdited);
            _eventAggregator.GetEvent<CustomerDeletedEvent>().Subscribe(CustomerDeleted);
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
                var newCustomer = CustomerHelper.CloneCustomer(Customer);
                if (Customer.CustomerId == 0)
                {
                    _eventAggregator.GetEvent<CustomerUpdatedEvent>().Publish(new CustomerEventArgs(newCustomer, Customer.bInEdit == false));
                }
                else
                {
                    _eventAggregator.GetEvent<CustomerUpdatedEvent>().Publish(new CustomerEventArgs(newCustomer, false));
                }

                CustomersLeft = ReservationHelper.GetCustomerLeft(Reservation);
                ControllEnabled = CustomersLeft > 0;
            }
            //CustomersLeft = ReservationHelper.GetCustomerLeft(Reservation);
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

            ClearCustomer();
        }

        private void OnClearCommand(object obj)
        {
            if (IsCustomerDirty())
            {
                var result = _messageDialogService.ShowOkCancelDialog((string)Application.Current.FindResource("AreYouSureMessage"), "Question");
                if (result == MessageDialogResult.CANCEL)
                    return;
            }
            ClearCustomer();
        }

        private void ClearCustomer()
        {
            PropertySupport.ResetProperties<Customer>(Customer);
            OnPropertyChanged(() => Customer);
            Customer.CleanAll();
        }

        private bool _lastDertinessValue = false;
        private bool IsCustomerDirty()
        {
            var bDirty = Customer != null ? Customer.IsAnythingDirty() && (!ViewMode) : false;
            if (bDirty != _lastDertinessValue)
            {

                log.Debug($"Dirty = {bDirty}");
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
            CustomersLeft = ReservationHelper.GetCustomerLeft(Reservation);
            ControllEnabled = CustomersLeft > 0;
            Customer.CleanAll();
        }

        public void CustomerDeleted(Object obj)
        {
            CustomersLeft = ReservationHelper.GetCustomerLeft(Reservation);
            ControllEnabled = true;
            Customer.CleanAll();
        }

        private void ReservationEdited(EditReservationEventArgs e)
        {
            Reservation = e.Reservation;
            ViewMode = e.ViewMode;
        }

        private void CustomerEdited(Customer customer)
        {
            Customer = CustomerHelper.CloneCustomer(customer);
            ControllEnabled = true;
            Customer.CleanAll();
        }

        private Customer _customer;
        [Import]
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
    }
}
