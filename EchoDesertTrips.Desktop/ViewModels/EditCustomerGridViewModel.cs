using AutoMapper;
using Core.Common.Contracts;
using Core.Common.Core;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Entities;
using EchoDesertTrips.Desktop.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace EchoDesertTrips.Desktop.ViewModels
{
    public class EditCustomerGridViewModel : ViewModelBase
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IMessageDialogService _messageDialogService;
        private CustomerWrapper _customerWrapper;
        private ReservationWrapper _currentReservation;

        public EditCustomerGridViewModel(IServiceFactory serviceFactory,
            IMessageDialogService messageDialogService,
            CustomerWrapper customer,
            ReservationWrapper currentReservation)
        {
            _serviceFactory = serviceFactory;
            _messageDialogService = messageDialogService;
            _customerWrapper = customer;
            _currentReservation = currentReservation;
            SaveCommand = new DelegateCommand<object>(OnSaveCommand, OnCommandCanExecute);
            ClearCommand = new DelegateCommand<object>(OnClearCommand, OnClearCanExecute);
            ExitWithoutSavingCommand = new DelegateCommand<object>(OnExitWithoutSavingCommand);
        }

        public DelegateCommand<object> SaveCommand { get; }
        public DelegateCommand<object> ClearCommand { get; }
        public DelegateCommand<object> ExitWithoutSavingCommand { get; }

        private bool OnCommandCanExecute(object obj)
        {
            return IsCustomerDirty();
        }

        private bool OnClearCanExecute(object obj)
        {
            return (IsCustomerDirty() && Customer.CustomerId == 0);
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
                var bIsNew = Customer.CustomerId == 0;
                CustomerUpdated?.Invoke(this, new CustomerEventArgs(Customer, bIsNew));
                if (bIsNew == true)
                {
                    CustomersLeft = GetCustomerLeft(_currentReservation);
                    Customer = null;
                    Customer = new CustomerWrapper();
                }
                CleanAll();
            }
        }

/*        private int _maxNumberOfCustomers;

        public int MaxNumberOfCustomers
        {
            get
            {
                return _maxNumberOfCustomers;
            }
            private set
            {
                _currentReservation.Tours.ToList().ForEach((tour) =>
                {
                    tour.TourHotels.ToList().ForEach((tourHotel) =>
                    {
                        tourHotel.TourHotelRoomTypes.ToList().ForEach((hotelRoomType) =>
                        {
                            _maxNumberOfCustomers += (hotelRoomType.Persons);
                        });
                    });
                });
                _maxNumberOfCustomers = value > _maxNumberOfCustomers ? value : _maxNumberOfCustomers;
            }
        }*/

        private void OnClearCommand(object obj)
        {
            if (IsCustomerDirty())
            {
                var result = _messageDialogService.ShowOkCancelDialog((string)Application.Current.FindResource("AreYouSureMessage"), "Question");
                if (result == MessageDialogResult.CANCEL)
                    return;
            }
            Customer = null;
            Customer = new CustomerWrapper();
        }

        private void OnExitWithoutSavingCommand(object obj)
        {
            if (IsCustomerDirty())
            {
                var result = _messageDialogService.ShowOkCancelDialog((string)Application.Current.FindResource("AreYouSureMessage"), "Question");
                if (result == MessageDialogResult.CANCEL)
                    return;
            }
            CustomerCancelled?.Invoke(this, new CustomerEventArgs(null, true));
        }

        private bool IsCustomerDirty()
        {
            return Customer.IsAnythingDirty();
        }

        protected override void AddModels(List<ObjectBase> models)
        {
            models.Add(Customer);
        }

        protected override void OnViewLoaded()
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<CustomerWrapper, CustomerWrapper>();
            });

            if (_customerWrapper != null)
            {

                IMapper iMapper = config.CreateMapper();
                Customer = iMapper.Map<CustomerWrapper, CustomerWrapper>(_customerWrapper);
            }
            else
                Customer = new CustomerWrapper();
            CustomersLeft = GetCustomerLeft(_currentReservation);
            CleanAll();
        }

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

        public event EventHandler<CustomerEventArgs> CustomerUpdated;
        public event EventHandler<CustomerEventArgs> CustomerCancelled;
    }
}
