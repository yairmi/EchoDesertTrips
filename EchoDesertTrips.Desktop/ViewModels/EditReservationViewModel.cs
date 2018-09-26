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


        private bool _lastDertinessValue = false;

        private bool IsReservationDirty()
        {
            var bDirty = Reservation.IsAnythingDirty() && Reservation.Tours.Count > 0;
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
                var startDay1 = new DateTime(Reservation.Tours[0].StartDate.Year, Reservation.Tours[0].StartDate.Month, Reservation.Tours[0].StartDate.Day);
                var endDay1 = new DateTime(Reservation.Tours[0].EndDate.Year, Reservation.Tours[0].EndDate.Month, Reservation.Tours[0].EndDate.Day);
                DateTime startDay2 = DateTime.Now;
                DateTime endDay2 = DateTime.Now;
                if (Reservation.Tours.Count > 1)
                {
                    startDay2 = new DateTime(Reservation.Tours[1].StartDate.Year, Reservation.Tours[1].StartDate.Month, Reservation.Tours[1].StartDate.Day);
                    endDay2 = new DateTime(Reservation.Tours[1].EndDate.Year, Reservation.Tours[1].EndDate.Month, Reservation.Tours[1].EndDate.Day);
                }
                for (int j = 0; j < 10; j++)
                {
                    Reservation.Tours[0].StartDate = startDay1.AddDays(j);
                    Reservation.Tours[0].EndDate = endDay1.AddDays(j);
                    if (Reservation.Tours.Count > 1)
                    {
                        Reservation.Tours[1].StartDate = startDay2.AddDays(j);
                        Reservation.Tours[1].EndDate = endDay2.AddDays(j);
                    }
                    for (int i = 0; i < 50; i++)
                    {
                        int exceptionPosition = 0;

                        if (Reservation.ReservationId == 0) //New Reservation
                        {
                            WithClient<IOrderService>(_serviceFactory.CreateClient<IOrderService>(), reservationClient =>
                            {
                                try
                                {
                                    exceptionPosition = 1;
                            //var reservation = ReservationMapper.CreateReservation(Reservation);
                            exceptionPosition = 2;
                                    var reservationData = reservationClient.UpdateReservation(Reservation); //Update or Add
                            exceptionPosition = 3;
                                    if (reservationData.DbReservation != null)
                                    {
                                        exceptionPosition = 4;
                                //var reservationWrapper = ReservationMapper.CreateReservationWrapper(reservationData.DbReservation);
                                exceptionPosition = 5;
                                        ReservationUpdated?.Invoke(this, new ReservationEventArgs(reservationData.DbReservation, true, false));
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
                            //var reservation = ReservationMapper.CreateReservation(Reservation);
                            exceptionPosition = 2;
                                    var reservationData = reservationClient.UpdateReservation(Reservation); //Add or Update but in this case its Update
                            exceptionPosition = 3;
                            //ReservationWrapper reservationWrapper;

                            if (reservationData.InEdit)
                                    {
                                        var inEdit = true;
                                        while (inEdit)
                                        {
                                            string message;
                                            if (reservationData.DbReservation == null)
                                            {
                                                log.Info("Reservation was deleted by someone else. ReservationID = " +
                                                         Reservation.ReservationId);

                                                message =
                                                    "The Reservation was deleted by someone else.\nClick OK to save your changes anyway,\nClick Cancel to discard your changes.";
                                            }
                                            else
                                            {
                                                log.Info("Reservation was edited by someone else. ReservationID = " +
                                                         Reservation.ReservationId);
                                                message =
                                                    $"The Reservation has been changed In the meantime by {reservationData.DbReservation.Operator.OperatorName} {".\nClick OK to save your changes anyway,\nClick Cancel to reload the entity from the database."}";
                                            }

                                            var result = _messageDialogService.ShowOkCancelDialog(message, "Question");
                                    //Client win
                                    if (result == MessageDialogResult.OK)
                                            {
                                                if (reservationData.DbReservation != null) //Update existing record
                                        {
                                                    Reservation.RowVersion = reservationData.DbReservation.RowVersion;
                                                    exceptionPosition = 4;
                                                    reservationData = reservationClient.UpdateReservation(Reservation);
                                                    if (!reservationData.InEdit)
                                                    {
                                                //reservationWrapper = ReservationMapper.CreateReservationWrapper(reservation);
                                                exceptionPosition = 5;
                                                        ReservationUpdated?.Invoke(this,
                                                            new ReservationEventArgs(Reservation, false, false));
                                                        exceptionPosition = 6;
                                                        inEdit = false;
                                                    }
                                                }
                                                else //Insert Deleted (By other user) record
                                        {
                                                    Reservation.ReservationId = 0;
                                                    Reservation.RowVersion = null;

                                                    var newReservation = reservationClient.UpdateReservation(Reservation);
                                                    if (newReservation.DbReservation != null)
                                                    {
                                                        exceptionPosition = 7;
                                                //var newReservationWrapper = 
                                                //    ReservationMapper.CreateReservationWrapper(newReservation
                                                //        .DbReservation);
                                                exceptionPosition = 8;
                                                        ReservationUpdated?.Invoke(this,
                                                            new ReservationEventArgs(newReservation.DbReservation, true, false));
                                                    }

                                                    inEdit = false;
                                                }

                                            }
                                    //Data base win
                                    else
                                            {
                                                if (reservationData.DbReservation != null)
                                                {
                                                    log.Info("DB Win. ReservationID = " + Reservation.ReservationId);
                                            //reservationWrapper = 
                                            //    ReservationMapper.CreateReservationWrapper(reservationData
                                            //        .DbReservation);
                                            ReservationUpdated?.Invoke(this,
                                                        new ReservationEventArgs(reservationData.DbReservation, false,
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
                                //reservationWrapper = ReservationMapper.CreateReservationWrapper(); //rw.CopyReservation(reservationData.DbReservation);
                                ReservationUpdated?.Invoke(this, new ReservationEventArgs(reservationData.DbReservation, false, false));
                                        exceptionPosition = 12;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    log.Error("OnSaveCommand failed update reservation. position: " + exceptionPosition + ". Error: " + ex.Message);
                                }
                            });
                        }
                    }//remove
                }//remove
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
            log.Debug("EditOrderViewModel OnViewLoaded start");
            TourGridViewModel.TourTypes = TourTypes;
            TourGridViewModel.Hotels = Hotels;
            TourGridViewModel.Optionals = Optionals;
            TourGridViewModel.Reservation = Reservation;

            CustomerGridViewModel.Reservation = Reservation;

            GeneralReservationViewModel.Reservation = Reservation;
            GeneralReservationViewModel.Agencies = Agencies;
            log.Debug("EditOrderViewModel OnViewLoaded end");
        }

        public void SetReservation(Reservation reservation)
        {
            Reservation = reservation == null ? new Reservation() : reservation;
            CleanAll();
        }
    }
}
