using Core.Common.Contracts;
using Core.Common.Core;
using Core.Common.Extensions;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Entities;
using EchoDesertTrips.Desktop.Support;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class EditTourGridViewModel : ViewModelBase
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IMessageDialogService _messageDialogService;
       
        [ImportingConstructor]
        public EditTourGridViewModel(IServiceFactory serviceFactory,
                IMessageDialogService messageBoxDialogService)
        {
            log.Debug("EditTourGridViewModel ctor start");
            _serviceFactory = serviceFactory;
            _messageDialogService = messageBoxDialogService;
            SaveCommand = new DelegateCommand<object>(OnSaveCommand, OnSaveCommandCanExecute);
            ClearCommand = new DelegateCommand<object>(OnClearCommand, /*OnClearCanExecute*/OnClearCommandCanExecute);
            CellEditEndingRoomTypeCommand = new DelegateCommand<TourHotelRoomType>(OnCellEditEndingRoomTypeCommand);
            TourHotelRoomTypes = new ObservableCollection<TourHotelRoomType>();
            log.Debug("EditTourGridViewModel ctor end");
        }

        public DelegateCommand<object> SaveCommand { get; private set; }
        public DelegateCommand<object> ClearCommand { get; private set; }
        public DelegateCommand<TourHotelRoomType> CellEditEndingRoomTypeCommand { get; set; }

        private void OnCellEditEndingRoomTypeCommand(TourHotelRoomType tourHotelRoomType)
        {
            UpdateTourHotel(tourHotelRoomType);
        }

        private bool OnSaveCommandCanExecute(object obj)
        {
            return IsTourDirty();
        }

        private bool OnClearCommandCanExecute(object obj)
        {
            return true;    //This is for the case that the user Edit an existing and made no change and after it he want to clear it.
        }

        private void OnSaveCommand(object obj)
        {
            if (IsTourDirty())
            {
                ValidateModel(true);
            }
            if (Tour.IsValid)
            {
                int removedItems = 0;
                //Remove Unselected Hotels
                Tour.TourHotels.ToList().ForEach((tourHotel) =>
                {
                    removedItems += tourHotel.TourHotelRoomTypes.RemoveItems(t => t.Persons == 0 && t.Capacity == 0);
                });
                removedItems += Tour.TourHotels.RemoveItems(t => !t.TourHotelRoomTypes.Any());
                //Remove Unselected Optionals
                removedItems += Tour.TourOptionals.RemoveItems(t => t.Selected == false);
                if (Tour.TourId == 0)
                {
                    TourUpdated?.Invoke(this, new TourEventArgs(Tour, removedItems, Tour.bInEdit == false));
                }
                else
                {
                    TourUpdated?.Invoke(this, new TourEventArgs(Tour, removedItems, false));
                }
                CreateTour();
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

            CreateTour();
        }

        private bool _lastDertinessValue = false;

        private bool IsTourDirty()
        {
            var bDirty = Tour != null ? Tour.IsAnythingDirty() && (!ViewMode) : false;
            if (bDirty != _lastDertinessValue)
            {

                log.Debug("EditTourGridViewModel dirty = " + bDirty);
                _lastDertinessValue = bDirty;
            }
            return bDirty;
        }

        protected override void AddModels(List<ObjectBase> models)
        {
            models.Add(Tour);
        }

        protected override void OnViewLoaded()
        {
            CreateTour();//This creates an empty tour. which result in displaying empty fields
        }

        public void CreateTour(Tour tour = null)
        {
            if (tour != null)
            {
                Tour = TourHelper.CloneTour(tour);
            }
            else
            {
                Tour = null;
                GC.Collect();
                log.Debug("GC Tour. Before WaitForPendingFinalizers");
                GC.WaitForPendingFinalizers();
                log.Debug("GC Tour. After WaitForPendingFinalizers");
                Tour = new Tour();
            }
            SelectedHotel = Tour.TourHotels.Count > 0 ? Hotels.FirstOrDefault(h => h.HotelId == Tour.TourHotels[0].Hotel.HotelId) :
                null;
            InitTourOptionals();
            Tour.CleanAll();
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

        private Tour _tour;

        public Tour Tour
        {
            get
            {
                return _tour;
            }
            set
            {
                _tour = value;
                OnPropertyChanged(() => Tour);
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

        public void InitTourOptionals()
        {
            var tourOptionals = new ObservableCollection<TourOptional>();
            foreach (var optional in Optionals)
            {
                var tourOptional = Tour.TourOptionals.FirstOrDefault(o => o.OptionalId == optional.OptionalId);
                if (tourOptional == null)
                {
                    var newTourOptional = new TourOptional()
                    {
                        Selected = false,
                        Optional = optional,
                        OptionalId = optional.OptionalId,
                        TourId = Tour.TourId,
                        PriceInclusive = false
                    };
                    tourOptionals.Add(newTourOptional);
                }
                else
                {
                    tourOptional.Selected = true;
                    tourOptionals.Add(tourOptional);
                }
            }
            Tour.TourOptionals.Clear();
            Tour.TourOptionals = tourOptionals;
        }

        public event EventHandler<TourEventArgs> TourUpdated;
        public event EventHandler<TourEventArgs> TourCancelled;
    }

    public class TourTypeControlConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var tour = (Tour)value;
            return (tour == null || tour.TourId == 0);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
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
