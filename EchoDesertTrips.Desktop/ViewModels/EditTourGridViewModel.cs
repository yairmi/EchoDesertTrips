﻿using Core.Common.Contracts;
using Core.Common.Core;
using Core.Common.UI.Core;
using Core.Common.UI.CustomEventArgs;
using Core.Common.UI.PubSubEvent;
using Core.Common.Utils;
using static Core.Common.Core.Const;
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
using EchoDesertTrips.Client.Contracts;

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
            ClearCommand = new DelegateCommand<object>(OnClearCommand, OnClearCommandCanExecute);
            Unloaded = new DelegateCommand<object>(OnViewUnloaded);
            EditTourHotelCommand = new DelegateCommand<object>(OnEditTourHotelCommand);
            OptionalCheckedCommand = new DelegateCommand<object>(OnOptionalCheckedCommand);
            OptionalUncheckedCommand = new DelegateCommand<object>(OnOptionalUncheckedCommand);
            RoomTypesRowEditEndingCommand = new DelegateCommand<object>(OnRoomTypesRowEditEndingCommand);
            TourTypeSelectionChangedCommand = new DelegateCommand<object>(OnTourTypeSelectionChangedCommand);
            StartDaySelectedDateChangedCommand = new DelegateCommand<object>(OnStartDaySelectedDateChangedCommand);
            TourHotelRoomTypesGUI = new ObservableCollection<TourHotelRoomType>();
            
            _eventAggregator.GetEvent<ReservationEditedEvent>().Subscribe(ReservationEdited);
            _eventAggregator.GetEvent<TourEditedEvent>().Subscribe(TourEdited);
        }

        public DelegateCommand<object> Unloaded { get; private set; }
        public DelegateCommand<object> SaveCommand { get; private set; }
        public DelegateCommand<object> ClearCommand { get; private set; }
        public DelegateCommand<object> EditTourHotelCommand { get; private set; }
        public DelegateCommand<object> OptionalCheckedCommand { get; private set; }
        public DelegateCommand<object> OptionalUncheckedCommand { get; private set; }
        public DelegateCommand<object> RoomTypesRowEditEndingCommand { get; private set; }
        public DelegateCommand<object> TourTypeSelectionChangedCommand { get; private set; }
        public DelegateCommand<object> StartDaySelectedDateChangedCommand { get; private set; }
        /*private void OnEditTourHotelCommand(object tourHotel)
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
        }*/

        private void OnEditTourHotelCommand(object tourHotel)
        {
            if (tourHotel is TourHotel)
            {
                _currentTourHotel = (TourHotel)tourHotel;
                OnPropertyChanged(() => TourHotelRoomTypesGUI);
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
                    _eventAggregator.GetEvent<TourUpdatedEvent>().Publish(new TourEventArgs(newTour, newTour.bInEdit == false));//TODO: set to 1 only if item was removed
                }
                else
                {
                    _eventAggregator.GetEvent<TourUpdatedEvent>().Publish(new TourEventArgs(newTour, false));
                }
                ClearTour();
            }
        }

        private void OnOptionalUncheckedCommand(object obj)
        {
            TourOptional tourOptional = obj as TourOptional;
            if (tourOptional != null)
            {
                Tour.TourOptionals.RemoveAt(Tour.TourOptionals.IndexOf(Tour.TourOptionals.FirstOrDefault(t => t.OptionalId == tourOptional.OptionalId)));
                Tour.PropertyDeleted = true;
             }
        }

        private void OnOptionalCheckedCommand(object obj)
        {
            TourOptional tourOptional = obj as TourOptional;
            if (tourOptional != null)
            {
                if (tourOptional.PriceInclusive == true)
                {
                    tourOptional.PriceInclusiveValue = tourOptional.PriceInclusiveValue == 0 ?
                        tourOptional.Optional.PriceInclusive : tourOptional.PriceInclusiveValue;
                } 
                else
                {
                    tourOptional.PricePerPerson = tourOptional.PricePerPerson == 0 ?
                           tourOptional.Optional.PricePerPerson : tourOptional.PricePerPerson;
                }
                tourOptional.TourId = Tour.TourId;
                Tour.TourOptionals.Add(tourOptional);
            }
        }

        private void OnRoomTypesRowEditEndingCommand(object obj)
        {
            TourHotelRoomType tourHotelRoomType = obj as TourHotelRoomType;
            if (tourHotelRoomType != null)
            {
                var nIndex = _currentTourHotel.TourHotelRoomTypes.IndexOf(_currentTourHotel.TourHotelRoomTypes.FirstOrDefault(t => t.HotelRoomTypeId == tourHotelRoomType.HotelRoomTypeId));
                if (nIndex == -1)
                {
                    if (tourHotelRoomType.Capacity > 0 || tourHotelRoomType.Persons > 0)
                    {
                        _currentTourHotel.TourHotelRoomTypes.Add(tourHotelRoomType);
                    }
                    else log.Error("Cannot add roomType because Capacity and Persons are empty");
                }
                else
                {
                    if (tourHotelRoomType.Capacity > 0 || tourHotelRoomType.Persons > 0)
                        _currentTourHotel.TourHotelRoomTypes[nIndex] = tourHotelRoomType;
                    else
                    {
                        _currentTourHotel.TourHotelRoomTypes.RemoveAt(nIndex);
                        Tour.PropertyDeleted = true;//080420 - It safe to do that on tour since the operator cannot delete a tour
                    }
                }
            }
        }

        private void OnTourTypeSelectionChangedCommand(object obj)
        {
            int? selectedId = obj as int?;
            if (Tour == null || selectedId <= 0)
                return;
            var tourType = Inventories.TourTypes.FirstOrDefault(t => t.TourTypeId == selectedId);
            if (tourType != null)
            {
                Tour.TourTypePrice = $"{tourType.InfantPrices}:{tourType.ChildPrices}:{tourType.AdultPrices}";
                Tour.EndDate = Tour.StartDate.AddDays(tourType.Days - 1);
            }
        }

        private void OnStartDaySelectedDateChangedCommand(object obj)
        {
            if (Tour != null)
            {
                Tour.EndDate = Tour.StartDate.AddDays(Tour.TourType.Days - 1);
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

        private void OnClearCommand(object obj)
        {
            if (IsTourDirty())
            {
                var result = _messageDialogService.ShowOkCancelDialog((string)Application.Current.FindResource("AreYouSureMessage"), "Question");
                if (result == MessageDialogResult.CANCEL)
                    return;
            }
            ClearTour();
        }

        private bool _lastDertinessValue = false;

        private bool IsTourDirty()
        {
            return IsDirtyCheck(() =>
            {
                var bDirty = Tour != null ? Tour.IsAnythingDirty() && (!ViewMode) : false;
                if (bDirty != _lastDertinessValue)
                {

                    log.Debug($"Dirty = {bDirty}");
                    _lastDertinessValue = bDirty;
                }
                return bDirty;
            });
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
            ViewModelLoaded(() =>
            {
                _eventAggregator.GetEvent<OptionalUpdatedEvent>().Subscribe(OptionalUpdated);
                _eventAggregator.GetEvent<RoomTypeUpdatedEvent>().Subscribe(RoomTypeUpdated);
                _eventAggregator.GetEvent<HotelUpdatedEvent>().Subscribe(HotelUpdated);
                InitTourControls();
            });
        }

        private void OnViewUnloaded(object obj)
        {
            ViewModelUnLoaded(() =>
            {
                _eventAggregator.GetEvent<OptionalUpdatedEvent>().Unsubscribe(OptionalUpdated);
                _eventAggregator.GetEvent<RoomTypeUpdatedEvent>().Unsubscribe(RoomTypeUpdated);
                _eventAggregator.GetEvent<HotelUpdatedEvent>().Unsubscribe(HotelUpdated);
                ClearTour();
            });
        }

        private void InitTourControls()
        {
            _currentTourHotel = Tour.TourHotels.Count > 0 ? Tour.TourHotels[0] : null;
            OnPropertyChanged(() => TourHotelRoomTypesGUI);
            OnPropertyChanged(() => TourOptionalsGUI);
        }

        private void ClearTour()
        {
            PropertySupport.ResetProperties<Tour>(Tour);
            OnPropertyChanged(() => Tour);
            InitTourControls();
            Tour.CleanAll();
        }

        private void TourEdited(Tour tour)
        {
            Tour = TourHelper.CloneTour(tour);
            InitTourControls();
            Tour.CleanAll();
        }

        private void ReservationEdited(EditReservationEventArgs e)
        {
            ViewMode = e.ViewMode;
        }

        private ObservableCollection<TourHotelRoomType> _tourHotelRoomTypesGUI;

        public ObservableCollection<TourHotelRoomType> TourHotelRoomTypesGUI {
            get
            {
                if (_tourHotelRoomTypesGUI == null)
                    _tourHotelRoomTypesGUI = new ObservableCollection<TourHotelRoomType>();
                else _tourHotelRoomTypesGUI.Clear();

                if (_currentTourHotel == null)
                    return _tourHotelRoomTypesGUI;
                _tourHotelRoomTypesGUI.AddRange(_currentTourHotel.TourHotelRoomTypes);
                var hotel = Inventories.Hotels.FirstOrDefault(h => h.HotelId == _currentTourHotel.HotelId);

                //Init all rooms(tourhotelroomtype) for selected hotel
                hotel.HotelRoomTypes.ForEach((hotelRoomType) =>
                {
                    if (IsHotelRoomTypeInTourDaysRange(hotelRoomType))
                    {
                        var exist = _currentTourHotel.TourHotelRoomTypes.FirstOrDefault(t => t.HotelRoomType.RoomTypeId == hotelRoomType.RoomTypeId);
                        if (exist == null)
                        {
                            _tourHotelRoomTypesGUI.Add(new TourHotelRoomType()
                            {
                                HotelRoomType = hotelRoomType,
                                HotelRoomTypeId = hotelRoomType.HotelRoomTypeId,
                                Capacity = 0,
                                Persons = 0
                            });
                        }
                    }
                });
                return _tourHotelRoomTypesGUI;
            }
            set
            {
                OnPropertyChanged(() => TourHotelRoomTypesGUI);
            }
        }

        private TourHotel _currentTourHotel;

        private Tour _tour;

        [Import]
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

        private bool IsHotelRoomTypeInTourDaysRange(HotelRoomType hotelRoomType)
        {
            bool bInRange = false;
            foreach (var dayRange in hotelRoomType.HotelRoomTypeDaysRanges)
            {
                if (Tour.StartDate.Date >= dayRange.StartDaysRange.Date && Tour.StartDate.Date <= dayRange.EndDaysRange.Date)
                {
                    bInRange = true;
                    break;
                }
            }
            return bInRange;
        }

        private ObservableCollection<TourOptional> _tourOptionalsGUI;
        public ObservableCollection<TourOptional> TourOptionalsGUI
        {
            get
            {
                if (_tourOptionalsGUI == null)
                    _tourOptionalsGUI = new ObservableCollection<TourOptional>();
                else _tourOptionalsGUI.Clear();

                foreach (var optional in Inventories.Optionals)
                {
                    var tourOptional = Tour.TourOptionals.FirstOrDefault(o => o.OptionalId == optional.OptionalId);
                    if (tourOptional == null)
                    {
                        var newTourOptional = new TourOptional()
                        {
                            Selected = false,
                            Optional = AutoMapperUtil.Map<Optional, Optional>(optional),
                            OptionalId = optional.OptionalId,
                            TourId = Tour.TourId,
                            PriceInclusive = false
                        };
                        _tourOptionalsGUI.Add(newTourOptional);
                    }
                    else
                    {
                        tourOptional.Selected = true;
                        _tourOptionalsGUI.Add(tourOptional);
                    }
                }
                return _tourOptionalsGUI;
            }
            set
            {
                _tourOptionalsGUI = value;
                OnPropertyChanged(() => TourOptionalsGUI);
            }
        }

        private void OptionalUpdated(OptionalEventArgs e)
        {
            try
            {
                if (Tour == null)
                    return;
                var existingOptional = Tour.TourOptionals.FirstOrDefault(t => t.Optional.OptionalId == e.Optional.OptionalId);
                if (existingOptional != null)
                {
                    var index = Tour.TourOptionals.IndexOf(existingOptional);
                    Tour.TourOptionals[index].Optional = AutoMapperUtil.Map<Optional, Optional>(e.Optional);
                }
                else
                {
                    var newTourOptional = new TourOptional()
                    {
                        Selected = false,
                        Optional = AutoMapperUtil.Map<Optional, Optional>(e.Optional),
                        OptionalId = e.Optional.OptionalId,
                        TourId = Tour.TourId,
                        PriceInclusive = false
                    };
                    Tour.TourOptionals.Add(newTourOptional);
                }
            }
            catch(Exception ex)
            {
                log.Error("Failed to update tour optional", ex);
            }
        }

        public TourType TourType
        {
            get
            {
                return Tour != null ? Tour.TourType : null;
            }
            set
            {
                if (value != null && Tour!= null && Tour.TourType != value)
                {
                    Tour.TourType = value;
                    Tour.SubTours.Clear();
                    string[] destinations = SimpleSplitter.Split(Tour.TourType.Destinations);
                    destinations.ToList().ForEach((destination) =>
                    {
                        Tour.SubTours.Add(new SubTour()
                        {
                            DestinationName = destination,
                        });
                    });
                    Tour.EndDate = Tour.TourType.Days == 0 ? Tour.EndDate : Tour.StartDate.AddDays(Tour.TourType.Days - 1);

                }
            }
        }

        private void RoomTypeUpdated(RoomTypeEventArgs e)
        {
            try
            {

            }
            catch(Exception ex)
            {
                log.Error("Failed to update tour room type", ex);
            }
        }

        private void HotelUpdated(HotelEventArgs e)
        {
            try
            {
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
                log.Error("Hotel updated failed", ex);
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
