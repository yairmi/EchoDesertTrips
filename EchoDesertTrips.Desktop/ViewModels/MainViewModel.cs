using Core.Common.Contracts;
using Core.Common.UI.Core;
using Core.Common.Utils;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using EchoDesertTrips.Client.Proxies.Service_Proxies;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Threading;
using EchoDesertTrips.Desktop.CustomEventArgs;
using EchoDesertTrips.Common;
using static Core.Common.Core.Const;
using System.Linq;

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MainViewModel : ViewModelBase
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IMessageDialogService _messageBoxDialogService;
        
        [ImportingConstructor]
        public MainViewModel(IServiceFactory serviceFactory, IMessageDialogService messageBoxDialogService)
        {
            _serviceFactory = serviceFactory;
            _messageBoxDialogService = messageBoxDialogService;
            LogOutCommand = new Core.Common.UI.Core.DelegateCommand<object>(OnLogOutCommand);
            log4net.Config.XmlConfigurator.Configure();
        }
        protected override void OnViewLoaded()
        {
            _loginControlViewModel.Authenticated -= LoginControlViewModel_Authenticated;
            _loginControlViewModel.Authenticated += LoginControlViewModel_Authenticated;

            _editReservationViewModel.ReservationUpdated -= _editReservationViewModel_ReservationUpdated;
            _editReservationViewModel.ReservationUpdated += _editReservationViewModel_ReservationUpdated;
            _editReservationViewModel.ReservationCancelled -= _editReservationViewModel_ReservationCancelled;
            _editReservationViewModel.ReservationCancelled += _editReservationViewModel_ReservationCancelled;

            CurrentViewModel = _loginControlViewModel;
        }

        ~MainViewModel()
        {
            UnRegisterClient();
        }
        [Import]
        private MainTabViewModel _mainTabViewModel { get; set; }
        [Import]
        private LoginControlViewModel _loginControlViewModel { get; set; }
        [Import]
        private EditReservationViewModel _editReservationViewModel { get; set; }

        private ViewModelBase _currentViewModel;

        public ViewModelBase CurrentViewModel
        {
            get
            {
                return _currentViewModel;
            }
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(() => CurrentViewModel);
            }
        }

        public Core.Common.UI.Core.DelegateCommand<object> LogOutCommand { get; protected set; }

        private void OnLogOutCommand(object obj)
        {
            UnRegisterClient();
            Operator = null;
            CurrentViewModel = _loginControlViewModel;
        }

        private void LoginControlViewModel_Authenticated(object sender, AuthenticationEventArgs e)
        {
            log.Info("Loggin Successfully. Operator Name: = " + e.Operator.OperatorName + ". Operator ID: =" + e.Operator.OperatorId);
            Operator = e.Operator;
            _mainTabViewModel.Operator = e.Operator;
            _mainTabViewModel.ReservationsViewModel.ReservationEdited -= MainTabViewModel_ReservationEdited;
            _mainTabViewModel.ReservationsViewModel.ReservationEdited += MainTabViewModel_ReservationEdited;
            CurrentViewModel = _mainTabViewModel;

            log.Debug("LoginControlViewModel_Authenticated: Start Register Client");
            try
            {
                RegisterClient();
                _mainTabViewModel.Client = Client;
            }
            catch (Exception ex)
            {
                log.Error("Exception in Register Clients: " + ex.Message);
            }

            LoadInventoryAsync();

            log.Debug("OnViewLoaded: Client Registration finished");
        }

        private void MainTabViewModel_ReservationEdited(object sender, EditReservationEventArgs e)
        {
            _editReservationViewModel.SetReservation(e.Reservation);
            _editReservationViewModel.ViewMode = e.ViewMode;
            _editReservationViewModel.SelectedTabIndex = e.IsContinual == false ? 0 : 2;
            CurrentViewModel = _editReservationViewModel;
        }

        private void _editReservationViewModel_ReservationCancelled(object sender, ReservationEventArgs e)
        {
            CurrentViewModel = _mainTabViewModel;
        }

        private void _editReservationViewModel_ReservationUpdated(object sender, ReservationEventArgs e)
        {
            _mainTabViewModel.ReservationsViewModel.UpdateReservations(e);
            CurrentViewModel = _mainTabViewModel;
        }

        public void RegisterClient()
        {
            if (Client != null)
            {
                Client.Abort();
                Client = null;
            }

            var cb = new BroadcastorCallback();
            cb.SetHandler(HandleBroadcast);

            System.ServiceModel.InstanceContext context =
                new System.ServiceModel.InstanceContext(cb);
            Client =
                new BroadcastorServiceClient(context);

            var operatorNameId = Operator.OperatorName + "-" + Operator.OperatorId;

            this.Client.RegisterClient(operatorNameId);
        }

        public void UnRegisterClient()
        {
            if (Operator == null)
                return;
            log.Debug("UnRegisterClient Client Started: " + Operator.OperatorName);
            var operatorNameId = Operator.OperatorName + "-" + Operator.OperatorId;
            try
            {
                if (Client == null)
                {
                    CreateClient();
                    log.Debug("UnRegisterClient. Client was NULL. After Client Creation");
                }
                else
                if (Client != null && Client.InnerDuplexChannel.State == System.ServiceModel.CommunicationState.Faulted)
                {
                    Client.Abort();
                    Client = null;
                    CreateClient();
                    log.Debug("UnRegisterClient. Client was NOT NULL. After Client Creation");
                }
                Client.UnRegisterClient(operatorNameId);
                log.Debug("UnRegisterClient. After UnRegister Client");
            }
            catch (Exception ex)
            {
                log.Error("UnRegisterClient Exception: " + ex.Message);
            }
        }

        private void CreateClient()
        {
            var cb = new BroadcastorCallback();
            cb.SetHandler(HandleBroadcast);

            var context =
                new System.ServiceModel.InstanceContext(new BroadcastorCallback());
            Client =
                new BroadcastorServiceClient(context);
        }

        public void HandleBroadcast(object sender, BroadcastMessage Message)
        {
            log.Debug("HandleBroadcast");
            if (Message.MessageType == eMsgTypes.E_RESERVATION)
            {
                foreach (var reservation in Message.ReservationsResult)
                    _mainTabViewModel.ReservationsViewModel.UpdateReservations(new ReservationEventArgs(reservation, false, true));
                foreach (var reservationId in Message.ReservationsIdsToDelete)
                    _mainTabViewModel.ReservationsViewModel.RemoveReservationFromGUI(reservationId);
            }
            else if (Message.MessageType == eMsgTypes.E_INVENTORY)
                UpdateInventory(Message.Inventories);
        }

        private void UpdateInventory(InventoryData inventories)
        {
            if (inventories.Hotels != null)
            {
                var hotel = Hotels.FirstOrDefault(item => item.HotelId == inventories.Hotels[0].HotelId);
                if (hotel != null)
                {
                    var index = Hotels.IndexOf(hotel);
                    Hotels[index] = inventories.Hotels[0];
                }
                else
                {
                    Hotels.Add(inventories.Hotels[0]);
                }
                _mainTabViewModel.ReservationsViewModel.Hotels = Hotels;
                _mainTabViewModel.AdminViewModel.Hotels = Hotels;
                _editReservationViewModel.Hotels = Hotels;
            }
            else if (inventories.Operators != null)
            {
                var _operator = Operators.FirstOrDefault(item => item.OperatorId == inventories.Operators[0].OperatorId);
                if (_operator != null)
                {
                    var index = Operators.IndexOf(_operator);
                    Operators[index] = inventories.Operators[0];
                }
                else
                {
                    Operators.Add(inventories.Operators[0]);
                }
                _mainTabViewModel.ReservationsViewModel.Operators = Operators;
                _mainTabViewModel.AdminViewModel.Operators = Operators;
                _editReservationViewModel.Operators = Operators;
            }
            else if (inventories.Optionals != null)
            {
                var optional = Optionals.FirstOrDefault(item => item.OptionalId == inventories.Optionals[0].OptionalId);
                if (optional != null)
                {
                    var index = Optionals.IndexOf(optional);
                    Optionals[index] = inventories.Optionals[0];
                }
                else
                {
                    Optionals.Add(inventories.Optionals[0]);
                }
                _mainTabViewModel.ReservationsViewModel.Optionals = Optionals;
                _mainTabViewModel.AdminViewModel.Optionals = Optionals;
                _editReservationViewModel.Optionals = Optionals;
            }
            else if (inventories.RoomTypes != null)
            {
                var roomType = RoomTypes.FirstOrDefault(item => item.RoomTypeId == inventories.RoomTypes[0].RoomTypeId);
                if (roomType != null)
                {
                    var index = RoomTypes.IndexOf(roomType);
                    RoomTypes[index] = inventories.RoomTypes[0];
                }
                else
                {
                    RoomTypes.Add(inventories.RoomTypes[0]);
                }
                _mainTabViewModel.ReservationsViewModel.RoomTypes = RoomTypes;
                _mainTabViewModel.AdminViewModel.RoomTypes = RoomTypes;
                _editReservationViewModel.RoomTypes = RoomTypes;
            }
            else if (inventories.TourTypes != null)
            {
                var tourType = TourTypes.FirstOrDefault(item => item.TourTypeId == inventories.TourTypes[0].TourTypeId);
                if (tourType != null)
                {
                    var index = TourTypes.IndexOf(tourType);
                    TourTypes[index] = inventories.TourTypes[0];
                }
                else
                {
                    TourTypes.Add(inventories.TourTypes[0]);
                }
                _mainTabViewModel.ReservationsViewModel.TourTypes = TourTypes;
                _mainTabViewModel.AdminViewModel.TourTypes = TourTypes;
                _editReservationViewModel.TourTypes = TourTypes;
            }
            else if (inventories.Agencies != null)
            {
                var agency = Agencies.FirstOrDefault(item => item.AgencyId == inventories.Agencies[0].AgencyId);
                if (agency != null)
                {
                    var index = Agencies.IndexOf(agency);
                    Agencies[index] = inventories.Agencies[0];
                }
                else
                {
                    Agencies.Add(inventories.Agencies[0]);
                }
                _mainTabViewModel.ReservationsViewModel.Agencies = Agencies;
                _mainTabViewModel.AdminViewModel.Agencies = Agencies;
                _editReservationViewModel.Agencies = Agencies;
            }
        }

        private async void LoadInventoryAsync()
        {
            var inventoryClient = _serviceFactory.CreateClient<IInventoryService>();
            {
                var uiContext = SynchronizationContext.Current;
                var task = Task.Factory.StartNew(() => inventoryClient.GetInventoryDataAsynchronous());
                var inventoryData = await task;
                await inventoryData.ContinueWith(e =>
                {
                    if (e.IsCompleted)
                    {
                        uiContext.Send((x) =>
                        {
                            try
                            {
                                log.Debug("Inventory Loading");
                                TourTypes.Clear();
                                Hotels.Clear();
                                Optionals.Clear();
                                Agencies.Clear();
                                Operators.Clear();
                                foreach (var tourType in inventoryData.Result.TourTypes)
                                {
                                    if (tourType.Visible)
                                        TourTypes.Add(tourType);
                                }
                                foreach (var hotel in inventoryData.Result.Hotels)
                                {
                                    if (hotel.Visible)
                                        Hotels.Add(hotel);
                                }

                                foreach (var optional in inventoryData.Result.Optionals)
                                {
                                    if (optional.Visible)
                                        Optionals.Add(optional);
                                }

                                Agencies.AddRange(inventoryData.Result.Agencies);
                                Operators.AddRange(inventoryData.Result.Operators);
                                RoomTypes.AddRange(inventoryData.Result.RoomTypes);

                                _mainTabViewModel.ReservationsViewModel.TourTypes = TourTypes;
                                _mainTabViewModel.ReservationsViewModel.Hotels = Hotels;
                                _mainTabViewModel.ReservationsViewModel.Optionals = Optionals;
                                _mainTabViewModel.ReservationsViewModel.Agencies = Agencies;

                                _mainTabViewModel.AdminViewModel.TourTypes = TourTypes;
                                _mainTabViewModel.AdminViewModel.Hotels = Hotels;
                                _mainTabViewModel.AdminViewModel.Optionals = Optionals;
                                _mainTabViewModel.AdminViewModel.Agencies = Agencies;
                                _mainTabViewModel.AdminViewModel.Operators = Operators;
                                _mainTabViewModel.AdminViewModel.RoomTypes = RoomTypes;

                                _editReservationViewModel.Operator = Operator;
                                _editReservationViewModel.Hotels = Hotels;
                                _editReservationViewModel.TourTypes = TourTypes;
                                _editReservationViewModel.Optionals = Optionals;
                                _editReservationViewModel.Agencies = Agencies;
                            }
                            catch(Exception ex)
                            {
                                log.Error("LoadInventoryAsync Exception:" + ex.Message);
                            }
                        }, null);
                    }
                });
            }

            var disposableClient = inventoryClient as IDisposable;
            disposableClient?.Dispose();
        }
    }
}
