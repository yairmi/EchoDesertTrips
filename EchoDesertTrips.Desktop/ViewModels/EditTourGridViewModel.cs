using Core.Common.Contracts;
using Core.Common.Core;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Entities;
using EchoDesertTrips.Desktop.Support;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace EchoDesertTrips.Desktop.ViewModels
{
    public class EditTourGridViewModel : ViewModelBase
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IMessageDialogService _messageDialogService;
        private readonly TourWrapper _tourWrapper;
        private readonly bool _isNewReservation;

        public EditTourGridViewModel(IServiceFactory serviceFactory,
                IMessageDialogService messageBoxDialogService,
                TourWrapper tour,
                bool isNewReservation)
        {
#if DEBUG
            log.Debug("EditTourGridViewModel ctor start");
#endif
            _serviceFactory = serviceFactory;
            _messageDialogService = messageBoxDialogService;
            _tourWrapper = tour;
            _isNewReservation = isNewReservation;

            SaveCommand = new DelegateCommand<object>(OnSaveCommand, OnCommandCanExecute);
            ClearCommand = new DelegateCommand<object>(OnClearCommand, OnClearCanExecute);
            ExitWithoutSavingCommand = new DelegateCommand<object>(OnExitCommand);
            CellEditEndingRoomTypeCommand = new DelegateCommand<TourHotelRoomType>(OnCellEditEndingRoomTypeCommand);
            TourHotelRoomTypes = new ObservableCollection<TourHotelRoomType>();
#if DEBUG
            log.Debug("EditTourGridViewModel ctor end");
#endif
        }

        public DelegateCommand<object> SaveCommand { get; private set; }
        public DelegateCommand<object> ClearCommand { get; private set; }
        public DelegateCommand<object> ExitWithoutSavingCommand { get; private set; }
        public DelegateCommand<TourHotelRoomType> CellEditEndingRoomTypeCommand { get; set; }

        private void OnCellEditEndingRoomTypeCommand(TourHotelRoomType tourHotelRoomType)
        {
            UpdateTourHotel(tourHotelRoomType);
        }

       private bool OnCommandCanExecute(object obj)
        {
            return IsTourDirty();
        }

        private bool OnClearCanExecute(object obj)
        {
            return (IsTourDirty() && Tour.TourId == 0);
        }

        private void OnSaveCommand(object obj)
        {
            if (IsTourDirty())
            {
                ValidateModel(true);
            }
            if (Tour.IsValid)
            {
                if (Tour.TourId == 0)
                {
                    TourUpdated?.Invoke(this, new TourEventArgs(Tour, true));
                    CreateNewTour();
                }
                else
                {
                    TourUpdated?.Invoke(this, new TourEventArgs(Tour, false));
                    CleanAll();
                }
            }
        }

        private void OnClearCommand(object obj)
        {
            if (IsTourDirty())
            {
                var result = _messageDialogService.ShowOkCancelDialog((string)Application.Current.FindResource("AreYouSureMessage"), "Question");
                if (result == MessageDialogResult.CANCEL)
                    return;
            }

            CreateNewTour();
        }

        private void OnExitCommand(object obj)
        {
            if (IsTourDirty())
            {
                var result = _messageDialogService.ShowOkCancelDialog((string)Application.Current.FindResource("AreYouSureMessage"), "Question");
                if (result == MessageDialogResult.CANCEL)
                    return;
            }
            TourCancelled?.Invoke(this, new TourEventArgs(null, true));
        }
#if DEBUG
        private bool _lastDertinessValue = false;
#endif

        private bool IsTourDirty()
        {
            var bDirty = Tour.IsAnythingDirty();
#if DEBUG
            if (bDirty != _lastDertinessValue)
            {

                log.Debug("EditTourGridViewModel dirty = " + bDirty);
                _lastDertinessValue = bDirty;
            }
#endif
            return bDirty;
        }

        protected override void AddModels(List<ObjectBase> models)
        {
            models.Add(Tour);
        }

        private TourWrapper _tour;

        public TourWrapper Tour
        {
            get
            {
                return _tour;
            }
            set
            {
                _tour = value;
                OnPropertyChanged(() => Tour, true);
            }
        }

        protected override void OnViewLoaded()
        {
#if DEBUG
            log.Debug("EditTourGridViewModel OnViewLoaded start");
#endif
            if (_tourWrapper != null)
            {
                Tour = TourHelper.CloneTourWrapper(_tourWrapper);
                SelectedHotel = Tour.TourHotels.Count > 0 ? Hotels.FirstOrDefault(h => h.HotelId == Tour.TourHotels[0].Hotel.HotelId) :
                    null;
                CleanAll();
            }
            else
            {
                CreateNewTour();
            }

            EnableCBTourType = _isNewReservation;
#if DEBUG
            log.Debug("EditTourGridViewModel OnViewLoaded end");
#endif
        }

        private void CreateNewTour()
        {
            Tour = null;
            Tour = new TourWrapper();
            InitTourOptionals(Tour);
            SelectedHotel = Tour.TourHotels.Count > 0 ? Hotels.FirstOrDefault(h => h.HotelId == Tour.TourHotels[0].Hotel.HotelId) :
                null;
            CleanAll();
        }

        private ObservableCollection<TourHotelRoomType> _tourHotelRoomTypes;

        public ObservableCollection<TourHotelRoomType> TourHotelRoomTypes
        {
            get
            {
                return _tourHotelRoomTypes;
            }
            set
            {
                _tourHotelRoomTypes = value;
                OnPropertyChanged(() => TourHotelRoomTypes);
            }
        }

        private Hotel _selectedHotel;

        public Hotel SelectedHotel
        {
            get
            {
                return _selectedHotel;
            }
            set
            {
                if (value != _selectedHotel)
                {
                    _selectedHotel = value;
                    UpdateTourHotelRoomTypes(_selectedHotel);
                    OnPropertyChanged(() => SelectedHotel);
                }
            }
        }

        private bool _enableCBTourType;

        public bool EnableCBTourType
        {
            get
            {
                return _enableCBTourType;
            }
            set
            {
                _enableCBTourType = value;
                OnPropertyChanged(() => EnableCBTourType);
            }
        }

        //For GUI (RoomTypes DataGrid DataSource) - After selecting Hotel from Drop Down
        private void UpdateTourHotelRoomTypes(Hotel hotel)
        {
            TourHotelRoomTypes.Clear();
            if (hotel == null)
                return;
            //Init all rooms(tourhotelroomtype) for selected hotel
            hotel.HotelRoomTypes.ForEach((hotelRoomType) =>
            {
                var tourHotelRoomType = new TourHotelRoomType()
                {
                    HotelRoomType = hotelRoomType,
                    HotelRoomTypeId = hotelRoomType.HotelRoomTypeId,
                    Capacity = 0,
                    Persons = 0
                };
                TourHotelRoomTypes.Add(tourHotelRoomType);
            });
            //update with actual persons+capacity
            var tempTourHotel = Tour.TourHotels.FirstOrDefault(t => t.Hotel.HotelId == hotel.HotelId);
            tempTourHotel?.TourHotelRoomTypes.ToList().ForEach((tourHotelRoomType) =>
            {
                var tempTourHotelRoomType = TourHotelRoomTypes.FirstOrDefault(t => t.HotelRoomType.RoomTypeId == tourHotelRoomType.HotelRoomType.RoomTypeId);
                var index = TourHotelRoomTypes.IndexOf(tempTourHotelRoomType);
                TourHotelRoomTypes[index] = tourHotelRoomType;
            });
        }
        //Update Model
        private void UpdateTourHotel(TourHotelRoomType tourHotelRoomType)
        {
            if (_selectedHotel == null)
                return;
            var tempTourHotel = Tour.TourHotels.FirstOrDefault(t => t.Hotel.HotelId == _selectedHotel.HotelId);
            if (tempTourHotel == null)
            {
                var tourHotel = new TourHotel
                {
                    HotelId = _selectedHotel.HotelId,
                    Hotel = _selectedHotel
                };
                tourHotel.TourHotelRoomTypes.Add(tourHotelRoomType);
                Tour.TourHotels.Add(tourHotel);
            }
            else
            {
                var existingtourHotelRoomType = tempTourHotel.TourHotelRoomTypes.FirstOrDefault(t => t.HotelRoomTypeId == tourHotelRoomType.HotelRoomTypeId);
                if (existingtourHotelRoomType == null)
                {
                    tempTourHotel.TourHotelRoomTypes.Add(tourHotelRoomType);
                }
            }
        }

        public event EventHandler<TourEventArgs> TourUpdated;
        public event EventHandler<TourEventArgs> TourCancelled;
    }

    public class TourHotelRoomsValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value,
            System.Globalization.CultureInfo cultureInfo)
        {
            var tourHotelRoomType = (value as BindingGroup).Items[0] as TourHotelRoomType;
            if (tourHotelRoomType.Capacity < 0)
            {
                return new ValidationResult(false,
                    "Room capacity must be greater then 0");
            }
            else
                if (tourHotelRoomType.Persons < 0)
            {
                return new ValidationResult(false,
                    "Number of pax must be greater then 0");
            }
            var validationResult = ValidationResult.ValidResult;
            return validationResult;
        }
    }
}
