﻿using Core.Common.Contracts;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Data;
using System.Globalization;
using System.Threading.Tasks;
using System.Threading;
using EchoDesertTrips.Desktop.CustomEventArgs;
using System.Configuration;

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
            DecreaseOneDayCommand = new DelegateCommand<object>(OnDecreaseOneDayCommand);
            IncreaseOneDayCommand = new DelegateCommand<object>(OnIncreaseOneDayCommand);
            ShowCustomersCommand = new DelegateCommand<object>(ShowCustomers, ShowCustomersCanExecute);
            RowEditEndingCommand = new DelegateCommand<Reservation>(OnRowEditEndingCommand);
            Beginning​EditCommand = new DelegateCommand<Reservation>(OnBeginning​EditCommand);
            _reservations = new RangeObservableCollection<Reservation>();
            _continualReservations = new RangeObservableCollection<Reservation>();
            _selectedDate = DateTime.Today;
            _lastSelectedDate = _selectedDate.AddDays(_daysRange + 1);
        }

        private void OnBeginning​EditCommand(Reservation obj)
        {
            throw new NotImplementedException();
        }

        private void OnRowEditEndingCommand(Reservation obj)
        {
            throw new NotImplementedException();
        }

        public ReservationsViewModel()
        {
        }

        private CustomersGroupViewModel _customerGroupViewModel;

        public CustomersGroupViewModel CustomersGroupViewModel
        {
            get { return _customerGroupViewModel; }
            set
            {
                if (_customerGroupViewModel != value)
                {
                    _customerGroupViewModel = value;
                    OnPropertyChanged(() => CustomersGroupViewModel, false);
                }
            }
        }

        public DelegateCommand<Reservation> DeleteReservationCommand { get; set; }

        public DelegateCommand<Reservation> EditReservationCommand { get; private set; }
        public DelegateCommand<object> AddReservationCommand { get; private set; }
        public DelegateCommand<object> SelectedDateChangedCommand { get; private set; }
        public DelegateCommand<Group> GenerateReportCommand { get; private set; }

        public DelegateCommand<object> DecreaseOneDayCommand { get; }
        public DelegateCommand<object> IncreaseOneDayCommand { get; }
        
        public DelegateCommand<object> ShowCustomersCommand { get; private set; }

        public DelegateCommand<Reservation> RowEditEndingCommand { get; set; }
        public DelegateCommand<Reservation> Beginning​EditCommand { get; set; }

        public async void ShowCustomers(object obj)
        {
            var reservation1 = obj as Reservation;
            if (reservation1 != null)
            {
                IsEnabled = false;
                CustomersGroupViewModel = new CustomersGroupViewModel();
                var orderClient = _serviceFactory.CreateClient<IOrderService>();
                {
                try
                {

                    int groupID = reservation1.GroupID;
                    var uiContext = SynchronizationContext.Current;

                    var reservations = await Task.Factory.StartNew(() =>
                    {
                        CustomersGroupViewModel.LoadingVisible = true;
                        return orderClient.GetCustomersByReservationGroupIdAsynchronous(groupID);
                    });
                    await reservations.ContinueWith((Task<Reservation[]> e) =>
                    {
                        if (e.IsCompleted)
                        {
                            uiContext.Send((x) =>
                            {
                                reservations.Result.ToList().ForEach((reservation) =>
                                {
                                    CustomersGroupViewModel.CustomersForGroup.AddRange(reservation.Customers);
                                });

                                RegisterEvents();
                                CustomersGroupViewModel.LoadingVisible = false;
                            }, null);
                        }
                    });
                }
                catch(Exception ex)
                {
                    log.Error(string.Format("Exception in ShowCustomers. exception: {0}", ex.Message));
                }
                }
                var disposableClient = orderClient as IDisposable;
                disposableClient?.Dispose();
            }
        }

        private bool ShowCustomersCanExecute(object obj)
        {
            return true;
        }

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
            var result = _messageDialogService.ShowOkCancelDialog((string)System.Windows.Application.Current.FindResource("ShortAreYouSureMessage"), "Question");
            if (result == MessageDialogResult.CANCEL)
                return;
            Reservation reservation = null;
            WithClient(_serviceFactory.CreateClient<IOrderService>(), reservationClient =>
            {
                reservation = reservationClient.DeleteReservation(obj.ReservationId);
            }, "DeleteReservation");
            if (reservation != null && reservation.Lock == true)
            {
                Operator Operator = null;
                WithClient(_serviceFactory.CreateClient<IOperatorService>(), operatorClient =>
                {
                    Operator = operatorClient.GetOperatorByID(reservation.LockedById);
                }, "GetOperatorByID");
                var operatorName = Operator != null ? Operator.OperatorName : string.Empty;
                var message = string.Format("Cannot delete reservation. The record is held by {0}.", operatorName);
                _messageDialogService.ShowInfoDialog(message, "Info");
             }
             else
             {
                _reservations.Remove(obj);
                _continualReservations.Remove(obj);
                Client.NotifyServer(new EventDataType()
                {
                    ClientName = Operator.OperatorName + "-" + Operator.OperatorId,
                    EventMessage = "1;" + obj.Tours[0].StartDate.ToString()
                });
             }
        }

        private void OnAddReservationCommand(object arg)
        {
            log.Debug("ReservationViewModel:OnAddCommand start");
            ReservationEdited?.Invoke(this, new EditReservationEventArgs(null, false));
            log.Debug("ReservationViewModel:OnAddCommand end");
        }

        private void OnEditReservationCommand(Reservation reservation)
        {
            log.Debug("ReservationViewModel:OnEditReservationCommand start");
            Reservation dbReservation = null;
            WithClient(_serviceFactory.CreateClient<IOrderService>(), reservationClient =>
            {
                dbReservation = reservationClient.EditReservation(reservation.ReservationId, Operator);
            });
            if (dbReservation == null)
            {
                _messageDialogService.ShowInfoDialog("Could not load reservation,\nmaybe it was deleted in the meantime by another user.", "Info");
                return;
            }
            bool bViewMode = false;
            if (dbReservation.Lock)
            {
                Operator Operator = null;
                WithClient(_serviceFactory.CreateClient<IOperatorService>(), operatorClient =>
                {
                    Operator = operatorClient.GetOperatorByID(dbReservation.LockedById);
                });
                var OperatorName = Operator == null ? string.Empty : Operator.OperatorName;
                string message = string.Format("Reservation is locked by {0}.\n\n   Press \"OK\" To view the Reservation or\n   Press \"Cancel\" to cancel Edit.", OperatorName);
                var result = _messageDialogService.ShowOkCancelDialog(message, "Info");
                if (result == MessageDialogResult.CANCEL)
                    return;
                bViewMode = true;
            }
            ReservationEdited?.Invoke(this, new EditReservationEventArgs(dbReservation, bViewMode));
            log.Debug("ReservationViewModel:OnEditReservationCommand end");
        }

        public override string ViewTitle => "Reservations";

        //private ObservableCollection<Reservation> _reservations;
        private RangeObservableCollection<Reservation> _reservations;
        private RangeObservableCollection<Reservation> _continualReservations;

        public ICollectionView ReservationsView { get; set; }
        public ICollectionView ContinualReservationsView { get; set; }

        private bool _isEnabled;

        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }
            set
            {
                _isEnabled = value;
                OnPropertyChanged(() => IsEnabled);
            }
        }

        private DateTime _selectedDate;

        public DateTime SelectedDate
        {
            get
            {
                return _selectedDate;
            }
            set
            {
                _selectedDate = value;
                FilterByDate();
                LoadReservationsForDayRangeAsync2();
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
            if (IsInDayRange(date))
            {
                log.Debug("LoadReservationsForDayRangeAsync execution started");
                var orderClient = _serviceFactory.CreateClient<IOrderService>();
                {
                    try
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
                    catch (Exception ex)
                    {
                        log.Error(string.Format("Exception in LoadReservationsForDayRangeAsync. exception: {0}", ex.Message));
                    }
                }
                var disposableClient = orderClient as IDisposable;
                disposableClient?.Dispose();
            }
        }

        public async void LoadReservationsForDayRangeAsync2()
        {
            log.Debug("LoadReservationsForDayRangeAsync2 start");

            if (!IsInDayRange(_selectedDate))     
            {
                IsEnabled = false;
                LoadingVisible = true;
                _lastSelectedDate = _selectedDate;
                var orderClient = _serviceFactory.CreateClient<IOrderService>();
                {
                    try
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
                                    int exceptionPosition = 0;
                                    try
                                    {
                                        if (reservations.Result != null)
                                        {
                                            _reservations.Clear();
                                            _reservations.AddRange(reservations.Result);
                                            exceptionPosition = 1;
                                            _continualReservations.Clear();
                                            _continualReservations.AddRange(_reservations);
                                            exceptionPosition = 2;
                                        }
                                        else
                                        {
                                            log.Error("LoadReservationsForDayRangeAsync2 returns null");
                                        }
                                    }
                                    catch(Exception ex)
                                    {
                                        log.Error(string.Format("LoadReservationsForDayRangeAsync2 exception. ExceptionPosition = {0}. Exception = {1}", exceptionPosition, ex.Message));
                                    }
                                }, null);
                            }
                        });
                    }
                    catch(Exception ex)
                    {
                        log.Error(string.Format("Exception in LoadReservationsForDayRangeAsync2. exception: {0}", ex.Message));
                    }
                }
                log.Debug("LoadReservationsForDayRangeAsync2 execution End");
                var disposableClient = orderClient as IDisposable;
                disposableClient?.Dispose();
                IsEnabled = true;
                LoadingVisible = false;
            }
        }

        private bool IsInDayRange(DateTime day)
        {
            return (day >= _lastSelectedDate.AddDays(_daysRange * (-1)) &&
                    day <= _lastSelectedDate.AddDays(_daysRange));
        }

        private DateTime _lastSelectedDate;

        protected override void OnViewLoaded()
        {
            log.Debug("ReservationViewModel OnViewLoaded Start");
            
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
            FilterByDate();
            LoadReservationsForDayRangeAsync2();
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
                    SelectedDate = e.Reservation.Tours.OrderBy(t => t.StartDate).FirstOrDefault().StartDate; ;
                    //if the first day in the new reservation is in current range then load was not called in SelectedDate 'Set' so we must add it to _reservations
                    if (IsDayInRange(_selectedDate))
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

        private void RegisterEvents()
        {
            CustomersGroupViewModel.Close -= CustomersGroupViewModel_Closed;
            CustomersGroupViewModel.Close += CustomersGroupViewModel_Closed;
        }

        private void CustomersGroupViewModel_Closed(object sender, EventArgs e)
        {
            CustomersGroupViewModel = null;
            IsEnabled = true;
        }

        private bool _loadingVisible;

        public bool LoadingVisible
        {
            get
            {
                return _loadingVisible;
            }
            set
            {
                if (_loadingVisible != value)
                {
                    _loadingVisible = value;
                    OnPropertyChanged(() => LoadingVisible);
                }
            }
        }

        private Reservation editedReservation;
    }

    public class GroupsToTotalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ReadOnlyObservableCollection<Object>)) return "";
            var items = (ReadOnlyObservableCollection<Object>)value;
            var reservations = items.Cast<Reservation>().ToList();
            int totalCustomers = 0;
            reservations.ForEach((reservation) =>
            {
                totalCustomers += reservation.ActualNumberOfCustomers;
            });
            return totalCustomers.ToString();
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
            //var items = ((CollectionViewGroup)(value))?.Items;
            //var customers = new ObservableCollection<Customer>();
            //if (items == null) return customers;
            //foreach (var reservation in items.Cast<Reservation>())
            //{
            //    foreach (var customer in reservation.Customers)
            //    {
            //        customers.Add(customer);
            //    }
            //}

            //return customers;
            return null;
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
            if (reservation != null)
            {
                return reservation.TotalPrice - reservation.AdvancePayment;
            }
            return 0;
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
                    reservation.Tours[0].TourType.Private ? "Private" : "Regular", reservation.GroupID);
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
