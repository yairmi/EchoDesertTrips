using Core.Common.Contracts;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using EchoDesertTrips.Desktop.Support;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using System.Threading.Tasks;
using System.Threading;
using EchoDesertTrips.Desktop.Reports;
using EchoDesertTrips.Desktop.CustomEventArgs;
using System.Windows.Controls;
using System.Configuration;
using Core.Common.Extensions;
using System.Collections.Generic;

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReservationsViewModel : ViewModelBase
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IMessageDialogService _messageDialogService;
        private readonly int _daysRange;
        private const int _defaultDayReange = 7;

        [ImportingConstructor]
        public ReservationsViewModel(IServiceFactory serviceFactory, 
            IMessageDialogService messageBoxDialogService)
        {
            _serviceFactory = serviceFactory;
            _messageDialogService = messageBoxDialogService;
            bool bResult = Int32.TryParse(ConfigurationManager.AppSettings["DaysRange"], out _daysRange);
            if (bResult == false)
                _daysRange = _defaultDayReange;
            EditReservationCommand = new DelegateCommand<Reservation>(OnEditReservationCommand);
            AddReservationCommand = new DelegateCommand<object>(OnAddReservationCommand);
            SelectedDateChangedCommand = new DelegateCommand<object>(OnSelectedDateChangedCommand);
            DeleteReservationCommand = new DelegateCommand<Reservation>(OnDeleteReservationCommand);
            GenerateReportCommand = new DelegateCommand<Group>(OnGenerateReportCommand);
            DecreaseOneDayCommand = new DelegateCommand<object>(OnDecreaseOneDayCommand);
            IncreaseOneDayCommand = new DelegateCommand<object>(OnIncreaseOneDayCommand);
            _reservations = new RangeObservableCollection<Reservation>();
            _continualReservations = new RangeObservableCollection<Reservation>();
            _selectedDate = DateTime.Today;
            _lastSelectedDate = _selectedDate.AddDays(_daysRange + 1);
        }

        public ReservationsViewModel()
        {
        }

        public DelegateCommand<Reservation> DeleteReservationCommand { get; set; }

        public DelegateCommand<Reservation> EditReservationCommand { get; private set; }
        public DelegateCommand<object> AddReservationCommand { get; private set; }
        public DelegateCommand<object> SelectedDateChangedCommand { get; private set; }
        public DelegateCommand<Group> GenerateReportCommand { get; private set; }

        public DelegateCommand<object> DecreaseOneDayCommand { get; }
        public DelegateCommand<object> IncreaseOneDayCommand { get; }

        private void OnIncreaseOneDayCommand(object obj)
        {
            SelectedDate = SelectedDate.AddDays(1);
        }

        private void OnDecreaseOneDayCommand(object obj)
        {
            SelectedDate = SelectedDate.AddDays(-1);
        }

        private void OnDeleteReservationCommand(Reservation obj)
        {
            var result = _messageDialogService.ShowOkCancelDialog((string)Application.Current.FindResource("ShortAreYouSureMessage"), "Question");
            if (result == MessageDialogResult.CANCEL)
                return;
            int exceptionPosition = 0;
            WithClient(_serviceFactory.CreateClient<IOrderService>(), reservationClient =>
            {
                try
                {
                    reservationClient.CancelReservation(obj.ReservationId);
                    exceptionPosition = 1;
                    _reservations.Remove(obj);
                    exceptionPosition = 2;
                    _continualReservations.Remove(obj);
                    exceptionPosition = 3;
                    Client.NotifyServer(new EventDataType()
                    {
                        ClientName = Operator.OperatorName + "-" + Operator.OperatorId,
                        EventMessage = "1;" + obj.Tours[0].StartDate.ToString()
                    });
                }
                catch(Exception ex)
                {
                    log.Error("OnDeleteReservationCommand dailed. position: " + exceptionPosition + ". Error: " + ex.Message);
                }
            });
        }

        private void OnGenerateReportCommand(Group group)
        {
            var reportGen = new ReportGenerator();
            int exceptionPosition = 0;
            WithClient(_serviceFactory.CreateClient<IOrderService>(), orderClient =>
            {
                try
                {
                    var reservations = orderClient.GetReservationsByGroupId(group.GroupId);
                    exceptionPosition = 1;
                    reportGen.GenerateReport(reservations, group.GroupId);
                }
                catch(Exception ex)
                {
                    log.Error("OnGenerateReportCommand dailed. position: " + exceptionPosition + ". Error: " + ex.Message);
                }
            });
        }

        private void OnAddReservationCommand(object arg)
        {
            log.Debug("ReservationViewModel:OnAddCommand start");
            ReservationEdited?.Invoke(this, new EditReservationEventArgs(null));
            log.Debug("ReservationViewModel:OnAddCommand end");
        }

        private void OnEditReservationCommand(Reservation reservation)
        {
            log.Debug("ReservationViewModel:OnEditReservationCommand start");
            Reservation dbReservation = null;
            WithClient(_serviceFactory.CreateClient<IOrderService>(), reservationClient =>
            {
                dbReservation = reservationClient.GetReservation(reservation.ReservationId);
            });
            if (dbReservation == null)
            {
                _messageDialogService.ShowInfoDialog("Could not load reservation,\nmaybe it was deleted in the meantime by another user.", "Info");
                return;
            }
            //var reservationWrapper = EchoDesertTrips.Client.Entities.ReservationMapper.CreateReservationWrapper(dbReservation);
            ReservationEdited?.Invoke(this, new EditReservationEventArgs(dbReservation));
            log.Debug("ReservationViewModel:OnEditReservationCommand end");
        }

        public override string ViewTitle => "Reservations";

        //private ObservableCollection<Reservation> _reservations;
        private RangeObservableCollection<Reservation> _reservations;
        private RangeObservableCollection<Reservation> _continualReservations;

        public ICollectionView ReservationsView { get; set; }
        public ICollectionView ContinualReservationsView { get; set; }

        private DateTime _selectedDate;

        public DateTime SelectedDate
        {
            get
            {
                FilterByDate();
                return _selectedDate;
            }
            set
            {
                _selectedDate = value;
                log.Debug("SelectedDate : " + _selectedDate);
                LoadReservationsForDayRange();
                OnPropertyChanged(() => SelectedDate, false);
            }
        }

        public bool IsDayInRange(DateTime date)
        {
            return (date > _selectedDate.AddDays(_daysRange * (-1)) && date < _selectedDate.AddDays(_daysRange));
        }

        public async void LoadReservationsForDayRangeAsync(DateTime date)
        {
            log.Debug("LoadReservationsForDayRangeAsync start");
            if (date >= _selectedDate.AddDays(_daysRange * (-1)) && date <= _selectedDate.AddDays(_daysRange))
            {
                log.Debug("LoadReservationsForDayRangeAsync execution started");
                var orderClient = _serviceFactory.CreateClient<IOrderService>();
                {
                    var uiContext = SynchronizationContext.Current;
                    var task = Task.Factory.StartNew(() => orderClient.GetReservationsForDayRangeAsynchronous(_selectedDate.AddDays(_daysRange * (-1)), _selectedDate.AddDays(_daysRange)));
                    var reservations = await task;
                    await reservations.ContinueWith((Task<Reservation[]> e) =>
                    {
                        if (e.IsCompleted)
                        {
                            uiContext.Send((x) =>
                            {
                                if (reservations.Result != null)
                                {
                                    _reservations.Clear();
                                    _reservations.AddRange(reservations.Result);
                                    _continualReservations.Clear();
                                    _continualReservations.AddRange(_reservations);
                                }
                                else
                                {
                                    log.Error("LoadReservationsForDayRangeAsync returns null");
                                }
                            }, null);
                        }
                    });
                }
                var disposableClient = orderClient as IDisposable;
                disposableClient?.Dispose();
            }
        }

        public void LoadReservationsForDayRange()
        {
            log.Debug("LoadReservationsForDayRange start");
            if (_selectedDate < _lastSelectedDate.AddDays(_daysRange * (-1)) ||
                _selectedDate > _lastSelectedDate.AddDays(_daysRange))
            {
                _lastSelectedDate = _selectedDate;
                int exceptionPosition = 0;

                WithClient(_serviceFactory.CreateClient<IOrderService>(), orderClient =>
                {
                    try
                    {
                        exceptionPosition = 1;
                        var reservations = orderClient.GetReservationsForDayRange(_selectedDate.AddDays(_daysRange * (-1)), _selectedDate.AddDays(_daysRange));
                        exceptionPosition = 2;
                        if (reservations!= null)
                        {
                            _reservations.Clear();
                            _reservations.AddRange(reservations);
                            exceptionPosition = 3;
                            _continualReservations.Clear();
                            _continualReservations.AddRange(_reservations);
                            exceptionPosition = 4;
                        }
                        else
                        {
                            log.Error("LoadReservationsForDayRange returns null");
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error("LoadReservationsForDayRange failed. position: " + exceptionPosition + ". Error: " + ex.Message);
                    }
                });
            }
            log.Debug("LoadReservationsForDayRange end");
        }

        private DateTime _lastSelectedDate;

        protected override void OnViewLoaded()
        {
            log.Debug("ReservationViewModel OnViewLoaded Start");
            LoadReservationsForDayRange();
            if (ReservationsView == null)
            {
                ReservationsView = CollectionViewSource.GetDefaultView(_reservations);
                ReservationsView.GroupDescriptions.Add(new PropertyGroupDescription(".", new GroupReservationsConverter()));
            }
            if (ContinualReservationsView == null)
            {
                ContinualReservationsView = CollectionViewSource.GetDefaultView(_continualReservations);
                ContinualReservationsView.GroupDescriptions.Add(new PropertyGroupDescription(".",new  GroupContinualReservationsConverter()));
            }
        }

        private void FilterByDate()
        {
            int exceptionPosition = 0;
            try
            {
                ReservationsView.Filter = null;
                ReservationsView.Filter += x =>
                    ((Reservation)x).Tours[0].StartDate == _selectedDate;
                exceptionPosition = 1;
                ContinualReservationsView.Filter = null;
                exceptionPosition = 2;
                ContinualReservationsView.Filter += x =>
                ((Reservation)x).Tours.ToList().Exists(tour => _selectedDate > tour.StartDate &&
                _selectedDate <= tour.EndDate);
            }
            catch (Exception ex)
            {
                log.Error("Exception in filter by day. Position: " + exceptionPosition + ". Error" + ex.Message);
            }
        }

        public void UpdateReservations(ReservationEventArgs e)
        {
            int exceptionPosition = 0;
            try
            {
                if (!e.IsNew)
                {
                    exceptionPosition = 1;
                    //This is done in order to update the Grid. Remember that in EditTripViewModel the updated trip
                    //Is a temporary object and it is not part of the Grid collection trips.
                    var reservation = _reservations.FirstOrDefault(item => item.ReservationId == e.Reservation.ReservationId);
                    exceptionPosition = 2;
                    if (reservation != null)
                    {
                        var index = _reservations.IndexOf(reservation);
                        _reservations[index] = e.Reservation;
                        exceptionPosition = 3;
                        _continualReservations[index] = e.Reservation;
                        exceptionPosition = 4;
                    }
                }
                else
                {
                    SelectedDate = e.Reservation.Tours.OrderBy(t => t.StartDate).FirstOrDefault().StartDate;
                    if (_reservations.FirstOrDefault(r => r.ReservationId == e.Reservation.ReservationId) == null)
                    {
                        exceptionPosition = 5;
                        _reservations.Add(e.Reservation);
                        exceptionPosition = 6;
                        _continualReservations.Add(e.Reservation);
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error("CurrentReservationViewModel_ReservationUpdated failed. Position: " + exceptionPosition + ". error: " + ex.Message);
            }
            if (!e.IsDbWon)
            {
                try
                {
                    Client.NotifyServer(new EventDataType()
                    {
                        ClientName = Operator.OperatorName + "-" + Operator.OperatorId,
                        EventMessage = "1;" + e.Reservation.Tours[0].StartDate.ToString()
                    });
                }
                catch (Exception ex)
                {
                    log.Error("CurrentReservationViewModel_ReservationUpdated: Failed to notify server: " + ex.Message);
                }
            }
        }

        public event EventHandler<EditReservationEventArgs> ReservationEdited;

        private void OnSelectedDateChangedCommand(object obj)
        {
            throw new NotImplementedException();
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

    public class GroupsToTotalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ReadOnlyObservableCollection<Object>)) return "";
            var items = (ReadOnlyObservableCollection<Object>)value;
            var total = items.Cast<Reservation>().Sum(otItem => otItem.Customers.Count);
            return total.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            CultureInfo culture)
        {
            return value;
        }
    }

    public class TotalPriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var reservation = (Reservation)value;
            return Support.ReservationHelper.CalculateReservationTotalPrice(reservation);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    
    public class CustomersGroupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var items = ((CollectionViewGroup) (value))?.Items;
            var customers = new ObservableCollection<Customer>();
            if (items == null) return customers;
            foreach (var reservation in items.Cast<Reservation>())
            {
                foreach (var customer in reservation.Customers)
                {
                    customers.Add(customer);
                }
            }

            return customers;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BalanceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var reservation = (Reservation)value;
            var totalPrice = Support.ReservationHelper.CalculateReservationTotalPrice(reservation);
            if (reservation != null)
            {
                return totalPrice - reservation.AdvancePayment;
            }
            return totalPrice;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class GroupReservationsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var reservation = ((Reservation)(value));
            if (reservation != null)
                return string.Format("{0}, {1}, Jordan Reservation ID: {2}", reservation.Tours[0].TourType.TourTypeName,
                    reservation.Tours[0].Private ? "Private" : "Regular", reservation.GroupID);
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    public class GroupContinualReservationsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var reservation = ((Reservation)(value));
            return reservation != null ? string.Format("{0}", reservation.Tours[0].TourType.TourTypeName) : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
