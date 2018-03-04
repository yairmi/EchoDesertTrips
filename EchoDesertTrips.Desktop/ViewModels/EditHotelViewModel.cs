using AutoMapper;
using Core.Common.Contracts;
using Core.Common.Core;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using EchoDesertTrips.Desktop.Support;
using System;
using System.Collections.Generic;
using System.Windows;

namespace EchoDesertTrips.Desktop.ViewModels
{
    public class EditHotelViewModel : ViewModelBase
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IMessageDialogService _messageDialogService;

        public EditHotelViewModel(IServiceFactory serviceFactory,
            IMessageDialogService messageDialogService,
            Hotel hotel)
        {
            _serviceFactory = serviceFactory;
            _messageDialogService = messageDialogService;
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<Hotel, Hotel>();
            });

            if (hotel != null)
            {

                IMapper iMapper = config.CreateMapper();
                Hotel = iMapper.Map<Hotel, Hotel>(hotel);
            }
            else
                Hotel = new Hotel();

            CleanAll();

            SaveCommand = new DelegateCommand<object>(OnSaveCommand, OnSaveCommandCanExecute);
            CancelCommand = new DelegateCommand<object>(OnCancelCommand);
        }

        public DelegateCommand<object> SaveCommand { get; private set; }
        public DelegateCommand<object> CancelCommand { get; private set; }

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
            if (Hotel.IsValid)
            {
                WithClient(_serviceFactory.CreateClient<IInventoryService>(), inventoryClient =>
                {
                    bool bIsNew = Hotel.HotelId == 0;
                    Hotel.HotelRoomTypes.ForEach((hotelRoomType) =>
                    {
                        hotelRoomType.HotelId = Hotel.HotelId;
                    });
                    var savedHotel = inventoryClient.UpdateHotel(Hotel);
                    HotelUpdated?.Invoke(this, new HotelEventArgs(savedHotel, bIsNew));
                });
            }
        }


        private void OnCancelCommand(object obj)
        {
            if (IsHotelDirty())
            {
                var Result = _messageDialogService.ShowOkCancelDialog((string)Application.Current.FindResource("AreYouSureMessage"), "Question");
                if (Result == MessageDialogResult.CANCEL)
                    return;
            }
            HotelCancelled?.Invoke(this, new HotelEventArgs(null, true));
        }

        private bool IsHotelDirty()
        {
            return Hotel.IsAnythingDirty();
        }

        protected override void AddModels(List<ObjectBase> models)
        {
            models.Add(Hotel);
        }

        private Hotel _hotel;

        public Hotel Hotel
        {
            get
            {
                return _hotel;
            }
            set
            {
                _hotel = value;
                OnPropertyChanged(() => Hotel, true);
            }
        }

        public event EventHandler<HotelEventArgs> HotelUpdated;
        public event EventHandler<HotelEventArgs> HotelCancelled;
    }
}
