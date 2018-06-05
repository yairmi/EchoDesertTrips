using Core.Common.Contracts;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using EchoDesertTrips.Desktop.Support;
using System;
using System.Collections.Generic;
using System.Windows;
using Core.Common.Core;


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
            _serviceFactory = serviceFactory;
            _messageDialogService = messageDialogService;
            SetReservation(reservstion);
            SaveCommand = new DelegateCommand<object>(OnSaveCommand, OnSaveCommandCanExecute);
            ExitWithoutSavingCommand = new DelegateCommand<object>(OnExitWithoutSavingCommand);
            CheckBoxAgreeChecked = new DelegateCommand<bool>(OnCheckBoxAgreeChecked);
            CustomerGridViewModel = new CustomerGridViewModel(_serviceFactory, _messageDialogService, Reservation);
            TourGridViewModel = new TourGridViewModel(_serviceFactory, _messageDialogService, Reservation);
            AgencyViewModel = new AgencyViewModel(_serviceFactory, Reservation);
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

        private bool IsReservationDirty()
        {
            return Reservation.IsAnythingDirty();
        }

        public event EventHandler<ReservationEventArgs> ReservationUpdated;

        private void OnSaveCommand(object obj)
        {
            ValidateModel();
            if (Reservation.IsValid)
            //if (Reservation.Tours.Count > 0)
            {
                Reservation.Operator = Operator;
                Reservation.OperatorId = Operator.OperatorId;
                ReservationUtils.CreateExternalId(Reservation);
                ReservationUtils.RemoveUnselectedHotels(Reservation);
                ReservationUtils.RemoveUnselectedOptionals(Reservation);

                if (Reservation.ReservationId == 0) //New Reservation
                {
                    WithClient<IOrderService>(_serviceFactory.CreateClient<IOrderService>(), reservationClient =>
                    {
                        var reservation = ReservationHelper.CreateReservation(Reservation);
                        try
                        {
                            var reservationData = reservationClient.UpdateReservation(reservation); //Update or Add

                            if (reservationData.DbReservation != null)
                            {
                                var reservationWrapper = ReservationHelper.CreateReservationWrapper(reservationData.DbReservation);
                                ReservationUpdated?.Invoke(this, new ReservationEventArgs(reservationWrapper, true, false));
                            }
                        }
                        catch (Exception ex)
                        {
                            log.Error("Exception in save new reservation: " + ex.Message);
                        }
                    });
                }
                else if (IsReservationDirty())
                {
                    WithClient(_serviceFactory.CreateClient<IOrderService>(), reservationClient =>
                    {
                        var reservation = ReservationHelper.CreateReservation(Reservation);

                        var reservationData = reservationClient.UpdateReservation(reservation); //Add or Update but in this case its Update
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
                                        reservationData = reservationClient.UpdateReservation(reservation);
                                        if (!reservationData.InEdit)
                                        {
                                            reservationWrapper = ReservationHelper.CreateReservationWrapper(reservation);
                                            ReservationUpdated?.Invoke(this,
                                                new ReservationEventArgs(reservationWrapper, false, false));
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
                                            var newReservationWrapper =
                                                ReservationHelper.CreateReservationWrapper(newReservation
                                                    .DbReservation);
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
                                        ReservationCancelled?.Invoke(this, new ReservationEventArgs(null, true, false));

                                    inEdit = false;
                                }
                            }
                        }
                        else
                        {
                            reservationWrapper = ReservationHelper.CreateReservationWrapper(reservationData.DbReservation); //rw.CopyReservation(reservationData.DbReservation);
                            ReservationUpdated?.Invoke(this, new ReservationEventArgs(reservationWrapper, false, false));
                        }
                    });
                }
            }
            //else
            //{
            //    var message = (string)Application.Current.FindResource("ZeroToursMessage");//"Could not save reservation. The reservation must contain\nat least one tour.";
            //    _messageDialogService.ShowInfoDialog(message, "Info");
            //}
        }

        protected override void AddModels(List<ObjectBase> models)
        {
            models.Add(Reservation);
        }

        protected override void OnViewLoaded()
        {
            TourGridViewModel.TourTypes = TourTypes;
            TourGridViewModel.Hotels = Hotels;
            TourGridViewModel.Optionals = Optionals;

            AgencyViewModel.Agencies = Agencies;
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
