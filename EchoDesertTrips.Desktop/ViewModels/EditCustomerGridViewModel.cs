using AutoMapper;
using Core.Common.Contracts;
using Core.Common.Core;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Entities;
using EchoDesertTrips.Desktop.Support;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EchoDesertTrips.Desktop.ViewModels
{
    public class EditCustomerGridViewModel : ViewModelBase
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IMessageDialogService _messageDialogService;
        private readonly ReservationWrapper _currentReservation;
        private CustomerWrapper _customerWrapper;

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
            ClearCommand = new DelegateCommand<object>(OnClearCommand, OnCommandCanExecute);
            ExitWithoutSavingCommand = new DelegateCommand<object>(OnExitWithoutSavingCommand);
        }

        public DelegateCommand<object> SaveCommand { get; private set; }
        public DelegateCommand<object> ClearCommand { get; private set; }
        public DelegateCommand<object> ExitWithoutSavingCommand { get; private set; }

        private bool OnCommandCanExecute(object obj)
        {
            return IsCustomerDirty();
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
                bool bIsNew = Customer.CustomerId == 0;
                TotalCustomers = 0;
                CustomerUpdated?.Invoke(this, new CustomerEventArgs(Customer, bIsNew));
                CustomersLeft = _totalCustomers - _currentReservation.Customers.Count();
                Customer = null;
                Customer = new CustomerWrapper();
                CleanAll();
            }
        }

        private int _totalCustomers;

        public int TotalCustomers
        {
            get
            {
                return _totalCustomers;
            }
            private set
            {
                _totalCustomers = value;
                _currentReservation.Tours.ToList().ForEach((tour) =>
                {
                    tour.TourHotels.ToList().ForEach((tourHotel) =>
                    {
                        tourHotel.TourHotelRoomTypes.ToList().ForEach((hotelRoomType) =>
                        {
                            _totalCustomers += (hotelRoomType.Persons);
                        });
                    });
                });
            }
        }

        private void OnClearCommand(object obj)
        {
            if (IsCustomerDirty())
            {
                var Result = _messageDialogService.ShowOkCancelDialog((string)Application.Current.FindResource("AreYouSureMessage"), "Question");
                if (Result == MessageDialogResult.CANCEL)
                    return;
            }
            Customer = null;
            Customer = new CustomerWrapper();
        }

        private void OnExitWithoutSavingCommand(object obj)
        {
            if (IsCustomerDirty())
            {
                var Result = _messageDialogService.ShowOkCancelDialog((string)Application.Current.FindResource("AreYouSureMessage"), "Question");
                if (Result == MessageDialogResult.CANCEL)
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

            TotalCustomers = 0;
            CustomersLeft = TotalCustomers - _currentReservation.Customers.Count();

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

        private int _customersLeft;

        public int CustomersLeft
        {
            get
            {
                return _customersLeft;
            }
            set
            {
                _customersLeft = value;
                OnPropertyChanged(() => CustomersLeft);
            }
        }

        public event EventHandler<CustomerEventArgs> CustomerUpdated;
        public event EventHandler<CustomerEventArgs> CustomerCancelled;
    }
}
