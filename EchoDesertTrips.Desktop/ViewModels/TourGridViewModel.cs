using System.Collections.ObjectModel;
using Core.Common.Contracts;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Entities;
using System.Windows.Data;
using System.Linq;
using System.Windows;
using Core.Common.Utils;
using System.ComponentModel.Composition;
using System;
using System.Globalization;
using Core.Common.UI.PubSubEvent;
using Core.Common.UI.CustomEventArgs;

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
            _eventAggregator.GetEvent<ReservationEditedEvent>().Subscribe(ReservationEdited);
            _eventAggregator.GetEvent<TourUpdatedEvent>().Subscribe(TourUpdated);
            _eventAggregator.GetEvent<TourCancelledEvent>().Subscribe(TourCancelled);
            _addNewEnabled = true;
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

        private void OnEditTourCommand(Tour tour)
        {
            tour.bInEdit = true;
            _eventAggregator.GetEvent<TourEditedEvent>().Publish(tour);
        }

        public DelegateCommand<Tour> EditTourCommand { get; private set; }

        [Import]
        private EditTourGridViewModel _editTourGridViewModel { get; set; }

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
            if (TourHotelRoomTypes != null)
                TourHotelRoomTypes = null;
            TourHotelRoomTypes = new ObservableCollection<TourHotelRoomType>();
            Tours = Reservation.Tours;
            CurrentTourViewModel = _editTourGridViewModel;
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

        private void ReservationEdited(EditReservationEventArgs e)
        {
            Reservation = e.Reservation;
            ViewMode = e.ViewMode;
        }

        private void TourUpdated(TourEventArgs e)
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
                _eventAggregator.GetEvent<PropertyRemovedFromTourEvent>().Publish(null);
            }
        }

        private void TourCancelled(TourEventArgs e)
        {
            CurrentTourViewModel = null;
            _addNewEnabled = true;
        }
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
