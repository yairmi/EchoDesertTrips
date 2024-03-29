﻿using Core.Common.Contracts;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using EchoDesertTrips.Desktop.Support;
using System;
using System.Collections.Generic;
using System.Windows;
using Core.Common.Core;
using EchoDesertTrips.Desktop.CustomEventArgs;


//Customer Grid

namespace EchoDesertTrips.Desktop.ViewModels
{
    public class EditOrderViewModel : ViewModelBase
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IMessageDialogService _messageDialogService;

        //[ImportingConstructor]
        public EditOrderViewModel(IServiceFactory serviceFactory,
            IMessageDialogService messageDialogService,
            ReservationWrapper reservstion)
        {
#if DEBUG
            log.Debug("EditOrderViewModel ctor start");
#endif
            _serviceFactory = serviceFactory;
            _messageDialogService = messageDialogService;
            SetReservation(reservstion);
            SaveCommand = new DelegateCommand<object>(OnSaveCommand, OnSaveCommandCanExecute);
            ExitWithoutSavingCommand = new DelegateCommand<object>(OnExitWithoutSavingCommand);
            CheckBoxAgreeChecked = new DelegateCommand<bool>(OnCheckBoxAgreeChecked);
            CustomerGridViewModel = new CustomerGridViewModel(_serviceFactory, _messageDialogService, Reservation);
            TourGridViewModel = new TourGridViewModel(_serviceFactory, _messageDialogService, Reservation);
            AgencyViewModel = new AgencyViewModel(_serviceFactory, Reservation);
#if DEBUG
            log.Debug("EditOrderViewModel ctor end");
#endif
        }

        public DelegateCommand<object> SaveCommand { get; }
        public DelegateCommand<object> ExitWithoutSavingCommand { get; }
        public DelegateCommand<bool> CheckBoxAgreeChecked { get; }

        private void OnCheckBoxAgreeChecked(bool checkBoxCheked)
        {
            AgencyViewModel.IsEnabled = checkBoxCheked;
        }

        //After pressing the 'Save' button
        private bool OnSaveCommandCanExecute(object obj)
        {
            return IsReservationDirty();
        }
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

        public event EventHandler<ReservationEventArgs> ReservationUpdated;

        private void OnSaveCommand(object obj)
        {
            ValidateModel();
            if (Reservation.IsValid)
            {
                Reservation.Operator = Operator;
                Reservation.OperatorId = Operator.OperatorId;
                ReservationUtils.CreateExternalId(Reservation);
                ReservationUtils.RemoveUnselectedHotels(Reservation);
                ReservationUtils.RemoveUnselectedOptionals(Reservation);
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
                            log.Error("OnSaveCommand failed adding new reservation. position: "+ exceptionPosition + ". Error: " + ex.Message);
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
                        catch(Exception ex)
                        {
                            log.Error("OnSaveCommand failed update reservation. position: " + exceptionPosition + ". Error: " + ex.Message);
                        }
                    });
                }
            }
        }

        protected override void AddModels(List<ObjectBase> models)
        {
            models.Add(Reservation);
        }

        protected override void OnViewLoaded()
        {
#if DEBUG
            log.Debug("EditOrderViewModel OnViewLoaded start");
#endif
            TourGridViewModel.TourTypes = TourTypes;
            TourGridViewModel.Hotels = Hotels;
            TourGridViewModel.Optionals = Optionals;
            AgencyViewModel.Agencies = Agencies;
#if DEBUG
            log.Debug("EditOrderViewModel OnViewLoaded end");
#endif
        }
        
        private void SetReservation(ReservationWrapper reservation)
        {
            if (reservation == null)
            {
                reservation = new ReservationWrapper();
            }
            Reservation = reservation;
            CleanAll();
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
            ReservationCancelled?.Invoke(this, new ReservationEventArgs(/*reservation*/null, true, false));
        }

        public TourGridViewModel TourGridViewModel { get; set; }
        public CustomerGridViewModel CustomerGridViewModel { get; set; }
        public AgencyViewModel AgencyViewModel { get; set; }

        private ReservationWrapper _reservation;

        public ReservationWrapper Reservation
        {
            get { return _reservation; }
            set
            {
                _reservation = value;
                OnPropertyChanged(() => Reservation, false);
            }
        }

        private Object _selectedeExpander;

        public Object SelectedExpander
        {
            get
            {
                return _selectedeExpander;
            }
            set
            {
                _selectedeExpander = value;
                OnPropertyChanged(() => SelectedExpander, false);
            }
        }
    }

    //public class ExpanderToBooleanConverter : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        return (value == parameter);
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (System.Convert.ToBoolean(value)) return parameter;
    //        return null;
    //    }
    //}
}
