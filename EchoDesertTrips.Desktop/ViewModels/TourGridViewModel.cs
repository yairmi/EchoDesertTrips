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

namespace EchoDesertTrips.Desktop.ViewModels
{
    public class TourGridViewModel : ViewModelBase
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IMessageDialogService _messageDialogService;

        public TourGridViewModel(IServiceFactory serviceFactory, 
            IMessageDialogService messageBoxDialogService, 
            ObservableCollection<TourWrapper> tours)
        {
            _serviceFactory = serviceFactory;
            _messageDialogService = messageBoxDialogService;
            RowExpanded = new DelegateCommand<TourWrapper>(OnRowExpanded);
            DeleteTourCommand = new DelegateCommand<TourWrapper>(OnDeleteTourCommand);
            EditTourCommand = new DelegateCommand<TourWrapper>(OnEditTourCommand);
            AddTourCommand = new DelegateCommand<object>(OnAddTourCommand);
            TourHotelRoomTypes = new ObservableCollection<TourHotelRoomType>();
            Tours = new ObservableCollection<TourWrapper>();
            Tours = tours;
        }

        public TourGridViewModel()
        {

        }

        public DelegateCommand<TourWrapper> DeleteTourCommand { get; set; }

        private void OnDeleteTourCommand(TourWrapper tour)
        {
            var Result = _messageDialogService.ShowOkCancelDialog((string)Application.Current.FindResource("ShortAreYouSureMessage"), "Question");
            if (Result == MessageDialogResult.CANCEL)
                return;
            Tours.Remove(tour);
        }

        //private void OnAddOptionalCommand(Optional obj)
        //{
        //    throw new NotImplementedException();
        //}

        //public DelegateCommand<Optional> AddOptionalCommand { get; set; }

        public ICollectionView ItemsView { get; set; }

        //public DelegateCommand<TourWrapper> RowEditEndingCommand { get; set; }
        //public DelegateCommand<object> RowSelectedCommand { get; set; }
        public DelegateCommand<TourWrapper> RowExpanded { get; set; }

//        private TourWrapper LastUpdatedTour;

        private void OnAddTourCommand(object obj)
        {
            CurrentTourViewModel = new EditTourGridViewModel(_serviceFactory, _messageDialogService, null);
            CurrentTourViewModel.TourTypes = TourTypes;
            CurrentTourViewModel.Hotels = Hotels;
            CurrentTourViewModel.Optionals = Optionals;
            RegisterEvents();
        }

        private void OnEditTourCommand(TourWrapper tour)
        {
            CurrentTourViewModel = new EditTourGridViewModel(_serviceFactory, _messageDialogService, tour);
            CurrentTourViewModel.TourTypes = TourTypes;
            CurrentTourViewModel.Hotels = Hotels;
            CurrentTourViewModel.Optionals = Optionals;
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
            Hotel hotel = null;
            if (_currentTour.TourHotels != null && _currentTour.TourHotels.Count > 0)
            {
                hotel = Hotels.FirstOrDefault(h => h.HotelId == _currentTour.TourHotels[0].Hotel.HotelId);
                SelectedTourHotel = new TourHotel() { Hotel = hotel };
            }

        }

        public ObservableCollection<TourWrapper> GetTours() { return Tours; }

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

        //private Hotel _selectedHotel;

        //public Hotel SelectedHotel
        //{
        //    get
        //    {
        //        return _selectedHotel;
        //    }
        //    set
        //    {
        //        if (value != _selectedHotel)
        //        {
        //            _selectedHotel = value;
        //            UpdateTourHotelRoomTypes();
        //            OnPropertyChanged(() => SelectedHotel);
        //        }
        //    }
        //}

        private TourHotel _selectedTourHotel;

        public TourHotel SelectedTourHotel
        {
            get
            {
                return _selectedTourHotel;
            }
            set
            {
                if (value != _selectedTourHotel)
                {
                    _selectedTourHotel = value;
                    UpdateTourHotelRoomTypes();
                    OnPropertyChanged(() => SelectedTourHotel);
                }
            }
        }

        //private ObservableCollection<Hotel> _currentTourHotels;

        //public ObservableCollection<Hotel> CurrentTourHotels
        //{
        //    get
        //    {
        //        return _currentTourHotels;
        //    }
        //    set
        //    {
        //        _currentTourHotels = value;
        //        OnPropertyChanged(() => CurrentTourHotels);
        //    }
        //}

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

        public Optional SelectedItem
        {
            get;
            set;
        }

        protected override void OnViewLoaded()
        {
            ItemsView = CollectionViewSource.GetDefaultView(Tours);
            ItemsView.GroupDescriptions.Add(new PropertyGroupDescription("TourId"));
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
            var tourWrapper = iMapper.Map<TourWrapper, TourWrapper>(e.Tour);

            if (!e.IsNew)
            {
                //This is done in order to update the Grid. Remember that in EditTripViewModel the updated trip
                //Is a temporary object and it is not part of the Grid collection trips.
                var tour = Tours.FirstOrDefault(item => item.TourId == e.Tour.TourId);
                if (tour != null)
                {
                    var index = Tours.IndexOf(tour);
                    Tours[index] = tourWrapper;
                }
            }
            else
            {
                Tours.Add(tourWrapper);
            }
            Tours = new ObservableCollection<TourWrapper>(Tours.OrderBy(tour => tour.StartDate));
            //ReservationsView.Refresh();
            //try
            //{
            //    _client.NotifyServer(new EventDataType()
            //    {
            //        ClientName = Operator.OperatorName + "-" + Operator.OperatorId,
            //        EventMessage = "Stam"
            //    });
            //}
            //catch (Exception ex)
            //{
            //    log.Error("CurrentReservationViewModel_ReservationUpdated: Failed to notify server");
            //}
            CurrentTourViewModel = null;
        }

        private void CurrentTourViewModel_TourCancelled(object sender, TourEventArgs e)
        {
            CurrentTourViewModel = null;
        }

        private void RegisterEvents()
        {
            CurrentTourViewModel.TourUpdated -= CurrentTourViewModel_TourUpdated;
            CurrentTourViewModel.TourUpdated += CurrentTourViewModel_TourUpdated;
            CurrentTourViewModel.TourCancelled -= CurrentTourViewModel_TourCancelled;
            CurrentTourViewModel.TourCancelled += CurrentTourViewModel_TourCancelled;
        }
    }
    /*public class TourValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value,
            System.Globalization.CultureInfo cultureInfo)
        {
            TourWrapper tour = (value as BindingGroup).Items[0] as TourWrapper;
            if (tour.TourType.TourTypeId == 0)
            {
                return new ValidationResult(false,
                    "Tour Type should not be empty");
            }
            else
            if (tour.PickupAddress == string.Empty)
            {
                return new ValidationResult(false,
                    "PickUp Address should not be empty.");
            }
            else
            if (tour.StartDate > tour.EndDate)
            {
                return new ValidationResult(false,
                    "Start Date must be earlier than End Date.");
            }
            else
            {
                var validationResult = ValidationResult.ValidResult;
                return validationResult;
            }
        }
    }*/
}
