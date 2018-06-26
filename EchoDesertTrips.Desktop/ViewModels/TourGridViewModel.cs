using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Core.Common.Contracts;
using Core.Common.Core;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Entities;
using System.ComponentModel;
using System.Windows.Data;
using System.Linq;
using System.Windows.Controls;
using System.Windows;
using EchoDesertTrips.Desktop.Support;
using AutoMapper;
using Core.Common.Utils;
using Core.Common.Extensions;
using System.ComponentModel.Composition;

namespace EchoDesertTrips.Desktop.ViewModels
{
    public class TourGridViewModel : ViewModelBase
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IMessageDialogService _messageDialogService;
        private ReservationWrapper _currentReservation;
        private bool _editZeroTourId = false;

        public TourGridViewModel(IServiceFactory serviceFactory,
            IMessageDialogService messageBoxDialogService,
            ReservationWrapper currentReservation)
        {
            _serviceFactory = serviceFactory;
            _messageDialogService = messageBoxDialogService;
            RowExpanded = new DelegateCommand<TourWrapper>(OnRowExpanded);
            DeleteTourCommand = new DelegateCommand<TourWrapper>(OnDeleteTourCommand);
            EditTourCommand = new DelegateCommand<TourWrapper>(OnEditTourCommand);
            AddTourCommand = new DelegateCommand<object>(OnAddTourCommand, CanAddTourCommand);
            _addNewEnabled = true;
            _currentReservation = currentReservation;
        }

        public TourGridViewModel()
        {

        }

        public override string ViewTitle => "Tours";

        public DelegateCommand<TourWrapper> DeleteTourCommand { get; set; }

        private void OnDeleteTourCommand(TourWrapper tour)
        {
            var result = _messageDialogService.ShowOkCancelDialog((string)Application.Current.FindResource("ShortAreYouSureMessage"), "Question");
            if (result == MessageDialogResult.CANCEL)
                return;
            Tours.Remove(tour);
        }

        public ICollectionView ItemsView { get; set; }
        public DelegateCommand<TourWrapper> RowExpanded { get; set; }
        private bool CanAddTourCommand(object obj)
        {
            return _reservation.ReservationId == 0 && _addNewEnabled == true;
        }



        private void OnAddTourCommand(object obj)
        {
            CurrentTourViewModel = null;

            CurrentTourViewModel = new EditTourGridViewModel(_serviceFactory, _messageDialogService, null,
                _reservation.ReservationId == 0)
            {
                TourTypes = TourTypes,
                Hotels = Hotels,
                Optionals = Optionals
            };
            _addNewEnabled = false;
            RegisterEvents();
        }

        private void OnEditTourCommand(TourWrapper tour)
        {
            if (tour.TourId == 0)
            {
                tour.TourId = Tours.Max(x => x.TourId) + 1;
                _editZeroTourId = true;
            }
            else
                _editZeroTourId = false;

            CurrentTourViewModel = null;

            CurrentTourViewModel = new EditTourGridViewModel(_serviceFactory, _messageDialogService, tour,
                _reservation.ReservationId == 0)
            {
                TourTypes = TourTypes,
                Hotels = Hotels,
                Optionals = Optionals
            };
            RegisterEvents();
        }

        public DelegateCommand<TourWrapper> EditTourCommand { get; private set; }
        public DelegateCommand<object> AddTourCommand { get; private set; }

        private EditTourGridViewModel _editTourViewModel;

        public EditTourGridViewModel CurrentTourViewModel
        {
            get { return _editTourViewModel; }
            set
            {
                if (_editTourViewModel != value)
                {
                    _editTourViewModel = value;
                    OnPropertyChanged(() => CurrentTourViewModel, false);
                }
            }
        }

        private void OnRowExpanded(TourWrapper tour)
        {
            CurrentTour = tour;
            if (_currentTour.TourHotels != null && _currentTour.TourHotels.Count > 0)
            {
                SelectedTourHotel = _currentTour.TourHotels[0];
            }

        }

        private ObservableCollection<TourWrapper> _tours;

