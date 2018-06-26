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

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReservationsViewModel : ViewModelBase
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IMessageDialogService _messageDialogService;

        [ImportingConstructor]
        public ReservationsViewModel(IServiceFactory serviceFactory, 
            IMessageDialogService messageBoxDialogService)
        {
            _serviceFactory = serviceFactory;
            _messageDialogService = messageBoxDialogService;
            EditReservationCommand = new DelegateCommand<ReservationWrapper>(OnEditReservationCommand);
            AddReservationCommand = new DelegateCommand<object>(OnAddReservationCommand);
            SelectedDateChangedCommand = new DelegateCommand<object>(OnSelectedDateChangedCommand);
            DeleteReservationCommand = new DelegateCommand<ReservationWrapper>(OnDeleteReservationCommand);
            GenerateReportCommand = new DelegateCommand<Group>(OnGenerateReportCommand);

            Reservations = new ObservableCollection<ReservationWrapper>();
            ContinualReservations = new ObservableCollection<ReservationWrapper>();
            SelectedDate = LastSelectedDate = DateTime.Now;
        }

        public ReservationsViewModel()
        {
        }

        public DelegateCommand<ReservationWrapper> DeleteReservationCommand { get; set; }

        public DelegateCommand<ReservationWrapper> EditReservationCommand { get; private set; }
        public DelegateCommand<object> AddReservationCommand { get; private set; }
        public DelegateCommand<object> SelectedDateChangedCommand { get; private set; }
        public DelegateCommand<Group> GenerateReportCommand { get; private set; }

        private void OnDeleteReservationCommand(ReservationWrapper obj)
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
                    Reservations.Remove(obj);
                    exceptionPosition = 2;
                    ContinualReservations.Remove(obj);
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
#if DEBUG
            log.Debug("ReservationViewModel:OnAddCommand start");
#endif
            ReservationEdited?.Invoke(this, new EditReservationEventArgs(null));
#if DEBUG
            log.Debug("ReservationViewModel:OnAddCommand end");
