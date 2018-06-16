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
            AddReservationCommand = new DelegateCommand<object>(OnAddCommand);
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

        public EditOrderViewModel EditOrderViewModel { get; set; }

        public DelegateCommand<ReservationWrapper> EditReservationCommand { get; private set; }
        public DelegateCommand<object> AddReservationCommand { get; private set; }
        public DelegateCommand<object> SelectedDateChangedCommand { get; private set; }
        public DelegateCommand<Group> GenerateReportCommand { get; private set; }

        private void OnDeleteReservationCommand(ReservationWrapper obj)
        {
            var result = _messageDialogService.ShowOkCancelDialog((string)Application.Current.FindResource("ShortAreYouSureMessage"), "Question");
            if (result == MessageDialogResult.CANCEL)
                return;
 
            WithClient(_serviceFactory.CreateClient<IOrderService>(), reservationClient =>
            {
                reservationClient.CancelReservation(obj.ReservationId);
                Reservations.Remove(obj);
                ContinualReservations.Remove(obj);
                Client.NotifyServer(new EventDataType()
                {
                    ClientName = Operator.OperatorName + "-" + Operator.OperatorId,
                    EventMessage = "1;" + obj.Tours[0].StartDate.ToString()
                });
            }, "OnDeleteReservationCommand");
        }

        private void OnGenerateReportCommand(Group group)
        {
            var reportGen = new ReportGenerator();
            WithClient(_serviceFactory.CreateClient<IOrderService>(), orderClient =>
            {
                var reservations = orderClient.GetReservationsByGroupId(group.GroupId);
                reportGen.GenerateReport(reservations, group.GroupId);
            },"OnGenerateReportCommand");
        }

        private void OnAddCommand(object arg)
        {
#if DEBUG
            log.Debug("ReservationViewModel:OnAddCommand start");
#endif
            CurrentReservationViewModel =
                new EditOrderViewModel(_serviceFactory, _messageDialogService, null)
                {
                    Operator = Operator,
                    Hotels = Hotels,
                    TourTypes = TourTypes,
                    Optionals = Optionals,
                    Agencies = Agencies
                };
            RegisterEvents();
#if DEBUG
            log.Debug("ReservationViewModel:OnAddCommand end");
#endif            
        }

        private void OnEditReservationCommand(ReservationWrapper reservation)
        {
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
            CurrentReservationViewModel =
                new EditOrderViewModel(_serviceFactory, _messageDialogService,
                    reservationWrapper)
                {
                    Operator = Operator,
                    Hotels = Hotels,
                    TourTypes = TourTypes,
                    Optionals = Optionals,
                    Agencies = Agencies
                };
            RegisterEvents();
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

        private EditOrderViewModel _editReservationViewModel;

        public EditOrderViewModel CurrentReservationViewModel
        {
            get { return _editReservationViewModel; }
            set
            {
                if (_editReservationViewModel != value)
                {
                    _editReservationViewModel = value;
                    OnPropertyChanged(() => CurrentReservationViewModel, false);
                }
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
            WithClient(_serviceFactory.CreateClient<IOrderService>(), orderClient =>
            {
                var reservations = orderClient.GetReservationsForDayRange(date.AddMonths(-1), date.AddMonths(1));
                if (reservations != null)
                {
                    log.Debug("LoadReservationsForDayRange: reservations count = " + reservations.Length);
                    Reservations.Clear();
                    ContinualReservations.Clear();
                    foreach (var reservation in reservations)
                    {
                        Reservations.Add(ReservationHelper.CreateReservationWrapper(reservation));
                    }
                    Reservations.ToList().ForEach((reservation) =>
                    {
                        ContinualReservations.Add(ReservationHelper.CloneReservationWrapper(reservation));
                    });
                }
            }, "LoadReservationsForDayRange");
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
#if DEBUG
            log.Debug("FilterByDate Start");
#endif
            ReservationsView.Filter = null;
            ReservationsView.Filter += x =>
                ((ReservationWrapper)x).Tours[0].StartDate == filterDate;

            ContinualReservationsView.Filter = null;
            ContinualReservationsView.Filter = null;
            ContinualReservationsView.Filter += x =>
            ((ReservationWrapper)x).Tours.ToList().Exists(tour => filterDate > tour.StartDate &&
            filterDate <= tour.EndDate);
#if DEBUG
            log.Debug("FilterByDate End");
#endif
        }

        private void CurrentReservationViewModel_ReservationUpdated(object sender, ReservationEventArgs e)
        {
            if (!e.IsNew)
            {
                //This is done in order to update the Grid. Remember that in EditTripViewModel the updated trip
                //Is a temporary object and it is not part of the Grid collection trips.
                var reservation = Reservations.FirstOrDefault(item => item.ReservationId == e.Reservation.ReservationId);
                if (reservation != null)
                {
                    var index = Reservations.IndexOf(reservation);
                    Reservations[index] = e.Reservation;
                    ContinualReservations[index] = e.Reservation;
                }
            }
            else
            {
                Reservations.Add(e.Reservation);
                ContinualReservations.Add(e.Reservation);
            }

            ReservationsView.Refresh();
            ContinualReservationsView.Refresh();
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

            CurrentReservationViewModel = null;
        }

        private void CurrentReservationViewModel_ReservationCancelled(object sender, ReservationEventArgs e)
        {
            CurrentReservationViewModel = null;
        }

        private void RegisterEvents()
        {
            CurrentReservationViewModel.ReservationUpdated -= CurrentReservationViewModel_ReservationUpdated;
            CurrentReservationViewModel.ReservationUpdated += CurrentReservationViewModel_ReservationUpdated;
            CurrentReservationViewModel.ReservationCancelled -= CurrentReservationViewModel_ReservationCancelled;
            CurrentReservationViewModel.ReservationCancelled += CurrentReservationViewModel_ReservationCancelled;
        }

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
