using System.Collections.ObjectModel;
using Core.Common.Contracts;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Entities;
using System.ComponentModel;
using System.Windows.Data;
using System.Linq;
using System.Windows;
using EchoDesertTrips.Desktop.Support;
using AutoMapper;
using Core.Common.Utils;
using System.ComponentModel.Composition;
using System;
using System.Globalization;

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class TourGridViewModel : ViewModelBase
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IMessageDialogService _messageDialogService;
        //private ReservationWrapper _currentReservation;
        //private bool _editZeroTourId = false;
        [ImportingConstructor]
        public TourGridViewModel(IServiceFactory serviceFactory,
            IMessageDialogService messageBoxDialogService)
        {
            _serviceFactory = serviceFactory;
            _messageDialogService = messageBoxDialogService;
            RowExpanded = new DelegateCommand<Tour>(OnRowExpanded);
            DeleteTourCommand = new DelegateCommand<Tour>(OnDeleteTourCommand);
            EditTourCommand = new DelegateCommand<Tour>(OnEditTourCommand);
            AddTourCommand = new DelegateCommand<object>(OnAddTourCommand, CanAddTourCommand);
            _addNewEnabled = true;
            //_currentReservation = currentReservation;
        }

        public TourGridViewModel()
        {

        }

        public override string ViewTitle => "Tours";

        public DelegateCommand<Tour> DeleteTourCommand { get; set; }

        private void OnDeleteTourCommand(Tour tour)
        {
            var result = _messageDialogService.ShowOkCancelDialog((string)Application.Current.FindResource("ShortAreYouSureMessage"), "Question");
            if (result == MessageDialogResult.CANCEL)
                return;
            Tours.Remove(tour);
        }

        //public ICollectionView ItemsView { get; set; }
        public DelegateCommand<Tour> RowExpanded { get; set; }
        private bool CanAddTourCommand(object obj)
        {
            return Reservation.ReservationId == 0 && _addNewEnabled == true;
        }



        private void OnAddTourCommand(object obj)
        {
            CurrentTourViewModel = null;
            _editTourViewModel.CreateTour(null);
            _editTourViewModel.TourTypes = TourTypes;
            _editTourViewModel.Hotels = Hotels;
            _editTourViewModel.Optionals = Optionals;
            //_editTourViewModel.EnableCBTourType = true; //TODO: try to remove this property
            CurrentTourViewModel = _editTourGridViewModel;
            _addNewEnabled = false;
            RegisterEvents();
        }

        [Import]
        private EditTourGridViewModel _editTourGridViewModel { get; set; }

        private void OnEditTourCommand(Tour tour)
        {
            tour.bInEdit = true;
            _editTourViewModel.CreateTour(tour);
            _editTourViewModel.TourTypes = TourTypes;
            _editTourViewModel.Hotels = Hotels;
            _editTourViewModel.Optionals = Optionals;
            //_editTourViewModel.EnableCBTourType = Reservation.ReservationId == 0; //TODO: try to remove this property
            RegisterEvents();
        }

        public DelegateCommand<Tour> EditTourCommand { get; private set; }
        public DelegateCommand<object> AddTourCommand { get; private set; }
        [Import]
        private EditTourGridViewModel _editTourViewModel;

        public EditTourGridViewModel CurrentTourViewModel { get; set; }

        private void OnRowExpanded(Tour tour)
        {
            CurrentTour = tour;
            if (_currentTour.TourHotels != null && _currentTour.TourHotels.Count > 0)
            {
                SelectedTourHotel = _currentTour.TourHotels[0];
            }

        }

        private ObservableCollection<Tour> _tours;

        public ObservableCollection<Tour> Tours
        {
            get { return _tours; }
            set
            {
                _tours = value;
                OnPropertyChanged(() => Tours, false);
            }
        }

        private Tour _currentTour;

        public Tour CurrentTour
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

        public int Adults
        {
            get
            {
                return Reservation.Adults;
            }
            set
            {
                if (Reservation.Adults != value)
                {
                    Reservation.Adults = value;
                    OnPropertyChanged(() => Adults);
                    OnPropertyChanged(() => NumberOfCustomers);
                    OnPropertyChanged(() => TotalPrice);
                }
            }
        }

        public int Childs
        {
            get
            {
                return Reservation.Childs;
            }
            set
            {
                if (Reservation.Childs != value)
                {
                    Reservation.Childs = value;
                    OnPropertyChanged(() => Childs);
                    OnPropertyChanged(() => NumberOfCustomers);
                    OnPropertyChanged(() => TotalPrice);
                }
            }
        }

        public int Infants
        {
            get
            {
                return Reservation.Infants;
            }
            set
            {
                if (Reservation.Infants != value)
                {
                    Reservation.Infants = value;
                    OnPropertyChanged(() => Infants);
                    OnPropertyChanged(() => NumberOfCustomers);
                    OnPropertyChanged(() => TotalPrice);
                }
            }
        }

        public int NumberOfCustomers
        {
            get
            {
                return Reservation.Adults + Reservation.Childs + Reservation.Infants;
            }
        }

        public double TotalPrice
        {
            get
            {
                return Support.ReservationHelper.CalculateReservationTotalPrice(Reservation);
            }
        }

        protected override void OnViewLoaded()
        {
            _editTourViewModel.TourTypes = TourTypes;
            _editTourViewModel.Hotels = Hotels;
            _editTourViewModel.Optionals = Optionals;
            _editTourViewModel.ViewMode = ViewMode;
            if (TourHotelRoomTypes != null)
                TourHotelRoomTypes = null;
            TourHotelRoomTypes = new ObservableCollection<TourHotelRoomType>();
            Tours = Reservation.Tours;
            CurrentTourViewModel = _editTourViewModel;
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
            //var config = new MapperConfiguration(cfg => {
            //    cfg.CreateMap<TourWrapper, TourWrapper>();
            //});

            //var tourWrapper = TourHelper.CloneTourWrapper(e.Tour);
            var tour_e = e.Tour;
            if (!e.IsNew)
            {
                //This is done in order to update the Grid. Remember that in EditTripViewModel the updated trip
                //Is a temporary object and it is not part of the Grid collection trips.
                tour_e.bInEdit = false;
                var tour = Tours.FirstOrDefault(item => item.bInEdit == true);
                if (tour != null)
                {
                    var index = Tours.IndexOf(tour);
                    Tours[index] = tour_e;
                }
            }
            else
            {
                Tours.Add(tour_e);
            }
            Tours.OrderBy(tour => tour.StartDate);
            //_editTourGridViewModel.EnableCBTourType = Reservation.ReservationId == 0;
            OnPropertyChanged(() => TotalPrice);
            //TourUpdatedFinished?.Invoke(this, new EventArgs());
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

        //public event EventHandler TourUpdatedFinished;
    }

    public class DeleteTourConverter : IValueConverter
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
}
