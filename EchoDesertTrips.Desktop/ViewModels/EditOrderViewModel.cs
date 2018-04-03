using Core.Common.Contracts;
using Core.Common.UI.Core;
using Core.Common.Utils;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using EchoDesertTrips.Desktop.Support;
using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;


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
            TourGridViewModel = new TourGridViewModel(_serviceFactory, _messageDialogService, Reservation.Tours);
            AgencyViewModel = new AgencyViewModel(_serviceFactory, Reservation);
        }

        public DelegateCommand<object> AddCustomerCommad { get; set; }
        public DelegateCommand<object> AddTripCommand { get; set; }

        public DelegateCommand<object> SaveCommand { get; private set; }
        public DelegateCommand<object> ExitWithoutSavingCommand { get; private set; }
        public DelegateCommand<bool> CheckBoxAgreeChecked { get; private set; }

        private void OnCheckBoxAgreeChecked(bool checkBoxCheked)
        {
            AgencyViewModel.IsEnabled = checkBoxCheked;
        }

        private bool OnAddTripCommadCanExecute(object obj)
        {
            return true;
        }

        private bool OnAddCustomerCommadCanExecute(object obj)
        {
            return true;
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
            if (Reservation.Tours.Count > 0)
            {
                Reservation.Operator = Operator;
                Reservation.OperatorId = Operator.OperatorId;
                if (Reservation.Tours[0].TourType.IncramentExternalId)
                {
                    var reservationForHashCode = String.Format("{0} {1} {2}",
                        Reservation.Tours[0].StartDate.Date.ToString("d"),
                        Reservation.Tours[0].TourType.TourTypeName,
                        Reservation.Tours[0].TourType.Private);
                    if (Reservation.Group == null)
                        Reservation.Group = new Group();
                    Reservation.Group.ExternalId = HashCodeUtil.GetHashCodeBernstein(reservationForHashCode);
                }

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
                                var reservationWrapper = new ReservationWrapper();
                                reservationWrapper = ReservationHelper.CreateReservationWrapper(reservationData.DbReservation);
                                ReservationUpdated?.Invoke(this, new ReservationEventArgs(reservationWrapper, true));
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
                        if (reservationData.DbReservation == null)
                        {
                            log.Info("Reservation was deleted by someone else. ReservationID = " + reservation.ReservationId);
                            _messageDialogService.ShowInfoDialog((string)Application.Current.FindResource("ReservationDeletedMessage"), "Info");
                            return;
                        }
                        var reservationWrapper = new ReservationWrapper();
                        if (reservationData.InEdit == true)
                        {
                            log.Info("Reservation was edited by someone else. ReservationID = " + reservation.ReservationId);
                            var message = String.Format("{0} {1} {2}",
                                "The Reservation has been changed In the meantime by",
                                reservationData.DbReservation.Operator.OperatorName,
                                ".\nClick OK to save your changes anyway,\nClick Cancel to reload the entity from the database.");
                            var result = _messageDialogService.ShowOkCancelDialog(message, "Question");

                            if (reservationData.InEdit == true)
                            {
                                //Client win
                                if (result == MessageDialogResult.OK)
                                {
                                    log.Info("Client Win. ReservationID = " + reservation.ReservationId);
                                    reservation.RowVersion = reservationData.DbReservation.RowVersion;
                                    reservationClient.UpdateReservation(reservation);
                                    reservationWrapper = ReservationHelper.CreateReservationWrapper(reservation);//rw.CopyReservation(reservation);
                                    ReservationUpdated?.Invoke(this, new ReservationEventArgs(reservationWrapper, false));
                                }
                                //Data base win
                                else
                                {
                                    log.Info("DB Win. ReservationID = " + reservation.ReservationId);
                                    reservationWrapper = ReservationHelper.CreateReservationWrapper(reservationData.DbReservation); //rw.CopyReservation(reservationData.DbReservation);
                                    ReservationUpdated?.Invoke(this, new ReservationEventArgs(reservationWrapper, false));
                                }
                            }
                        }
                        else
                        {
                            reservationWrapper = ReservationHelper.CreateReservationWrapper(reservationData.DbReservation); //rw.CopyReservation(reservationData.DbReservation);
                            ReservationUpdated?.Invoke(this, new ReservationEventArgs(reservationWrapper, false));
                        }
                    });
                }
            }
            else
            {
                string message = (string)Application.Current.FindResource("ZeroToursMessage");//"Could not save reservation. The reservation must contain\nat least one tour.";
                _messageDialogService.ShowInfoDialog(message, "Info");
            }
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
                var Result = _messageDialogService.ShowOkCancelDialog((string)Application.Current.FindResource("AreYouSureMessage"), "Question");
                if (Result == MessageDialogResult.CANCEL)
                    return;
            }
            ReservationCancelled?.Invoke(this, new ReservationEventArgs(/*reservation*/null, true));
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

    public class ExpanderToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (value == parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (System.Convert.ToBoolean(value)) return parameter;
            return null;
        }
    }
}
