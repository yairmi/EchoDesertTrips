using Core.Common.Contracts;
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
using System.Configuration;
using static Core.Common.Core.Const;
using Core.Common.UI.PubSubEvent;
using Core.Common.UI.CustomEventArgs;

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
            EditReservationCommand = new DelegateCommand<ReservationDTO>(OnEditReservationCommand);
            EditContinualReservationCommand = new DelegateCommand<ReservationDTO>(OnEditContinualReservationCommand);
            AddReservationCommand = new DelegateCommand<object>(OnAddReservationCommand);
            SelectedDateChangedCommand = new DelegateCommand<object>(OnSelectedDateChangedCommand);
            DeleteReservationCommand = new DelegateCommand<ReservationDTO>(OnDeleteReservationCommand);
            DecreaseOneDayCommand = new DelegateCommand<object>(OnDecreaseOneDayCommand);
            IncreaseOneDayCommand = new DelegateCommand<object>(OnIncreaseOneDayCommand);
            ShowCustomersCommand = new DelegateCommand<object>(ShowCustomers, ShowCustomersCanExecute);
            _reservations = new RangeObservableCollection<ReservationDTO>();
            _continualReservations = new RangeObservableCollection<ReservationDTO>();
            _selectedDate = DateTime.Today;
            _lastSelectedDate = _selectedDate.AddDays(_daysRange + 1);
            //_eventAggregator.GetEvent<HotelUpdatedEvent>().Subscribe(HotelUpdated);
            _eventAggregator.GetEvent<ReservationUpdatedEvent>().Subscribe(ReservationUpdated);
            _eventAggregator.GetEvent<ReservationUpdatedAndNotifyClientsEvent>().Subscribe(ReservationUpdatedAndNotify);
            _eventAggregator.GetEvent<CustomerGroupClosedEvent>().Subscribe(CustomerGroupClosed);
            //_eventAggregator.GetEvent<OptionalUpdatedEvent>().Subscribe(OptionalUpdated);
        }

        /*private void OptionalUpdated(OptionalEventArgs e)
        {
            int exceptionPosition = 0;
            try
            {
                bool bChanged = false;
                exceptionPosition = 1;
                foreach (var reservation in _reservations)
                {
                    foreach (var tour in reservation.Tours)
                    {
                        exceptionPosition = 2;
                        var tourOptional = tour.TourOptionals.FirstOrDefault(o => o.OptionalId == e.Optional.OptionalId);
                        if (tourOptional != null &&
                            (tourOptional.Optional.PricePerPerson != e.Optional.PricePerPerson || tourOptional.Optional.PriceInclusive != e.Optional.PriceInclusive))
                        {
                            exceptionPosition = 3;
                            tourOptional.Optional.PricePerPerson = e.Optional.PricePerPerson;
                            tourOptional.Optional.PriceInclusive = e.Optional.PriceInclusive;
                            Support.ReservationHelper.CalculateReservationTotalPrice(reservation);
                            bChanged = true;
                            exceptionPosition = 4;
                        }
                    }
                }
                if (bChanged)
                {
                    exceptionPosition = 5;
                    OnPropertyChanged(() => ReservationsView);
                    exceptionPosition = 6;
                }
            }
            catch(Exception ex)
            {
                log.Error("OptionalUpdated failed. Position: " + exceptionPosition + ". error: " + ex.Message);
            }
        }*/

        private void ReservationUpdatedAndNotify(ReservationEventArgs e)
        {
            var reservationDTO = ConvertToReservationDTO(e.Reservation);
            try
            {
                if (!e.IsNew)
                {
                    //This is done in order to update the Grid. Remember that in EditTripViewModel the updated trip
                    //Is a temporary object and it is not part of the Grid collection trips.
                    var reservation = _reservations.FirstOrDefault(item => item.ReservationId == e.Reservation.ReservationId);
                    if (reservation != null)
                    {
                        var index = _reservations.IndexOf(_reservations.FirstOrDefault( r => r.ReservationId == reservationDTO.ReservationId));
                        _reservations[index] = reservationDTO;
                        _continualReservations[index] = reservationDTO;
                    }
                }
                else
                {
                    if (IsDayInRange(reservationDTO.Tours[0].StartDate))
                    {
                        _reservations.Add(reservationDTO);
                        _continualReservations.Add(reservationDTO);
                    }
                    else
                        SelectedDate = reservationDTO.Tours[0].StartDate;

                }
                if (!e.IsDbWon)
                {
                    Client.NotifyServer(SerializeReservationMessage(e.Reservation.ReservationId, eOperation.E_UPDATED),
                        eMsgTypes.E_RESERVATION);
                }
            }
            catch (Exception ex)
            {
                log.Error("Failed in CurrentReservationViewModel_ReservationUpdated", ex);
            }

            _eventAggregator.GetEvent<ReservationUpdatedFinishedEvent>().Publish(null);
        }

        private void ReservationUpdated(ReservationDTO reservation)
        {
            if (!IsDayInRange(reservation.Tours[0].StartDate))
                return;
            var existingReservation = _reservations.FirstOrDefault(item => item.ReservationId == reservation.ReservationId);
            if (existingReservation != null)
            {
                var index = _reservations.IndexOf(existingReservation);
                _reservations[index] = reservation;
                _continualReservations[index] = reservation;
            }
            else
            {
                _reservations.Add(reservation);
                _continualReservations.Add(reservation);
            }
        }
 
        //private void HotelUpdated(HotelEventArgs e)
        //{
        //    try
        //    {
        //        foreach (var reservation in _reservations)
        //        {
        //            foreach (var tour in reservation.Tours)
        //            {
        //                foreach (var tourHotel in tour.TourHotels)
        //                {
        //                    if (tourHotel.Hotel.HotelId == e.Hotel.HotelId)
        //                        tourHotel.Hotel = e.Hotel;
        //                }
        //            }
        //        }
        //    }
        //    catch(Exception ex)
        //    {
        //        log.Error("Exception in HotelUpdated: " + ex.Message);
        //    }
        //}

        //private void OnLostFocusCommand(Reservation reservation)
        //{
        //    log.Debug("OnLostFocusCommand start");
        //}

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

        public DelegateCommand<ReservationDTO> DeleteReservationCommand { get; set; }
        public DelegateCommand<ReservationDTO> EditReservationCommand { get; private set; }
        public DelegateCommand<ReservationDTO> EditContinualReservationCommand { get; private set; }
        public DelegateCommand<object> AddReservationCommand { get; private set; }
        public DelegateCommand<object> SelectedDateChangedCommand { get; private set; }
        public DelegateCommand<Group> GenerateReportCommand { get; private set; }
        public DelegateCommand<object> DecreaseOneDayCommand { get; }
        public DelegateCommand<object> IncreaseOneDayCommand { get; }
        public DelegateCommand<object> ShowCustomersCommand { get; private set; }

        public async void ShowCustomers(object obj)
        {
            var reservation1 = obj as ReservationDTO;
            if (reservation1 != null)
            {
                IsEnabled = false;
                CustomersGroupViewModel = new CustomersGroupViewModel();
                var reservationClient = _serviceFactory.CreateClient<IOrderService>();
                {
                try
                {

                    int groupID = reservation1.GroupID;
                    var uiContext = SynchronizationContext.Current;

                    var customers = await Task.Factory.StartNew(() =>
                    {
                        CustomersGroupViewModel.LoadingVisible = true;
                        return reservationClient.GetCustomersByReservationGroupIdAsynchronous(groupID);
                    });
                    await customers.ContinueWith((Task<Customer[]> e) =>
                    {
                        if (e.IsCompleted)
                        {
                            uiContext.Send((x) =>
                            {
                                CustomersGroupViewModel.CustomersForGroup.AddRange(customers.Result);
                                CustomersGroupViewModel.LoadingVisible = false;
                            }, null);
                        }
                    });
                }
                catch(Exception ex)
                {
                    log.Error("Failed to show customers", ex);
                }
                }
                var disposableClient = reservationClient as IDisposable;
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

        private void OnDeleteReservationCommand(ReservationDTO obj)
        {
            var result = _messageDialogService.ShowOkCancelDialog((string)System.Windows.Application.Current.FindResource("ShortAreYouSureMessage"), "Question");
            if (result == MessageDialogResult.CANCEL)
                return;
            Reservation reservation = null;
            try
            {
                WithClient(_serviceFactory.CreateClient<IOrderService>(), reservationClient =>
                {
                    reservation = reservationClient.DeleteReservation(obj.ReservationId);
                });

                if (reservation != null && reservation.Lock == true)
                {
                    Operator Operator = null;
                    try
                    {
                        WithClient(_serviceFactory.CreateClient<IOperatorService>(), operatorClient =>
                        {
                            Operator = operatorClient.GetOperatorByID(reservation.LockedById);
                        });
                    }
                    catch(Exception ex)
                    {
                        log.Error(string.Empty, ex);
                    }
                    var operatorName = Operator != null ? Operator.OperatorName : string.Empty;
                    var message = $"Cannot delete reservation. The record is held by {operatorName}.";
                    _messageDialogService.ShowInfoDialog(message, "Info");
                }
                else
                {
                    _reservations.Remove(obj);
                    _continualReservations.Remove(obj);
                    try
                    {
                        Client.NotifyServer(
                            SerializeReservationMessage(obj.ReservationId, eOperation.E_DELETED), eMsgTypes.E_RESERVATION);
                    }
                    catch (Exception ex)
                    {
                        log.Error("Failed to notify server after a reservation was deleted", ex);
                    }
                }
            }
            catch(Exception ex)
            {
                log.Error(string.Empty, ex);
            }
        }

        private void OnAddReservationCommand(object arg)
        {
            log.Debug("Start");
            _eventAggregator.GetEvent<ReservationEditSelectedEvent>().Publish(new EditReservationEventArgs(null, false, false));
            log.Debug("End");
        }

        private bool _bIsContinual = false;
        private void OnEditContinualReservationCommand(ReservationDTO reservation)
        {
            _bIsContinual = true;
            OnEditReservationCommand(reservation);
            _bIsContinual = false;
        }

        private void OnEditReservationCommand(ReservationDTO reservation)
        {
            log.Debug($"Start. ReservationId={reservation.ReservationId}");
            Reservation dbReservation = null;
            bool bViewMode = false;

            try
            {
                WithClient(_serviceFactory.CreateClient<IOrderService>(), reservationClient =>
                {
                    dbReservation = reservationClient.EditReservation(reservation.ReservationId, CurrentOperator.Operator);
                });
                if (dbReservation == null)
                {
                    _messageDialogService.ShowInfoDialog("Could not load reservation,\nmaybe it was deleted in the meantime by another user.", "Info");
                    return;
                }
                
                if (dbReservation.Lock)
                {
                    Operator Operator = null;
                    try
                    {
                        WithClient(_serviceFactory.CreateClient<IOperatorService>(), operatorClient =>
                        {
                            Operator = operatorClient.GetOperatorByID(dbReservation.LockedById);
                        });
                        var OperatorName = Operator == null ? string.Empty : Operator.OperatorName;
                        string message = $"Reservation is locked by {OperatorName}.\n\n   Press \"OK\" To view the Reservation or\n   Press \"Cancel\" to cancel Edit.";
                        var result = _messageDialogService.ShowOkCancelDialog(message, "Info");
                        if (result == MessageDialogResult.CANCEL)
                            return;

                        bViewMode = true;
                    }
                    catch(Exception)
                    {
                        throw;
                    }
                }
                else
                {
                    bViewMode = !Client.Registered;
                }

                _eventAggregator.GetEvent<ReservationEditSelectedEvent>().Publish(new EditReservationEventArgs(dbReservation, bViewMode, _bIsContinual));
                log.Debug("End");
            }
            catch(Exception ex)
            {
                log.Error(string.Empty, ex);
            }
        }

        public override string ViewTitle => "Reservations";

        private RangeObservableCollection<ReservationDTO> _reservations;
        private RangeObservableCollection<ReservationDTO> _continualReservations;

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

        private bool IsDayInRange(DateTime date)
        {
            return (date > _selectedDate.AddDays(_daysRange * (-1)) && date < _selectedDate.AddDays(_daysRange));
        }

        public async void LoadReservationsForDayRangeAsync2()
        {
            if (!IsInDayRange(_selectedDate))     
            {
                log.Debug("LoadReservationsForDayRangeAsync2 Start");
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
                        await reservations.ContinueWith((Task<ReservationDTO[]> e) =>
                        {
                            if (e.IsCompleted)
                            {
                                uiContext.Send((x) =>
                                {
                                    try
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
                                            log.Error("LoadReservationsForDayRangeAsync2 returns null");
                                        }
                                    }
                                    catch(Exception ex)
                                    {
                                        log.Error("Failed to display reservations", ex);
                                    }
                                }, null);
                            }
                        });
                    }
                    catch(Exception ex)
                    {
                        log.Error("Failed to load reservations", ex);
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
            log.Debug("Start");
            
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
            try
            {
                ReservationsView.Filter = null;
                ReservationsView.Filter += x =>
                    ((ReservationDTO)x).Tours[0].StartDate == _selectedDate;
                ContinualReservationsView.Filter = null;
                ContinualReservationsView.Filter += x =>
                ((ReservationDTO)x).Tours.ToList().Exists(tour => _selectedDate > tour.StartDate &&
                _selectedDate <= tour.EndDate);
            }
            catch (Exception ex)
            {
                log.Error("Failed to filter dates", ex);
            }
        }

        public void UpdateReservations(ReservationDTO reservation)
        {
            if (!IsDayInRange(reservation.Tours[0].StartDate))
                return;
            var existingReservation = _reservations.FirstOrDefault(item => item.ReservationId == reservation.ReservationId);
            if (existingReservation != null)
            {
                var index = _reservations.IndexOf(existingReservation);
                _reservations[index] = reservation;
                _continualReservations[index] = reservation;
            }
            else
            {
                _reservations.Add(reservation);
                _continualReservations.Add(reservation);
            }
        }

        public void RemoveReservationFromGUI(int reservationId)
        {
            var reservation = _reservations.FirstOrDefault(item => item.ReservationId == reservationId);
            if (reservation != null)
            {
                _reservations.Remove(reservation);
            }
        }

        private void OnSelectedDateChangedCommand(object obj)
        {
            throw new NotImplementedException();
        }

        private void CustomerGroupClosed(EventArgs obj)
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

        private ReservationDTO ConvertToReservationDTO(Reservation reservation)
        {
            var customersExist = reservation.Customers != null && reservation.Customers.Count() > 0;
            var toursExist = reservation.Tours != null && reservation.Tours.Count() > 0;
            return new ReservationDTO()
            {
                ReservationId = reservation.ReservationId,
                FirstName = customersExist ? reservation.Customers[0].FirstName : string.Empty,
                LastName = customersExist ? reservation.Customers[0].LastName : string.Empty,
                Phone1 = customersExist  ? reservation.Customers[0].Phone1 : string.Empty,
                HotelName = toursExist ? (reservation.Tours[0].TourHotels[0] == null ? string.Empty : reservation.Tours[0].TourHotels[0].Hotel.HotelName) : string.Empty,
                AgencyName = reservation.Agency == null ? string.Empty : reservation.Agency.AgencyName,
                AdvancePayment = reservation.AdvancePayment,
                TotalPrice = reservation.TotalPrice,
                PickUpTime = reservation.PickUpTime,
                Comments = reservation.Comments,
                Messages = reservation.Messages,
                Group = reservation.Group,
                GroupID = reservation.GroupID,
                ActualNumberOfCustomers = reservation.ActualNumberOfCustomers,
                FirstTourTypeName = toursExist ? reservation.Tours[0].TourType.TourTypeName : string.Empty,
                Private = toursExist ? reservation.Tours[0].TourType.Private : false,
                Tours = reservation.Tours.ToList()
            };
        }
    }

    public class GroupsToTotalConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is ReadOnlyObservableCollection<Object>)) return "";
            var items = (ReadOnlyObservableCollection<Object>)value;
            var reservations = items.Cast<ReservationDTO>().ToList();
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
            var reservation = (ReservationDTO)value;
            return $"{reservation.TotalPrice}/{reservation.TotalPrice - reservation.AdvancePayment}";
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
            //var reservation = (Reservation)value;
            //if (reservation != null)
            //{
            //    return reservation.TotalPrice - reservation.AdvancePayment;
            //}
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
            var reservation = ((ReservationDTO)(value));
            if (reservation != null)
            {
                string reservationType = reservation.Private ? "Private" : "Regular";
                return $"{reservation.FirstTourTypeName}, {reservationType}, Jordan Reservation ID: {reservation.GroupID}";
            }
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
            var reservation = ((ReservationDTO)(value));
            return reservation != null ? reservation.FirstTourTypeName : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
