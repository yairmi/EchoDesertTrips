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
                    bool bIsNew = _editedNewCustomer == false;
                    CustomerUpdated?.Invoke(this, new CustomerEventArgs(Customer, bIsNew));
                }
                else
                {
                    CustomerUpdated?.Invoke(this, new CustomerEventArgs(Customer, false));
                }
                CreateNewCustomer();
            }
            CustomersLeft = GetCustomerLeft(Reservation);
        }

        private void CreateNewCustomer()
        {
            Customer = null;

            ControllEnabled = GetCustomerLeft(Reservation) > 0;// || _customer != null;
            if (ControllEnabled)
            {
                Customer = new CustomerWrapper();
                _editedNewCustomer = false;
                Customer.CleanAll();
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
            //Customer = null;
            //Customer = new CustomerWrapper();
            CreateNewCustomer();
        }

        //private void OnExitWithoutSavingCommand(object obj)
        //{
        //    if (IsCustomerDirty())
        //    {
        //        var result = _messageDialogService.ShowOkCancelDialog((string)Application.Current.FindResource("AreYouSureMessage"), "Question");
        //        if (result == MessageDialogResult.CANCEL)
        //            return;
        //    }
        //    CustomerCancelled?.Invoke(this, new CustomerEventArgs(null, true));
        //}
        private bool _lastDertinessValue = false;
        private bool IsCustomerDirty()
        {
            var bDirty = Customer != null ? Customer.IsAnythingDirty() : false;
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
            ControllEnabled = GetCustomerLeft(Reservation) > 0;// || _customer != null;

            //var config = new MapperConfiguration(cfg =>
            //{
            //    cfg.CreateMap<CustomerWrapper, CustomerWrapper>();
            //});

            //if (_customerWrapper != null)
            //{

            //    IMapper iMapper = config.CreateMapper();
            //    Customer = iMapper.Map<CustomerWrapper, CustomerWrapper>(_customerWrapper);
            //}
            //else
            //    Customer = new CustomerWrapper();
            //CustomersLeft = GetCustomerLeft(_currentReservation);
            //CleanAll();
        }

        public void SetCustomer(CustomerWrapper customer)
        {
            if (customer != null)
            {
                ControllEnabled = true;
                _editedNewCustomer = customer.CustomerId == 0;
                var config = new MapperConfiguration(cfg => {
                    cfg.CreateMap<CustomerWrapper, CustomerWrapper>();
                });
                IMapper iMapper = config.CreateMapper();
                Customer = iMapper.Map<CustomerWrapper, CustomerWrapper>(customer);
            }
            else
            {
                _editedNewCustomer = false;
                Customer = new CustomerWrapper();
            }

            Customer.CleanAll();

        }

        private bool _editedNewCustomer;

        private CustomerWrapper _customer;

        public CustomerWrapper Customer
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
                _controllEnabled = value;
                OnPropertyChanged(() => ControllEnabled);
            }
        }

        public event EventHandler<CustomerEventArgs> CustomerUpdated;
        public event EventHandler<CustomerEventArgs> CustomerCancelled;
    }
}
