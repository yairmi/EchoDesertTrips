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
                        var index = _reservations.IndexOf(_reservations.FirstOrDefault( r => r.ReservationId == reservationDTO.ReservationId));
                        _reservations[index] = reservationDTO;
                        exceptionPosition = 3;
                        _continualReservations[index] = reservationDTO;
                        exceptionPosition = 4;
                    }
                }
                else
                {
                    if (IsDayInRange(reservationDTO.Tours[0].StartDate))
                    {
                        exceptionPosition = 5;
                        _reservations.Add(reservationDTO);
                        exceptionPosition = 6;
                        _continualReservations.Add(reservationDTO);
                    }
                    else
                        SelectedDate = reservationDTO.Tours[0].StartDate;

                }
                if (!e.IsDbWon)
                {
                    exceptionPosition = 7;
                    Client.NotifyServer(SerializeReservationMessage(e.Reservation.ReservationId, eOperation.E_UPDATED),
                        eMsgTypes.E_RESERVATION);
                }
            }
            catch (Exception ex)
            {
                log.Error("CurrentReservationViewModel_ReservationUpdated failed. Position: " + exceptionPosition + ". error: " + ex.Message);
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
                    log.Error(string.Format("Exception in ShowCustomers. exception: {0}", ex.Message));
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
                try
                {
                    Client.NotifyServer(
                        SerializeReservationMessage(obj.ReservationId, eOperation.E_DELETED), eMsgTypes.E_RESERVATION);
                }
                catch(Exception ex)
                {
                    log.Error("Notify Server Error: " + ex.Message);
                }
            }
        }

        private void OnAddReservationCommand(object arg)
        {
            log.Debug("ReservationViewModel:OnAddCommand start");
            _eventAggregator.GetEvent<ReservationEditSelectedEvent>().Publish(new EditReservationEventArgs(null, false, false));
            log.Debug("ReservationViewModel:OnAddCommand end");
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
            log.Debug(string.Format("ReservationViewModel:OnEditReservationCommand start. ReservationID={0}", reservation.ReservationId));
            Reservation dbReservation = null;
            WithClient(_serviceFactory.CreateClient<IOrderService>(), reservationClient =>
            {
                dbReservation = reservationClient.EditReservation(reservation.ReservationId, CurrentOperator.Operator);
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
            _eventAggregator.GetEvent<ReservationEditSelectedEvent>().Publish(new EditReservationEventArgs(dbReservation, bViewMode, _bIsContinual));
            log.Debug("ReservationViewModel:OnEditReservationCommand end");
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
                        await reservations.ContinueWith((Task<ReservationDTO[]> e) =>
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
                    ((ReservationDTO)x).Tours[0].StartDate == _selectedDate;
                exceptionPosition = 1;
                ContinualReservationsView.Filter = null;
                exceptionPosition = 2;
                ContinualReservationsView.Filter += x =>
                ((ReservationDTO)x).Tours.ToList().Exists(tour => _selectedDate > tour.StartDate &&
                _selectedDate <= tour.EndDate);
            }
            catch (Exception ex)
            {
                log.Error("Exception in filter by day. Position: " + exceptionPosition + ". Error" + ex.Message);
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
            return new ReservationDTO()
            {
                ReservationId = reservation.ReservationId,
                FirstName = reservation.Customers[0] == null ? string.Empty : reservation.Customers[0].FirstName,
                LastName = reservation.Customers[0] == null ? string.Empty : reservation.Customers[0].LastName,
                Phone1 = reservation.Customers[0] == null ? string.Empty : reservation.Customers[0].Phone1,
                HotelName = reservation.Tours[0] == null ? string.Empty : reservation.Tours[0].TourHotels[0] == null ? "" : reservation.Tours[0].TourHotels[0].Hotel.HotelName,
                AgencyName = reservation.Agency == null ? string.Empty : reservation.Agency.AgencyName,
                AdvancePayment = reservation.AdvancePayment,
                TotalPrice = reservation.TotalPrice,
                PickUpTime = reservation.PickUpTime,
                Comments = reservation.Comments,
                Messages = reservation.Messages,
                Group = reservation.Group,
                GroupID = reservation.GroupID,
                ActualNumberOfCustomers = reservation.ActualNumberOfCustomers,
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
            return string.Format("{0}/{1}", reservation.TotalPrice, reservation.TotalPrice - reservation.AdvancePayment);
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
                return string.Format("{0}, {1}, Jordan Reservation ID: {2}", reservation.FirstTourTypeName,
                    reservation.Private ? "Private" : "Regular", reservation.GroupID);
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
            return reservation != null ? string.Format("{0}", reservation.FirstTourTypeName) : string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter,
            System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}