        public ObservableCollection<TourWrapper> Tours
        {
            get { return _tours; }
            set
            {
                _tours = value;
                OnPropertyChanged(() => Tours, false);
            }
        }

        private TourWrapper _currentTour;

        public TourWrapper CurrentTour
        {
            get
            {
                return _currentTour;
            }
            set
            {
                _currentTour = value;
                OnPropertyChanged(() => CurrentTour);
            }
        }

        private TourHotel _selectedTourHotel;

        public TourHotel SelectedTourHotel
        {
            get
            {
                return _selectedTourHotel;
            }
            set
            {
                if (value != _selectedTourHotel && value != null)
                {
                    _selectedTourHotel = value;
                    UpdateTourHotelRoomTypes();
                    OnPropertyChanged(() => SelectedTourHotel);
                }
            }
        }

        private bool _addNewEnabled;

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

        private ReservationWrapper _reservation;

        public ReservationWrapper Reservation
        {
            get
            {
                return _reservation;
            }
            set
            {
                _reservation = value;
                OnPropertyChanged(() => Reservation);
            }
        }

        protected override void OnViewLoaded()
        {
            TourHotelRoomTypes = new ObservableCollection<TourHotelRoomType>();
            Reservation = _currentReservation;
            Tours = Reservation.Tours;
            Tours.ToList().ForEach(InitTourOptionals);
            //Reservation.CleanAll();
            ItemsView = CollectionViewSource.GetDefaultView(Tours);
            ItemsView.GroupDescriptions.Add(new PropertyGroupDescription("TourId"));
            CurrentTourViewModel = new EditTourGridViewModel(_serviceFactory, _messageDialogService, null,
                Reservation.ReservationId == 0)
            {
                TourTypes = TourTypes,
                Hotels = Hotels,
                Optionals = Optionals
            };
            RegisterEvents();
        }

        //For GUI - After selecting Hotel from Drop Down
        private void UpdateTourHotelRoomTypes()
        {
            TourHotelRoomTypes.Clear();
            var temp_tourHotel = _currentTour.TourHotels.FirstOrDefault(t => t.Hotel.HotelId == _selectedTourHotel.HotelId);
            if (temp_tourHotel != null)
            {
                temp_tourHotel.TourHotelRoomTypes.ToList().ForEach((tourHotelRoomType) =>
                {
                    TourHotelRoomTypes.Add(AutoMapperUtil.Map<TourHotelRoomType, TourHotelRoomType>(tourHotelRoomType));
                });
            }
        }

        private void CurrentTourViewModel_TourUpdated(object sender, TourEventArgs e)
        {
            var config = new MapperConfiguration(cfg => {
                cfg.CreateMap<TourWrapper, TourWrapper>();
            });

            IMapper iMapper = config.CreateMapper();
            var tourWrapper = TourHelper.CloneTourWrapper(e.Tour);
            if (!e.IsNew || _editZeroTourId == true)
            {
                //This is done in order to update the Grid. Remember that in EditTripViewModel the updated trip
                //Is a temporary object and it is not part of the Grid collection trips.
                var tour = Tours.FirstOrDefault(item => item.TourId == e.Tour.TourId);
                if (tour != null)
                {
                    var index = Tours.IndexOf(tour);
                    Tours[index] = tourWrapper;
                    if (_editZeroTourId == true)
                        tourWrapper.TourId = 0;
                }
            }
            else
            {
                Tours.Add(tourWrapper);
            }
            Tours.OrderBy(tour => tour.StartDate);
            //AddNewEnabled = true;
            //CurrentTourViewModel = null;
        }

        private void CurrentTourViewModel_TourCancelled(object sender, TourEventArgs e)
        {
            CurrentTourViewModel = null;
            _addNewEnabled = true;
        }

        private void RegisterEvents()
        {
            CurrentTourViewModel.TourUpdated -= CurrentTourViewModel_TourUpdated;
            CurrentTourViewModel.TourUpdated += CurrentTourViewModel_TourUpdated;
            CurrentTourViewModel.TourCancelled -= CurrentTourViewModel_TourCancelled;
            CurrentTourViewModel.TourCancelled += CurrentTourViewModel_TourCancelled;
        }
    }
}
