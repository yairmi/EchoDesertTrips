﻿using Core.Common.Contracts;
using Core.Common.Core;
using Core.Common.Extensions;
using Core.Common.UI.Core;
using Core.Common.Utils;
using EchoDesertTrips.Client.Entities;
using EchoDesertTrips.Desktop.Support;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace EchoDesertTrips.Desktop.ViewModels
{
    public class EditTourGridViewModel : ViewModelBase
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IMessageDialogService _messageDialogService;

        public EditTourGridViewModel(IServiceFactory serviceFactory,
                IMessageDialogService messageBoxDialogService,
                TourWrapper tour)
        {
            _serviceFactory = serviceFactory;
            _messageDialogService = messageBoxDialogService;
            if (tour != null)
            {
                Tour = TourHelper.CloneTourWrapper(tour);
            }
            else
            {
                Tour = new TourWrapper();
            }

            SaveCommand = new DelegateCommand<object>(OnSaveCommand, OnSaveCommandCanExecute);
            ExitWithoutSavingCommand = new DelegateCommand<object>(OnExitWithoutSavingCommand);
            CellEditEndingRoomTypeCommand = new DelegateCommand<TourHotelRoomType>(OnCellEditEndingRoomTypeCommand);
            TourHotelRoomTypes = new ObservableCollection<TourHotelRoomType>();
        }

        public DelegateCommand<object> SaveCommand { get; private set; }
        public DelegateCommand<object> ExitWithoutSavingCommand { get; private set; }
        public DelegateCommand<TourHotelRoomType> CellEditEndingRoomTypeCommand { get; set; }

        private void OnCellEditEndingRoomTypeCommand(TourHotelRoomType tourHotelRoomType)
        {
            updateTourHotel(tourHotelRoomType);
            //throw new NotImplementedException();
        }

        //After pressing the 'Save' button
        private bool OnSaveCommandCanExecute(object obj)
        {
            return IsTourDirty();
        }

        private void OnSaveCommand(object obj)
        {
            if (IsTourDirty())
            {
                ValidateModel();
            }
            if (Tour.IsValid)
            {
                removeUnNessecaryTourHotelOptionals();
                removeUnNessecaryTourHotel();
                bool bIsNew = Tour.TourId == 0;
                TourUpdated?.Invoke(this, new TourEventArgs(Tour, bIsNew));
            }
        }

        private void OnExitWithoutSavingCommand(object obj)
        {
            if (IsTourDirty())
            {
                var Result = _messageDialogService.ShowOkCancelDialog((string)Application.Current.FindResource("AreYouSureMessage"), "Question");
                if (Result == MessageDialogResult.CANCEL)
                    return;
            }
            TourCancelled?.Invoke(this, new TourEventArgs(null, true));
        }

        private bool IsTourDirty()
        {
            return Tour.IsAnythingDirty();
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
            Hotel hotel = null;
            if (Tour.TourHotels.Count > 0)
            {
                hotel = Hotels.FirstOrDefault(h => h.HotelId == Tour.TourHotels[0].Hotel.HotelId);
                SelectedHotel = hotel;
                //SelectedTourHotel = new TourHotel() { Hotel = hotel };
            }
            UpdateOptionalsInTour();
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
                OnPropertyChanged(() => TourHotelRoomTypes, true);
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

        //private TourHotel _selectedTourHotel;

        //public TourHotel SelectedTourHotel
        //{
        //    get
        //    {
        //        return _selectedTourHotel;
        //    }
        //    set
        //    {
        //        if (value != _selectedTourHotel)
        //        {
        //            _selectedTourHotel = value;
        //            SelectedHotel = null;
        //            if (_selectedTourHotel != null)
        //            {
        //                var hotel = Hotels.FirstOrDefault(h => h.HotelId == _selectedTourHotel.Hotel.HotelId);
        //                UpdateTourHotelRoomTypes(hotel);
        //            }
        //            OnPropertyChanged(() => SelectedTourHotel);
        //        }
        //    }
        //}

        //For GUI - After selecting Hotel from Drop Down
        private void UpdateTourHotelRoomTypes(Hotel hotel)
        {
            _tourHotelRoomTypes.Clear();
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

            var temp_tourHotel = Tour.TourHotels.FirstOrDefault(t => t.Hotel.HotelId == hotel.HotelId);
            if (temp_tourHotel != null)
            {
                temp_tourHotel.TourHotelRoomTypes.ToList().ForEach((tourHotelRoomType) =>
                {
                    var temp_tourHotelRoomType = TourHotelRoomTypes.FirstOrDefault(t => t.HotelRoomType.RoomTypeId == tourHotelRoomType.HotelRoomType.RoomTypeId);
                    var index = TourHotelRoomTypes.IndexOf(temp_tourHotelRoomType);
                    TourHotelRoomTypes[index] = tourHotelRoomType;
                });
            }
        }
        //Update Model
        private void updateTourHotel(TourHotelRoomType tourHotelRoomType)
        {
            if (_selectedHotel == null)
                return;
            var temp_tourHotel = Tour.TourHotels.FirstOrDefault(t => t.Hotel.HotelId == _selectedHotel.HotelId);
            if (temp_tourHotel == null)
            {
                var tourHotel = new TourHotel();
                tourHotel.HotelId = _selectedHotel.HotelId;
                tourHotel.Hotel = _selectedHotel;
                tourHotel.TourHotelRoomTypes.Add(tourHotelRoomType);
                Tour.TourHotels.Add(tourHotel);
            }
            else
            {
                var existingtourHotelRoomType = temp_tourHotel.TourHotelRoomTypes.FirstOrDefault(t => t.HotelRoomTypeId == tourHotelRoomType.HotelRoomTypeId);
                if (existingtourHotelRoomType == null)
                {
                    temp_tourHotel.TourHotelRoomTypes.Add(tourHotelRoomType);
                }
                else
                {
                    var index = temp_tourHotel.TourHotelRoomTypes.IndexOf(existingtourHotelRoomType);
                    temp_tourHotel.TourHotelRoomTypes[index] = tourHotelRoomType;
                }
            }
        }

        ////For GUI
        //private void UpdateTourOptionals()
        //{
        //    var tourOptionals = new ObservableCollection<TourOptionalWrapper>();
        //    tourOptionals.Clear();
        //    Optionals.ToList().ForEach((optional) =>
        //    {
        //        var tourOptional = new TourOptionalWrapper()
        //        {
        //            Optional = optional,
        //            OptionalId = optional.OptionalId,
        //            TourId = Tour.TourId,
        //            PriceInclusive = false,
        //            Selected = false
        //        };
        //        tourOptionals.Add(tourOptional);
        //    });
        //    //Update TourOptionals with the correct values from Tour DB
        //    tourOptionals.ToList().ForEach((tourOptional) =>
        //    {
        //        var existingTourOptional = Tour.TourOptionals.FirstOrDefault(t => t.OptionalId == tourOptional.OptionalId);
        //        if (existingTourOptional != null)
        //        {
        //            tourOptional.Selected = true;
        //            tourOptional.PriceInclusive = existingTourOptional.PriceInclusive;
        //        }
        //    });

        //    Tour.TourOptionals = tourOptionals;
        //}

        private void removeUnNessecaryTourHotel()
        {
            Tour.TourHotels.ToList().ForEach((tourHotel) =>
            {
                tourHotel.TourHotelRoomTypes.RemoveItems(t => t.Persons == 0 && t.Capacity == 0);
            });

            Tour.TourHotels.RemoveItems(t => t.TourHotelRoomTypes.Count() == 0);
        }

        private void removeUnNessecaryTourHotelOptionals()
        {
            Tour.TourOptionals.RemoveItems(t => t.Selected == false);
        }

        private void UpdateOptionalsInTour()
        {
            foreach (var tourOptional in Tour.TourOptionals)
            {
                tourOptional.Selected = true;
            }

            if (Tour.TourOptionals.Count == Optionals.Count)
                return;

            foreach (var optional in Optionals)
            {
                if (!(Tour.TourOptionals.Any(o => o.OptionalId == optional.OptionalId)))
                {
                    TourOptionalWrapper newTourOptional = new TourOptionalWrapper()
                    {
                        Selected = false,
                        Optional = optional,
                        OptionalId = optional.OptionalId,
                        TourId = Tour.TourId,
                        PriceInclusive = false
                    };
                    Tour.TourOptionals.Add(newTourOptional);
                }
            }
        }

        public event EventHandler<TourEventArgs> TourUpdated;
        public event EventHandler<TourEventArgs> TourCancelled;
    }
}
