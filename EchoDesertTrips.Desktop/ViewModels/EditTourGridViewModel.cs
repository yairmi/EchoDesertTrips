using Core.Common.Contracts;
using Core.Common.Core;
using Core.Common.Extensions;
using Core.Common.UI.Core;
using Core.Common.UI.CustomEventArgs;
using Core.Common.UI.PubSubEvent;
using EchoDesertTrips.Client.Entities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using static EchoDesertTrips.Client.Entities.TourHotelRoomType;

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
            _serviceFactory = serviceFactory;
            _messageDialogService = messageBoxDialogService;
            SaveCommand = new DelegateCommand<object>(OnSaveCommand, OnSaveCommandCanExecute);
            ClearCommand = new DelegateCommand<object>(OnClearCommand, /*OnClearCanExecute*/OnClearCommandCanExecute);
            Unloaded = new DelegateCommand<object>(OnViewUnloaded);
            EditTourHotelCommand = new DelegateCommand<object>(OnEditTourHotelCommand);
            TourHotelRoomTypes = new ObservableCollection<TourHotelRoomType>();
            _eventAggregator.GetEvent<ReservationEditedEvent>().Subscribe(ReservationEdited);
            _eventAggregator.GetEvent<TourEditedEvent>().Subscribe(TourEdited);
            log.Debug("EditTourGridViewModel ctor end");
        }

        public DelegateCommand<object> Unloaded { get; private set; }
        public DelegateCommand<object> SaveCommand { get; private set; }
        public DelegateCommand<object> ClearCommand { get; private set; }
        public DelegateCommand<object> EditTourHotelCommand { get; private set; }

        private void OnEditTourHotelCommand(object tourHotel)
        {
            if (tourHotel is TourHotel)
            {
                bool bIsDirty = false;
                if (_currentTourHotel != null)
                {
                    var tourHotelRoomTypes = new ObservableCollection<TourHotelRoomType>();
                    foreach (var tourHotelRoomType in _currentTourHotel.TourHotelRoomTypes)
                    {
                        var copiedTourHotelRoomType = TourHotelRoomTypeHelper.CloneTourHotelRoomType(tourHotelRoomType);
                        tourHotelRoomTypes.Add(copiedTourHotelRoomType);
                    }
                    bIsDirty = Tour.IsAnythingDirty();
                    _currentTourHotel.TourHotelRoomTypes = tourHotelRoomTypes;
                }

                _currentTourHotel = (TourHotel)tourHotel;
                UpdateTourHotelRoomTypes(_currentTourHotel);
                if (!bIsDirty)
                    Tour.CleanAll();
            }
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
            int removedItems = RemoveUnselectedHotelsAndOptionals();
            if (IsTourDirty())
            {
                ValidateModel(true);
            }
            if (IsTourValid())
            {
                SetHotelEndDayForEachTourHotel();
                var newTour = TourHelper.CloneTour(Tour);
                if (Tour.TourId == 0)
                {
                    _eventAggregator.GetEvent<TourUpdatedEvent>().Publish(new TourEventArgs(newTour, removedItems, newTour.bInEdit == false));
                }
                else
                {
                    _eventAggregator.GetEvent<TourUpdatedEvent>().Publish(new TourEventArgs(newTour, removedItems, false));
                }
                CreateTour();
            }
        }

        private void SetHotelEndDayForEachTourHotel()
        {
            Tour.TourHotels.OrderBy(t => t.HotelStartDay);
            int count = Tour.TourHotels.Count();
            for (int i = count - 1; i >= 0; i--)
            {
                if (i == count - 1)
                    Tour.TourHotels[i].HotelEndDay = Tour.StartDate.AddDays(Tour.TourType.Days - 1);
                else
                    Tour.TourHotels[i].HotelEndDay = Tour.TourHotels[i+1].HotelStartDay;
            }
        }

        private int RemoveUnselectedHotelsAndOptionals()
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
            return removedItems;
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

        private bool IsTourValid()
        {
            foreach (var tourHotel in Tour.TourHotels)
            {
                foreach(var tourHotelRoomType in tourHotel.TourHotelRoomTypes)
                {
                    if (!tourHotelRoomType.IsValid)
                        return false;
                }
            }
            return Tour.IsValid;
        }

        protected override void AddModels(List<ObjectBase> models)
        {
            models.Add(Tour);
            Tour.TourHotels.ToList().ForEach((tourHotel) =>
            {
                tourHotel.TourHotelRoomTypes.ToList().ForEach(tourHotelRoomType =>
                {
                    models.Add(tourHotelRoomType);
                });
            });

        }

        protected override void OnViewLoaded()
        {
            _eventAggregator.GetEvent<OptionalUpdatedEvent>().Subscribe(OptionalUpdated);
            _eventAggregator.GetEvent<RoomTypeUpdatedEvent>().Subscribe(RoomTypeUpdated);
            _eventAggregator.GetEvent<HotelUpdatedEvent>().Subscribe(HotelUpdated);
            CreateTour();//This creates an empty tour. which result in displaying empty fields
        }

        private void OnViewUnloaded(object obj)
        {
            _eventAggregator.GetEvent<OptionalUpdatedEvent>().Unsubscribe(OptionalUpdated);
            _eventAggregator.GetEvent<RoomTypeUpdatedEvent>().Unsubscribe(RoomTypeUpdated);
            _eventAggregator.GetEvent<HotelUpdatedEvent>().Unsubscribe(HotelUpdated);
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
                Tour = new Tour();
            }
            _currentTourHotel = Tour.TourHotels.Count > 0 ? Tour.TourHotels[0] : null;
            UpdateTourHotelRoomTypes(_currentTourHotel);
            InitTourOptionals();
            Tour.CleanAll();
        }

        private void TourEdited(Tour tour)
        {
            CreateTour(tour);
        }

        private void ReservationEdited(EditReservationEventArgs e)
        {
            ViewMode = e.ViewMode;
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

        private TourHotel _currentTourHotel;

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

        private void UpdateTourHotelRoomTypes(TourHotel tourHotel)
        {
            TourHotelRoomTypes.Clear();
            if (tourHotel == null)
                return;

            var hotel = Inventories.Hotels.FirstOrDefault(h => h.HotelId == tourHotel.HotelId);

            //Init all rooms(tourhotelroomtype) for selected hotel
            hotel.HotelRoomTypes.ForEach((hotelRoomType) =>
            {
                if (IsHotelRoomTypeInTourDaysRange(hotelRoomType))
                {
                    var exist = tourHotel.TourHotelRoomTypes.FirstOrDefault(t => t.HotelRoomType.RoomTypeId == hotelRoomType.RoomTypeId);
                    if (exist == null)
                    {
                        var tourHotelRoomType = new TourHotelRoomType()
                        {
                            HotelRoomType = hotelRoomType,
                            HotelRoomTypeId = hotelRoomType.HotelRoomTypeId,
                            Capacity = 0,
                            Persons = 0
                        };
                        tourHotel.TourHotelRoomTypes.Add(tourHotelRoomType);
                    }
                }
            });
            TourHotelRoomTypes = tourHotel.TourHotelRoomTypes;
        }

        private bool IsHotelRoomTypeInTourDaysRange(HotelRoomType hotelRoomType)
        {
            bool bInRange = false;
            foreach (var dayRange in hotelRoomType.HotelRoomTypeDaysRanges)
            {
                if (Tour.StartDate >= dayRange.StartDaysRange && Tour.StartDate <= dayRange.EndDaysRange)
                {
                    bInRange = true;
                    break;
                }
            }
            return bInRange;
        }

        public void InitTourOptionals()
        {
            var tourOptionals = new ObservableCollection<TourOptional>();
            foreach (var optional in Inventories.Optionals)
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

        private void OptionalUpdated(OptionalEventArgs e)
        {
            try
            {
                log.Info("EditTourGridViewModel::OptionalUpdated called");
                if (Tour == null)
                    return;
                var existingOptional = Tour.TourOptionals.FirstOrDefault(t => t.Optional.OptionalId == e.Optional.OptionalId);
                if (existingOptional != null)
                {
                    var index = Tour.TourOptionals.IndexOf(existingOptional);
                    Tour.TourOptionals[index].Optional = e.Optional;
                }
                else
                {
                    var newTourOptional = new TourOptional()
                    {
                        Selected = false,
                        Optional = e.Optional,
                        OptionalId = e.Optional.OptionalId,
                        TourId = Tour.TourId,
                        PriceInclusive = false
                    };
                    Tour.TourOptionals.Add(newTourOptional);
                }
            }
            catch(Exception ex)
            {
                log.Error("Exception in OptionalUpdated: " + ex.Message);
            }
        }

        private void RoomTypeUpdated(RoomTypeEventArgs e)
        {
            try
            {

            }
            catch(Exception ex)
            {
                log.Error("Exception in RoomTypeUpdated: " + ex.Message);
            }
        }

        private void HotelUpdated(HotelEventArgs e)
        {
            try
            {
                log.Info("EditTourGridViewModel::HotelUpdated called");
                if (Tour == null)
                    return;
                foreach (var tourHotel in Tour.TourHotels)
                {
                    foreach (var hotelRoomType in e.Hotel.HotelRoomTypes)
                    {
                        if (IsHotelRoomTypeInTourDaysRange(hotelRoomType))
                        {
                            var exist = tourHotel.TourHotelRoomTypes.FirstOrDefault(t => t.HotelRoomType.HotelId == hotelRoomType.HotelId);
                            if (exist == null)
                            {
                                var tourHotelRoomType = new TourHotelRoomType()
                                {
                                    HotelRoomType = hotelRoomType,
                                    HotelRoomTypeId = hotelRoomType.HotelRoomTypeId,
                                    Capacity = 0,
                                    Persons = 0
                                };
                                tourHotel.TourHotelRoomTypes.Add(tourHotelRoomType);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("Exception in HotelUpdated: " + ex.Message);
            }
        }
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
