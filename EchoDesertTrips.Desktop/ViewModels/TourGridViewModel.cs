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
        [ImportingConstructor]
        public TourGridViewModel(IServiceFactory serviceFactory,
            IMessageDialogService messageBoxDialogService)
        {
            _serviceFactory = serviceFactory;
            _messageDialogService = messageBoxDialogService;
            RowExpanded = new DelegateCommand<Tour>(OnRowExpanded);
            DeleteTourCommand = new DelegateCommand<Tour>(OnDeleteTourCommand);
            EditTourCommand = new DelegateCommand<Tour>(OnEditTourCommand);
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

        [Import]
        private EditTourGridViewModel _editTourGridViewModel { get; set; }

        private void OnEditTourCommand(Tour tour)
        {
            tour.bInEdit = true;
            _editTourViewModel.CreateTour(tour);
            RegisterEvents();
        }

        public DelegateCommand<Tour> EditTourCommand { get; private set; }

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

        public int ReservationID
        {
            get
            {
                return Reservation.ReservationId;
            }
            set
            {
                OnPropertyChanged(() => ReservationID);
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
            //var tour_e = e.Tour;
            if (!e.IsNew)
            {
                //This is done in order to update the Grid. Remember that in EditTripViewModel the updated trip
                //Is a temporary object and it is not part of the Grid collection trips.
                e.Tour.bInEdit = false;
                var tour = Tours.FirstOrDefault(item => item.bInEdit == true);
                if (tour != null)
                {
                    var index = Tours.IndexOf(tour);
                    Tours[index] = e.Tour;
                }
            }
            else
            {
                Tours.Add(e.Tour);
            }
            Tours.OrderBy(tour => tour.StartDate);
            OnPropertyChanged(() => TotalPrice);
            if (e.RemovedItems > 0)
            {
                PropertyRemovedFromTour?.Invoke(this, null);
            }
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

        public event EventHandler PropertyRemovedFromTour;
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
