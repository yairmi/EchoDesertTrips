using Core.Common.Contracts;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using EchoDesertTrips.Desktop.Support;
using System;
using System.Windows;
using System.ComponentModel.Composition;
using EchoDesertTrips.Desktop.CustomEventArgs;

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

#if DEBUG
        private bool _lastDertinessValue = false;
#endif

        private bool IsReservationDirty()
        {
            var bDirty = Reservation.IsAnythingDirty();
#if DEBUG
            if (bDirty != _lastDertinessValue)
            {
                log.Debug("EditOrderViewModel dirty = " + bDirty);
                _lastDertinessValue = bDirty;
            }
#endif
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
                ReservationUtils.CreateExternalId(Reservation);
                //ReservationUtils.RemoveUnselectedHotels(Reservation);
                //ReservationUtils.RemoveUnselectedOptionals(Reservation);
                int exceptionPosition = 0;
                if (Reservation.ReservationId == 0) //New Reservation
                {
                    WithClient<IOrderService>(_serviceFactory.CreateClient<IOrderService>(), reservationClient =>
                    {
                        try
                        {
                            exceptionPosition = 1;
                            var reservation = ReservationHelper.CreateReservation(Reservation);
                            exceptionPosition = 2;
                            var reservationData = reservationClient.UpdateReservation(reservation); //Update or Add
                            exceptionPosition = 3;
                            if (reservationData.DbReservation != null)
                            {
                                exceptionPosition = 4;
                                var reservationWrapper = ReservationHelper.CreateReservationWrapper(reservationData.DbReservation);
                                exceptionPosition = 5;
                                ReservationUpdated?.Invoke(this, new ReservationEventArgs(reservationWrapper, true, false));
                                exceptionPosition = 6;
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error("OnSaveCommand failed adding new reservation. position: " + exceptionPosition + ". Error: " + ex.Message);
                        }

                    });
                }
                else if (IsReservationDirty())
                {
                    WithClient(_serviceFactory.CreateClient<IOrderService>(), reservationClient =>
                    {
                        try
                        {
                            exceptionPosition = 1;
                            var reservation = ReservationHelper.CreateReservation(Reservation);
                            exceptionPosition = 2;
                            var reservationData = reservationClient.UpdateReservation(reservation); //Add or Update but in this case its Update
                            exceptionPosition = 3;
                            ReservationWrapper reservationWrapper;

                            if (reservationData.InEdit)
                            {
                                var inEdit = true;
                                while (inEdit)
                                {
                                    string message;
                                    if (reservationData.DbReservation == null)
                                    {
                                        log.Info("Reservation was deleted by someone else. ReservationID = " +
                                                 reservation.ReservationId);

                                        message =
                                            "The Reservation was deleted by someone else.\nClick OK to save your changes anyway,\nClick Cancel to discard your changes.";
                                    }
                                    else
                                    {
                                        log.Info("Reservation was edited by someone else. ReservationID = " +
                                                 reservation.ReservationId);
                                        message =
                                            $"The Reservation has been changed In the meantime by {reservationData.DbReservation.Operator.OperatorName} {".\nClick OK to save your changes anyway,\nClick Cancel to reload the entity from the database."}";
                                    }

                                    var result = _messageDialogService.ShowOkCancelDialog(message, "Question");
                                    //Client win
                                    if (result == MessageDialogResult.OK)
                                    {
                                        if (reservationData.DbReservation != null) //Update existing record
                                        {
                                            reservation.RowVersion = reservationData.DbReservation.RowVersion;
                                            exceptionPosition = 4;
                                            reservationData = reservationClient.UpdateReservation(reservation);
                                            if (!reservationData.InEdit)
                                            {
                                                reservationWrapper = ReservationHelper.CreateReservationWrapper(reservation);
                                                exceptionPosition = 5;
                                                ReservationUpdated?.Invoke(this,
                                                    new ReservationEventArgs(reservationWrapper, false, false));
                                                exceptionPosition = 6;
                                                inEdit = false;
                                            }
                                        }
                                        else //Insert Deleted (By other user) record
                                        {
                                            reservation.ReservationId = 0;
                                            reservation.RowVersion = null;

                                            var newReservation = reservationClient.UpdateReservation(reservation);
                                            if (newReservation.DbReservation != null)
                                            {
                                                exceptionPosition = 7;
                                                var newReservationWrapper =
                                                    ReservationHelper.CreateReservationWrapper(newReservation
                                                        .DbReservation);
                                                exceptionPosition = 8;
                                                ReservationUpdated?.Invoke(this,
                                                    new ReservationEventArgs(newReservationWrapper, true, false));
                                            }

                                            inEdit = false;
                                        }

                                    }
                                    //Data base win
                                    else
                                    {
                                        if (reservationData.DbReservation != null)
                                        {
                                            log.Info("DB Win. ReservationID = " + reservation.ReservationId);
                                            reservationWrapper =
                                                ReservationHelper.CreateReservationWrapper(reservationData
                                                    .DbReservation);
                                            ReservationUpdated?.Invoke(this,
                                                new ReservationEventArgs(reservationWrapper, false,
                                                    true)); //The last parameter is true since the record was retrieved from DB and there is no need to update other clients
                                        }
                                        else
                                        {
                                            exceptionPosition = 9;
                                            ReservationCancelled?.Invoke(this, new ReservationEventArgs(null, true, false));
                                            exceptionPosition = 10;
                                        }

                                        inEdit = false;
                                    }
                                }
                            }
                            else
                            {
                                exceptionPosition = 11;
                                reservationWrapper = ReservationHelper.CreateReservationWrapper(reservationData.DbReservation); //rw.CopyReservation(reservationData.DbReservation);
                                ReservationUpdated?.Invoke(this, new ReservationEventArgs(reservationWrapper, false, false));
                                exceptionPosition = 12;
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error("OnSaveCommand failed update reservation. position: " + exceptionPosition + ". Error: " + ex.Message);
                        }
                    });
                }
            }
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
            ReservationCancelled?.Invoke(this, new ReservationEventArgs(null, true, false));
        }

        protected override void OnViewLoaded()
        {
#if DEBUG
            log.Debug("EditOrderViewModel OnViewLoaded start");
#endif
            TourGridViewModel.TourTypes = TourTypes;
            TourGridViewModel.Hotels = Hotels;
            TourGridViewModel.Optionals = Optionals;
            TourGridViewModel.Reservation = Reservation;

            CustomerGridViewModel.Reservation = Reservation;

            GeneralReservationViewModel.Reservation = Reservation;
            GeneralReservationViewModel.Agencies = Agencies;
#if DEBUG
            log.Debug("EditOrderViewModel OnViewLoaded end");
#endif
        }

        public void SetReservation(ReservationWrapper reservation)
        {
            Reservation = reservation == null ? new ReservationWrapper() : reservation;
            CleanAll();
        }
    }
}