#endif
        }

        private void OnEditReservationCommand(ReservationWrapper reservation)
        {
#if DEBUG
            log.Debug("ReservationViewModel:OnEditReservationCommand start");
#endif
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
            var reservationWrapper = ReservationHelper.CreateReservationWrapper(dbReservation);
            ReservationEdited?.Invoke(this, new EditReservationEventArgs(reservationWrapper));
#if DEBUG
            log.Debug("ReservationViewModel:OnEditReservationCommand end");
#endif
        }

        public override string ViewTitle => "Reservations";

        private ObservableCollection<ReservationWrapper> _reservations;

        public ObservableCollection<ReservationWrapper> Reservations
        {
            get
            {
                return _reservations;
            }
            set
            {
                if (value != _reservations)
                {
                    _reservations = value;
                    OnPropertyChanged(() => Reservations, false);
                }
            }
        }

        private ObservableCollection<ReservationWrapper> _continualReservations;

        public ObservableCollection<ReservationWrapper> ContinualReservations
        {
            get
            {
                return _continualReservations;
            }
            set
            {
                if (value != _continualReservations)
                {
                    _continualReservations = value;
                    OnPropertyChanged(() => ContinualReservations, false);
                }
            }
        }

        public ICollectionView ReservationsView { get; set; }
        public ICollectionView ContinualReservationsView { get; set; }

        private ObservableCollection<Customer> _customers;

        public ObservableCollection<Customer> Customers
        {
            get { return _customers; }
            set
            {
                _customers = value;
                OnPropertyChanged(()=>Customers);
            }
        }

        private DateTime _selectedDate;

        public DateTime SelectedDate
        {
            get
            {
                FilterByDate(_selectedDate);
                return _selectedDate;
            }
            set
            {
                _selectedDate = value;
#if DEBUG
                log.Debug("SelectedDate : " + _selectedDate);
#endif
                if (_selectedDate < LastSelectedDate.AddMonths(-1) ||
                    _selectedDate > LastSelectedDate.AddMonths(1))
                {
                    log.Debug("SelectedDate: Calling LoadReservationsForDayRange");
                    LoadReservationsForDayRange(_selectedDate);
                }
                LastSelectedDate = _selectedDate;
                OnPropertyChanged(()=>SelectedDate, false);
            }
        }

        public bool IsDayInRange(DateTime date)
        {
            return (date > _selectedDate.AddMonths(-1) && date < _selectedDate.AddMonths(1));
        }

        public async void LoadReservationsForDayRangeAsync(DateTime date)
        {
#if DEBUG
            log.Debug("LoadReservationsForDayRangeAsync start");
#endif
            var orderClient = _serviceFactory.CreateClient<IOrderService>();
            {
                var uiContext = SynchronizationContext.Current;
                var task = Task.Factory.StartNew(() => orderClient.GetReservationsForDayRangeAsynchronous(date.AddMonths(-1), date.AddMonths(1)));
                var reservations = await task;
                await reservations.ContinueWith(e =>
                {
                    if (e.IsCompleted)
                    {
                        uiContext.Send((x) =>
                        {
                            log.Debug("LoadReservationsForDayRange: reservations count = " + reservations.Result.Length);
                            Reservations.Clear();
                            ContinualReservations.Clear();
                            foreach (var reservation in reservations.Result)
                            {
                                Reservations.Add(ReservationHelper.CreateReservationWrapper(reservation));
                            }
                            Reservations.ToList().ForEach((reservation) =>
                            {
                                ContinualReservations.Add(ReservationHelper.CloneReservationWrapper(reservation));
                            });
                        }, null);
                    }
                });
            }

            var disposableClient = orderClient as IDisposable;
            disposableClient?.Dispose();
        }

        public void LoadReservationsForDayRange(DateTime date)
        {
#if DEBUG
            log.Debug("LoadReservationsForDayRange start");
#endif
            int exceptionPosition = 0;
            WithClient(_serviceFactory.CreateClient<IOrderService>(), orderClient =>
            {
                try
                {
                    var reservations = orderClient.GetReservationsForDayRange(date.AddMonths(-1), date.AddMonths(1));
                    exceptionPosition = 1;
                    if (reservations != null)
                    {
                        log.Debug("LoadReservationsForDayRange: reservations count = " + reservations.Length);
                        Reservations.Clear();
                        exceptionPosition = 2;
                        ContinualReservations.Clear();
                        exceptionPosition = 3;
                        foreach (var reservation in reservations)
                        {
                            Reservations.Add(ReservationHelper.CreateReservationWrapper(reservation));
                        }
                        exceptionPosition = 4;
                        Reservations.ToList().ForEach((reservation) =>
                        {
                            ContinualReservations.Add(ReservationHelper.CloneReservationWrapper(reservation));
                        });
                    }
                }
                catch(Exception ex)
                {
                    log.Error("LoadReservationsForDayRange failed. position: " + exceptionPosition + ". Error: " + ex.Message);
                }
            });
        }

        public static DateTime LastSelectedDate;

        protected override void OnViewLoaded()
        {
            log.Debug("ReservationViewModel OnViewLoaded Start");
            Customers = new ObservableCollection<Customer>();
            LoadReservationsForDayRange(_selectedDate);
            SelectedDate = DateTime.Today;
            if (ReservationsView == null)
            {
                ReservationsView = CollectionViewSource.GetDefaultView(Reservations);
                ReservationsView.GroupDescriptions.Add(new PropertyGroupDescription(".", new GroupReservationsConverter()));
                ReservationsView.SortDescriptions.Add(new SortDescription("Days", ListSortDirection.Ascending));
            }
            if (ContinualReservationsView == null)
            {
                ContinualReservationsView = CollectionViewSource.GetDefaultView(ContinualReservations);
                ContinualReservationsView.GroupDescriptions.Add(new PropertyGroupDescription(".",new  GroupContinualReservationsConverter()));
                ContinualReservationsView.SortDescriptions.Add(new SortDescription("Days", ListSortDirection.Ascending));
            }
        }

        private void FilterByDate(DateTime filterDate)
        {
            int exceptionPosition = 0;
            try
            {
                ReservationsView.Filter = null;
                ReservationsView.Filter += x =>
                    ((ReservationWrapper)x).Tours[0].StartDate == filterDate;
                exceptionPosition = 1;
                ContinualReservationsView.Filter = null;
                ContinualReservationsView.Filter = null;
                exceptionPosition = 2;
                ContinualReservationsView.Filter += x =>
                ((ReservationWrapper)x).Tours.ToList().Exists(tour => filterDate > tour.StartDate &&
                filterDate <= tour.EndDate);
            }
            catch(Exception ex)
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
                    var reservation = Reservations.FirstOrDefault(item => item.ReservationId == e.Reservation.ReservationId);
                    exceptionPosition = 2;
                    if (reservation != null)
                    {
                        var index = Reservations.IndexOf(reservation);
                        Reservations[index] = e.Reservation;
                        exceptionPosition = 3;
                        ContinualReservations[index] = e.Reservation;
                        exceptionPosition = 4;
                    }
                }
                else
                {
                    exceptionPosition = 5;
                    Reservations.Add(e.Reservation);
                    exceptionPosition = 6;
                    ContinualReservations.Add(e.Reservation);
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
            var total = items.Cast<ReservationWrapper>().Sum(otItem => otItem.Customers.Count);
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
            var reservation = (ReservationWrapper)value;
            return ReservationUtils.CalculateReservationTotalPrice(reservation);
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
            var customers = new ObservableCollection<CustomerWrapper>();
            if (items == null) return customers;
            foreach (var reservation in items.Cast<ReservationWrapper>())
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
            var reservation = (ReservationWrapper)value;
            var totalPrice = ReservationUtils.CalculateReservationTotalPrice(reservation);
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
            var reservation = ((ReservationWrapper)(value));
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
            var reservation = ((ReservationWrapper)(value));
            return reservation != null ? string.Format("{0}", reservation.Tours[0].TourType.TourTypeName) : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
