using Core.Common.Contracts;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using EchoDesertTrips.Desktop.Support;
using System;
using System.Windows;
using System.ComponentModel.Composition;
using EchoDesertTrips.Desktop.CustomEventArgs;
using System.Threading.Tasks;

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
            var bDirty = SomethingDeleted || (Reservation.IsAnythingDirty() && Reservation.Tours.Count > 0 && (!ViewMode));
            if (bDirty != _lastDertinessValue)
            {
                log.Debug("EditOrderViewModel dirty = " + bDirty);
                _lastDertinessValue = bDirty;
            }
            return bDirty;
        }

        //After pressing the 'Save' button
        private bool OnSaveCommandCanExecute(object obj)
        {
            return IsReservationDirty();
        }

        public event EventHandler<ReservationEventArgs> ReservationUpdated;

        private void OnSaveCommand(object obj)
        {
            ValidateModel();
            if (Reservation.IsValid)
            {
                Reservation.Operator = Operator;
                Reservation.OperatorId = Operator.OperatorId;
                ReservationHelper.CreateExternalId(Reservation);
                Reservation.TotalPrice = ReservationHelper.CalculateReservationTotalPrice(Reservation);
                Reservation.Lock = false;
                int exceptionPosition = 0;

                if (Reservation.ReservationId == 0) //New Reservation
                {
                    WithClient<IOrderService>(_serviceFactory.CreateClient<IOrderService>(), reservationClient =>
                    {
                        exceptionPosition = 1;
                        var reservation = reservationClient.UpdateReservation(Reservation); //Update or Add
                        exceptionPosition = 2;
                        if (reservation != null)
                        {
                            ReservationUpdated?.Invoke(this, new ReservationEventArgs(reservation, true, false));
                        }
                    }, "OnSaveCommand", exceptionPosition);
                }
                else if (IsReservationDirty())
                {
                    WithClient(_serviceFactory.CreateClient<IOrderService>(), reservationClient =>
                    {
                        exceptionPosition = 3;
                        var reservationLocal = Reservation;
                        var reservation = reservationClient.UpdateReservation(Reservation); //Add or Update but in this case its Update
                        if (reservation.RowVersionConflict)
                        {
                            var operatorName = reservation.Operator != null ? reservation.Operator.OperatorName : string.Empty;
                            var message = string.Format("Reservation was saved by {0}.\n\nPress \"OK\" to override the changes made by {0}.\nPress \"Cancel\" to cancel your changes.", operatorName);
                            var result = _messageDialogService.ShowOkCancelDialog(message, "Info");
                            if (result == MessageDialogResult.OK)//Client Win
                            {
                                reservationLocal.RowVersion = reservation.RowVersion;
                                reservation = reservationClient.UpdateReservation(reservationLocal);
                            }
                        }
                        exceptionPosition = 4;
                        ReservationUpdated?.Invoke(this, new ReservationEventArgs(reservation, false, false));
                    }, "OnSaveCommand", exceptionPosition);
                }
            }
        }

        private void UnLock(int ReservationID)
        {
            WithClient(_serviceFactory.CreateClient<IOrderService>(), reservationClient =>
            {
                reservationClient.UnLock(ReservationID);
            }, "UnLock");
        }

        public event EventHandler<ReservationEventArgs> ReservationCancelled;

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
            ReservationCancelled?.Invoke(this, new ReservationEventArgs(null, true, false));
        }

        protected override void OnViewLoaded()
        {
            log.Debug("EditOrderViewModel OnViewLoaded start");
            TourGridViewModel.TourTypes = TourTypes;
            TourGridViewModel.Hotels = Hotels;
            TourGridViewModel.Optionals = Optionals;
            TourGridViewModel.Reservation = Reservation;
            TourGridViewModel.ViewMode = ViewMode;

            CustomerGridViewModel.Reservation = Reservation;
            CustomerGridViewModel.ViewMode = ViewMode;

            GeneralReservationViewModel.Reservation = Reservation;
            GeneralReservationViewModel.Agencies = Agencies;

            SomethingDeleted = false;

            CustomerGridViewModel.CustomerDeleted -= EditReservationViewModel_SomethingRemoved;
            CustomerGridViewModel.CustomerDeleted += EditReservationViewModel_SomethingRemoved;

            TourGridViewModel.PropertyRemovedFromTour -= EditReservationViewModel_SomethingRemoved;
            TourGridViewModel.PropertyRemovedFromTour += EditReservationViewModel_SomethingRemoved;

            log.Debug("EditOrderViewModel OnViewLoaded end");
        }

        private void EditReservationViewModel_SomethingRemoved(object sender, EventArgs e)
        {
            SomethingDeleted = true;
        }

        public void SetReservation(Reservation reservation)
        {
            Reservation = reservation == null ? new Reservation() : reservation;
            CleanAll();
        }

        public bool SomethingDeleted;
    }
}
