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
            Customers = new ObservableCollection<Customer>();
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
            var Result = _messageDialogService.ShowOkCancelDialog((string)Application.Current.FindResource("ShortAreYouSureMessage"), "Question");
            if (Result == MessageDialogResult.CANCEL)
                return;
            try
            {
                WithClient(_serviceFactory.CreateClient<IOrderService>(), reservationClient =>
                {
                    reservationClient.CancelReservation(obj.ReservationId);
                    Reservations.Remove(obj);
                    ContinualReservations.Remove(obj);
                    Client.NotifyServer(new EventDataType()
                    {
                        ClientName = Operator.OperatorName + "-" + Operator.OperatorId,
                        EventMessage = obj.Tours[0].StartDate.ToString()
                    });
                });
            }
            catch(Exception ex)
            {
                log.Error("OnDeleteReservationCommand Failed: " + ex.Message);
            }
        }

        private void OnGenerateReportCommand(Group group)
        {
            throw new NotImplementedException();
        }

        private void OnAddCommand(object arg)
        {
            CurrentReservationViewModel = new EditOrderViewModel(_serviceFactory, _messageDialogService, null);
            CurrentReservationViewModel.Operator = Operator;
            CurrentReservationViewModel.Hotels = Hotels;
            CurrentReservationViewModel.TourTypes = TourTypes;
            CurrentReservationViewModel.Optionals = Optionals;
            CurrentReservationViewModel.Agencies = Agencies;
            RegisterEvents();
            
        }

        private void OnEditReservationCommand(ReservationWrapper reservation)
        {
            Reservation dbReservation = null;
            try
            {
                WithClient(_serviceFactory.CreateClient<IOrderService>(), reservationClient =>
                {
                    dbReservation = reservationClient.GetReservation(reservation.ReservationId);
                });
            }
            catch (Exception ex)
            {
                _messageDialogService.ShowInfoDialog("Could not load reservation,\nmaybe it was deleted in the meantime by another user.", "Info");
                return;
            }
            var reservationWrapper = EchoDesertTrips.Client.Entities.ReservationHelper.CreateReservationWrapper(dbReservation);
            CurrentReservationViewModel = new EditOrderViewModel(_serviceFactory, _messageDialogService, reservationWrapper);
            CurrentReservationViewModel.Operator = Operator;
            CurrentReservationViewModel.Hotels = Hotels;
            CurrentReservationViewModel.TourTypes = TourTypes;
            CurrentReservationViewModel.Optionals = Optionals;
            CurrentReservationViewModel.Agencies = Agencies;
            RegisterEvents();
        }

        public override string ViewTitle
        {
            get { return "Reservations"; }
        }

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

        public async void LoadReservationsForDayRangeAsync(DateTime Date)
        {
            var orderClient = _serviceFactory.CreateClient<IOrderService>();
            {
                var uiContext = SynchronizationContext.Current;
                var task = Task.Factory.StartNew(() => orderClient.GetReservationsForDayRangeAsynchronous(Date.AddMonths(-1), Date.AddMonths(1)));
                var reservations = await task;
                await reservations.ContinueWith(e =>
                {
                    if (e.IsCompleted)
                    {
                        if (reservations != null)
                        {
                            uiContext.Send((x) =>
                            {
                                log.Debug("LoadReservationsForDayRange: reservations count = " + reservations.Result.Count());
                                Reservations.Clear();
                                ContinualReservations.Clear();
                                foreach (var reservation in reservations.Result)
                                {
                                    Reservations.Add(EchoDesertTrips.Client.Entities.ReservationHelper.CreateReservationWrapper(reservation));
                                }
                                Reservations.ToList().ForEach((reservation) =>
                                {
                                    ContinualReservations.Add(EchoDesertTrips.Client.Entities.ReservationHelper.CloneReservationWrapper(reservation));
                                });
                            }, null);
                        }
                    }
                });
            }

            IDisposable disposableClient = orderClient as IDisposable;
            if (disposableClient != null)
                disposableClient.Dispose();
        }

        public void LoadReservationsForDayRange(DateTime Date)
        {
            try
            {
                WithClient<IOrderService>(_serviceFactory.CreateClient<IOrderService>(), orderClient =>
                {
                    var reservations = orderClient.GetReservationsForDayRange(Date.AddMonths(-1), Date.AddMonths(1));
                    if (reservations != null)
                    {
                        log.Debug("LoadReservationsForDayRange: reservations count = " + reservations.Count());
                        Reservations.Clear();
                        ContinualReservations.Clear();
                        foreach (var reservation in reservations)
                        {
                            Reservations.Add(EchoDesertTrips.Client.Entities.ReservationHelper.CreateReservationWrapper(reservation));
                        }
                        Reservations.ToList().ForEach((reservation) =>
                        {
                            ContinualReservations.Add(EchoDesertTrips.Client.Entities.ReservationHelper.CloneReservationWrapper(reservation));
                        });
                    }
                });
            }
            catch (Exception ex)
            {
                log.Error("LoadReservationsForDayRange: Failed to load reservations for day range: " + ex.Message);
            }
        }

        public static DateTime LastSelectedDate;

        protected override void OnViewLoaded()
        {
            log.Debug("OnViewLoaded: Calling LoadReservationsForDayRange");
            LoadReservationsForDayRange(_selectedDate);
            //try
            //{
            //    WithClient<IInventoryService>(_serviceFactory.CreateClient<IInventoryService>(), inventoryClient =>
            //    {
            //        var inventoryData = inventoryClient.GetInventoryData();
            //        TourTypes.Clear();
            //        Hotels.Clear();
            //        Optionals.Clear();
            //        Agencies.Clear();
            //        foreach (var tourType in inventoryData.TourTypes)
            //        {
            //            if (tourType.Visible)
            //                TourTypes.Add(TourTypeHelper.CreateTourTypeWrapper(tourType));
            //        }
            //        foreach (var hotel in inventoryData.Hotels)
            //        {
            //            if (hotel.Visible)
            //                Hotels.Add(hotel);
            //        }
            //        foreach (var optional in inventoryData.Optionals)
            //        {
            //            if (optional.Visible)
            //                Optionals.Add(optional);
            //        }

            //        foreach(var agency in inventoryData.Agencies)
            //        {
            //            Agencies.Add(agency);
            //        }
            //    });
            //}
            //catch (Exception ex)
            //{
            //    log.Error("Exception load inventory data: " + ex.Message);
            //}
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
            ReservationsView.Filter = null;
            ReservationsView.Filter += x =>
                ((ReservationWrapper)x).Tours[0].StartDate == filterDate;

            ContinualReservationsView.Filter = null;
            ContinualReservationsView.Filter = null;
            ContinualReservationsView.Filter += x =>
            ((ReservationWrapper)x).Tours.ToList().Exists(tour => filterDate > tour.StartDate &&
            filterDate <= tour.EndDate);
        }

        //private void CurrentOrderViewModel_CustomerUpdated(object sender, ReservationEventArgs e)
        //{
        //    var reservation = Reservations.FirstOrDefault(item => item.ReservationId == e.Reservation.ReservationId);
        //    if (reservation != null)
        //    {
        //        foreach (var customer in e.Reservation.Customers)
        //            reservation.Customers.Add(customer);
        //    }
        //}

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
                else
                {
                    _messageDialogService.ShowInfoDialog((string)Application.Current.FindResource("ReservationDeletedMessage"), "Info");
                }
            }
            else
            {
                Reservations.Add(e.Reservation);
                ContinualReservations.Add(e.Reservation);
            }

            ReservationsView.Refresh();
            ContinualReservationsView.Refresh();
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
    }

    public class GroupsToTotalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (!(value is ReadOnlyObservableCollection<Object>)) return "";
            var items = (ReadOnlyObservableCollection<Object>)value;
            var total = items.Cast<ReservationWrapper>().Sum(otItem => otItem.Customers.Count);
            return total.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }

    public class TotalPriceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var reservation = (ReservationWrapper)value;
            return Support.ReservationUtils.calculateReservationTotalPrice(reservation);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class CustomersGroupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var items = ((System.Windows.Data.CollectionViewGroup) (value))?.Items;
            var customers = new ObservableCollection<CustomerWrapper>();
            foreach (var reservation in items.Cast<ReservationWrapper>())
            {
                foreach (var customer in reservation.Customers)
                {
                    customers.Add(customer);
                }
            }
            return customers;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class BalanceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var reservation = (ReservationWrapper)value;
            double totalPrice = Support.ReservationUtils.calculateReservationTotalPrice(reservation);
            return totalPrice - reservation.AdvancePayment;
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
            return string.Format("{0}, {1}, Jordan Reservation ID: {2}", reservation.Tours[0].TourType.TourTypeName, reservation.Tours[0].Private ? "Private" : "Regular", reservation.GroupID);
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
            return string.Format("{0}", reservation.Tours[0].TourType.TourTypeName);
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
