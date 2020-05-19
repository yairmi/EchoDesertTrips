using AutoMapper;
using Core.Common.Contracts;
using Core.Common.Core;
using Core.Common.UI.Core;
using Core.Common.UI.CustomEventArgs;
using Core.Common.UI.PubSubEvent;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
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
            Hotel = hotel;
            CleanAll();
            SaveCommand = new DelegateCommand<object>(OnSaveCommand, OnSaveCommandCanExecute);
            CancelCommand = new DelegateCommand<object>(OnCancelCommand);
        }

        private void Update_Hotel_HotelRoomTypes()
        {
            if (hotelRoomTypeWithDaysRanges.hotelRoomTypeWithDaysRanges.Count == 0)
                return;
            if (Hotel.HotelRoomTypes == null)
                Hotel.HotelRoomTypes = new List<HotelRoomType>();
            else
                Hotel.HotelRoomTypes.Clear();

            foreach (var hotelRoomTypeWithDaysRange in hotelRoomTypeWithDaysRanges.hotelRoomTypeWithDaysRanges)
            {
                var existingHotelRoomType = Hotel.HotelRoomTypes.Find(h => h.RoomTypeId == hotelRoomTypeWithDaysRange.RoomTypeId);
                if (existingHotelRoomType != null)
                {
                    existingHotelRoomType.HotelRoomTypeDaysRanges.Add(new HotelRoomTypeDaysRange()
                    {
                        HotelRoomTypeDaysRangeId = hotelRoomTypeWithDaysRange.HotelRoomTypeDaysRangeId,
                        StartDaysRange = hotelRoomTypeWithDaysRange.StartDaysRange,
                        EndDaysRange = hotelRoomTypeWithDaysRange.EndDaysRange,
                        PricePerPerson = hotelRoomTypeWithDaysRange.PricePerPerson
                    });
                }
                else
                {
                    var hotelRoomType = new HotelRoomType()
                    {
                        HotelRoomTypeId = hotelRoomTypeWithDaysRange.HotelRoomTypeId,
                        HotelId = hotelRoomTypeWithDaysRange.HotelId,
                        RoomTypeId = hotelRoomTypeWithDaysRange.RoomTypeId,
                        HotelRoomTypeDaysRanges = new List<HotelRoomTypeDaysRange>()
                    };
                    hotelRoomType.HotelRoomTypeDaysRanges.Add(new HotelRoomTypeDaysRange()
                    {
                        HotelRoomTypeDaysRangeId = hotelRoomTypeWithDaysRange.HotelRoomTypeDaysRangeId,
                        StartDaysRange = hotelRoomTypeWithDaysRange.StartDaysRange,
                        EndDaysRange = hotelRoomTypeWithDaysRange.EndDaysRange,
                        PricePerPerson = hotelRoomTypeWithDaysRange.PricePerPerson
                    });
                    Hotel.HotelRoomTypes.Add(hotelRoomType);
                }
            }
            //}
        }

        public DelegateCommand<object> SaveCommand { get; private set; }
        public DelegateCommand<object> CancelCommand { get; private set; }
        public DelegateCommand<object> SaveRoomDataCommand { get; private set; }
        public DelegateCommand<object> CancelRoomDataCommand { get; private set; }

        //After pressing the 'Save' button
        private bool OnSaveCommandCanExecute(object obj)
        {
            return IsHotelDirty() || hotelRoomTypeWithDaysRanges.IsAnythingDirty();
        }

        private void OnSaveCommand(object obj)
        {
            Update_Hotel_HotelRoomTypes();
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
                    var savedHotel = inventoryClient.UpdateHotel(Hotel);//Update DB
                    _eventAggregator.GetEvent<HotelUpdatedEvent>().Publish(new HotelEventArgs(savedHotel, bIsNew));
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
            _eventAggregator.GetEvent<HotelCancelledEvent>().Publish(new HotelEventArgs(null, true));
        }

        private bool IsHotelDirty()
        {
            return Hotel.IsAnythingDirty();
        }

        protected override void AddModels(List<ObjectBase> models)
        {
            models.Add(Hotel);
        }

        protected override void OnViewLoaded()
        {
            if (hotelRoomTypeWithDaysRanges == null)
                hotelRoomTypeWithDaysRanges = new HotelRoomTypeWithDaysRanges()
                {
                    hotelRoomTypeWithDaysRanges = new RangeObservableCollection<HotelRoomTypeWithDaysRange>() 
                };

            foreach (var hotelRoomType in Hotel.HotelRoomTypes)
            {
                if (hotelRoomType.HotelRoomTypeDaysRanges != null)
                {
                    foreach (var hotelRoomTypeDaysRange in hotelRoomType.HotelRoomTypeDaysRanges)
                    {
                        hotelRoomTypeWithDaysRanges.hotelRoomTypeWithDaysRanges.Add(new HotelRoomTypeWithDaysRange()
                        {
                            HotelRoomTypeDaysRangeId = hotelRoomTypeDaysRange.HotelRoomTypeDaysRangeId,
                            HotelRoomTypeId = hotelRoomType.HotelRoomTypeId,
                            RoomTypeId = hotelRoomType.RoomTypeId,
                            RoomType = hotelRoomType.RoomType,
                            HotelId = hotelRoomType.HotelId,
                            StartDaysRange = hotelRoomTypeDaysRange.StartDaysRange,
                            EndDaysRange = hotelRoomTypeDaysRange.EndDaysRange,
                            PricePerPerson = hotelRoomTypeDaysRange.PricePerPerson
                        });
                    }
                }
                hotelRoomTypeWithDaysRanges.CleanAll();
            }
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

        public HotelRoomTypeWithDaysRanges hotelRoomTypeWithDaysRanges { get; set; }

        public class HotelRoomTypeWithDaysRanges : ObjectBase
        {
            public RangeObservableCollection<HotelRoomTypeWithDaysRange> hotelRoomTypeWithDaysRanges { get; set; }
        }

        public class HotelRoomTypeWithDaysRange : ObjectBase
        {
            public HotelRoomTypeWithDaysRange()
            {
                _startDaysRange = DateTime.Now.Date;
                _endDaysRange = DateTime.Now.Date;
            }
            public int HotelRoomTypeDaysRangeId { get; set; }
            public int HotelRoomTypeId { get; set; }
            public int HotelId { get; set; }
            public int RoomTypeId { get; set; }

            private RoomType _roomType;
            public RoomType RoomType
            {
                get
                {
                    return _roomType;
                }
                set
                {
                    _roomType = value;
                    OnPropertyChanged(() => RoomType, true);
                }
            }

            private DateTime _startDaysRange;
            public DateTime StartDaysRange
            {
                get
                {
                    return _startDaysRange;
                }
                set
                {
                    _startDaysRange = value;
                    OnPropertyChanged(() => StartDaysRange, true);
                }
            }

            private DateTime _endDaysRange;
            public DateTime EndDaysRange
            {
                get
                {
                    return _endDaysRange;
                }
                set
                {
                    _endDaysRange = value;
                    OnPropertyChanged(() => EndDaysRange, true);
                }
            }

            private decimal _pricePerPerson;
            public decimal PricePerPerson
            {
                get
                {
                    return _pricePerPerson;
                }
                set
                {
                    _pricePerPerson = value;
                    OnPropertyChanged(() => PricePerPerson, true);
                }
            }
        }
    }
}
