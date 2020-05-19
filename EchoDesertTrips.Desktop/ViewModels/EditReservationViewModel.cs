using Core.Common.Contracts;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using EchoDesertTrips.Desktop.Support;
using static Core.Common.Core.Const;
using System.Windows;
using System.ComponentModel.Composition;
using Core.Common.UI.PubSubEvent;
using Core.Common.UI.CustomEventArgs;

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class EditReservationViewModel : ViewModelBase
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IMessageDialogService _messageDialogService;
        [ImportingConstructor]
        public EditReservationViewModel(IServiceFactory serviceFactory,
            IMessageDialogService messageDialogService)
        {
            _serviceFactory = serviceFactory;
            _messageDialogService = messageDialogService;
            SaveCommand = new DelegateCommand<object>(OnSaveCommand, OnSaveCommandCanExecute);
            ExitWithoutSavingCommand = new DelegateCommand<object>(OnExitWithoutSavingCommand);
            _eventAggregator.GetEvent<ReservationEditSelectedFinishedEvent>().Subscribe(ReservationEditSelectedFinished);
        }

        [Import]
        public TourGridViewModel TourGridViewModel { get; set; }
        [Import]
        public CustomerGridViewModel CustomerGridViewModel { get; set; }
        [Import]
        public GeneralReservationViewModel GeneralReservationViewModel { get; set; }

        public DelegateCommand<object> SaveCommand { get; }
        public DelegateCommand<object> ExitWithoutSavingCommand { get; }


        private bool _lastDertinessValue = false;

        private bool IsReservationDirty()
        {
            var bDirty = (Reservation.IsAnythingDirty() && Reservation.Tours.Count > 0 && (!ViewMode));
            if (bDirty != _lastDertinessValue)
            {
                log.Debug($"Dirty = {bDirty}");
                _lastDertinessValue = bDirty;
            }
            return bDirty;
        }

        //After pressing the 'Save' button
        private bool OnSaveCommandCanExecute(object obj)
        {
            return IsReservationDirty();
        }

        private void OnSaveCommand(object obj)
        {
            ValidateModel();
            if (Reservation.IsValid)
            {
                Reservation.Operator = CurrentOperator.Operator;
                Reservation.OperatorId = CurrentOperator.Operator.OperatorId;
                ReservationHelper.CreateExternalId(Reservation);
                Reservation.ActualNumberOfCustomers = Reservation.Customers.Count;
                Reservation.TotalPrice = ReservationHelper.CalculateReservationTotalPrice(Reservation);
                Reservation.Lock = false;

                if (Reservation.ReservationId == 0) //New Reservation
                {
                    WithClient<IOrderService>(_serviceFactory.CreateClient<IOrderService>(), reservationClient =>
                    {
                        var reservation = reservationClient.UpdateReservation(Reservation); //Update or Add
                        if (reservation != null)
                        {
                            _eventAggregator.GetEvent<ReservationUpdatedAndNotifyClientsEvent>().Publish(new ReservationEventArgs(reservation, true, false));
                        }
                    });
                }
                else if (IsReservationDirty())
                {
                    WithClient(_serviceFactory.CreateClient<IOrderService>(), reservationClient =>
                    {
                        var reservationLocal = Reservation;
                        var reservation = reservationClient.UpdateReservation(Reservation); //Add or Update but in this case its Update
                        if (reservation.RowVersionConflict)
                        {
                            log.Error("Cannot save reservation. reservation.RowVersionConflict is true");
                            var operatorName = reservation.Operator != null ? reservation.Operator.OperatorName : string.Empty;
                            var message = $"Reservation was saved by {operatorName}.\n\nPress \"OK\" to override the changes made by {0}.\nPress \"Cancel\" to cancel your changes.";
                            var result = _messageDialogService.ShowOkCancelDialog(message, "Info");
                            if (result == MessageDialogResult.OK)//Client Win
                            {
                                reservationLocal.RowVersion = reservation.RowVersion;
                                reservation = reservationClient.UpdateReservation(reservationLocal);
                            }
                        }
                        _eventAggregator.GetEvent<ReservationUpdatedAndNotifyClientsEvent>().Publish(new ReservationEventArgs(reservation, false, false));
                    });
                }
            }
            else
            {
                log.Error($"Cannot save reservation. reservation ID = {Reservation.ReservationId} is not valid.");
            }
        }

        private void UnLock(int ReservationID)
        {
            WithClient(_serviceFactory.CreateClient<IOrderService>(), reservationClient =>
            {
                reservationClient.UnLock(ReservationID);
            });
        }

        private void OnExitWithoutSavingCommand(object obj)
        {
            if (IsReservationDirty())
            {
                var result = _messageDialogService.ShowOkCancelDialog((string)Application.Current.FindResource("AreYouSureMessage"), "Question");
                if (result == MessageDialogResult.CANCEL)
                    return;
            }
            if (!ViewMode && Reservation.ReservationId != 0)
                UnLock(Reservation.ReservationId);
            _eventAggregator.GetEvent<ReservationCancelledEvent>().Publish(new ReservationEventArgs(null, true, false));
        }

        protected override void OnViewLoaded()
        {
            _eventAggregator.GetEvent<HotelUpdatedEvent>().Subscribe(HotelUpdated);
        }

        private void OnViewUnloaded(object obj)
        {
            _eventAggregator.GetEvent<HotelUpdatedEvent>().Unsubscribe(HotelUpdated);
        }

        private void ReservationEditSelectedFinished(EditReservationEventArgs e)
        {
            if (e.Reservation == null)
            {
                Reservation = new Reservation();
                var tmpEditReservationEventArgs = new EditReservationEventArgs(e.Reservation, e.ViewMode, e.IsContinual);
                e = new EditReservationEventArgs(Reservation, tmpEditReservationEventArgs.ViewMode, tmpEditReservationEventArgs.IsContinual);
            }
            else
                Reservation = e.Reservation;

            SelectedTabIndex = e.IsContinual == false ? 0 : 2;
            CleanAll();
            _eventAggregator.GetEvent<ReservationEditedEvent>().Publish(e);
        }

        private void HotelUpdated(HotelEventArgs e)
        {
            if (Reservation != null)
            {
                foreach (var tour in Reservation.Tours)
                {
                    foreach (var tourHotel in tour.TourHotels)
                    {
                        if (tourHotel.Hotel.HotelId == e.Hotel.HotelId)
                        {
                            tourHotel.Hotel.HotelAddress = e.Hotel.HotelAddress;
                            tourHotel.Hotel.HotelName = e.Hotel.HotelName;
                        }
                    }
                }
            }
        }

        private int _selectedTabIndex;
        public int SelectedTabIndex
        {
            get
            {
                return _selectedTabIndex;
            }
            set
            {
                _selectedTabIndex = value;
                OnPropertyChanged(() => SelectedTabIndex);
            }
        }
    }
}
