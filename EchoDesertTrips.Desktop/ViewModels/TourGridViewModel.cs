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
using EchoDesertTrips.Client.Contracts;

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
            DeleteTourCommand = new DelegateCommand<Tour>(OnDeleteTourCommand);
            EditTourCommand = new DelegateCommand<Tour>(OnEditTourCommand);
            _eventAggregator.GetEvent<ReservationEditedEvent>().Subscribe(ReservationEdited);
            _eventAggregator.GetEvent<TourUpdatedEvent>().Subscribe(TourUpdated);
            _eventAggregator.GetEvent<TourCancelledEvent>().Subscribe(TourCancelled);
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

        private void OnEditTourCommand(Tour tour)
        {
            tour.bInEdit = true;
            _eventAggregator.GetEvent<TourEditedEvent>().Publish(tour);
        }

        public DelegateCommand<Tour> EditTourCommand { get; private set; }

        [Import]
        private EditTourGridViewModel _editTourGridViewModel { get; set; }

        public EditTourGridViewModel CurrentTourViewModel { get; set; }

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

        public ObservableCollection<TourHotelRoomType> TourHotelRoomTypes { get; set; }

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

        public decimal TotalPrice
        {
            get
            {
                return Support.ReservationHelper.CalculateReservationTotalPrice(Reservation);
            }
        }

        protected override void OnViewLoaded()
        {
            Tours = Reservation.Tours;
            CurrentTourViewModel = _editTourGridViewModel;
        }

        public override void OnViewUnloaded()
        {
            ;
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
        }

        private void TourCancelled(TourEventArgs e)
        {
            CurrentTourViewModel = null;
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
