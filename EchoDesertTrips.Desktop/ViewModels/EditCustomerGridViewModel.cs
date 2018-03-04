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

        public EditCustomerGridViewModel(IServiceFactory serviceFactory,
            IMessageDialogService messageDialogService,
            CustomerWrapper customer)
        {
            _serviceFactory = serviceFactory;
            _messageDialogService = messageDialogService;
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<CustomerWrapper, CustomerWrapper>();
            });

            if (customer != null)
            {

                IMapper iMapper = config.CreateMapper();
                Customer = iMapper.Map<CustomerWrapper, CustomerWrapper>(customer);
            }
            else
                Customer = new CustomerWrapper();

            CleanAll();

            SaveCommand = new DelegateCommand<object>(OnSaveCommand, OnSaveCommandCanExecute);
            ExitWithoutSavingCommand = new DelegateCommand<object>(OnExitWithoutSavingCommand);
        }

        public DelegateCommand<object> SaveCommand { get; private set; }
        public DelegateCommand<object> ExitWithoutSavingCommand { get; private set; }

        //After pressing the 'Save' button
        private bool OnSaveCommandCanExecute(object obj)
        {
            return IsHotelDirty();
        }

        private void OnSaveCommand(object obj)
        {
            if (IsHotelDirty())
            {
                ValidateModel();
            }
            if (Customer.IsValid)
            {
                //WithClient(_serviceFactory.CreateClient<IInventoryService>(), inventoryClient =>
                //{
                //    bool bIsNew = Hotel.HotelId == 0;
                //    Hotel.HotelRoomTypes.ForEach((hotelRoomType) =>
                //    {
                //        hotelRoomType.HotelId = Hotel.HotelId;
                //    });
                //    var savedHotel = inventoryClient.UpdateHotel(Hotel);
                //    HotelUpdated?.Invoke(this, new HotelEventArgs(savedHotel, bIsNew));
                //});
                bool bIsNew = Customer.CustomerId == 0;
                CustomerUpdated?.Invoke(this, new CustomerEventArgs(Customer, bIsNew));
            }
        }


        private void OnExitWithoutSavingCommand(object obj)
        {
            if (IsHotelDirty())
            {
                var Result = _messageDialogService.ShowOkCancelDialog((string)Application.Current.FindResource("AreYouSureMessage"), "Question");
                if (Result == MessageDialogResult.CANCEL)
                    return;
            }
            CustomerCancelled?.Invoke(this, new CustomerEventArgs(null, true));
        }

        private bool IsHotelDirty()
        {
            return Customer.IsAnythingDirty();
        }

        protected override void AddModels(List<ObjectBase> models)
        {
            models.Add(Customer);
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

        public event EventHandler<CustomerEventArgs> CustomerUpdated;
        public event EventHandler<CustomerEventArgs> CustomerCancelled;
    }
}
