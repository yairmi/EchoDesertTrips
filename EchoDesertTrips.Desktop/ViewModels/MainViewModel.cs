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
            _editReservationViewModel.Operator = Operator;
            _editReservationViewModel.Hotels = Hotels;
            _editReservationViewModel.TourTypes = TourTypes;
            _editReservationViewModel.Optionals = Optionals;
            _editReservationViewModel.Agencies = Agencies;
            _editReservationViewModel.SetReservation(e._reservation);
            _editReservationViewModel.ReservationUpdated -= _editReservationViewModel_ReservationUpdated;
            _editReservationViewModel.ReservationUpdated += _editReservationViewModel_ReservationUpdated;
            _editReservationViewModel.ReservationCancelled -= _editReservationViewModel_ReservationCancelled;
            _editReservationViewModel.ReservationCancelled += _editReservationViewModel_ReservationCancelled;
            CurrentViewModel = _editReservationViewModel;
        }

        private void _editReservationViewModel_ReservationCancelled(object sender, ReservationEventArgs e)
        {
            //_editReservationViewModel = null;
            CurrentViewModel = _mainTabViewModel;
        }

        private void _editReservationViewModel_ReservationUpdated(object sender, ReservationEventArgs e)
        {
            //_editReservationViewModel = null;
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

        public void HandleBroadcast(object sender, EventDataType e)
        {
            log.Debug("HandleBroadcast");
            string[] separators = { ";" };
            string[] strings = SimpleSplitter.Split(e.EventMessage, separators);
            //Reservations message
            if (strings[0] == "1")
            {
                //var isDayInRange = _mainTabViewModel.ReservationsViewModel.IsDayInRange(Convert.ToDateTime(strings[1]));
                //if (isDayInRange == true)
                _mainTabViewModel.ReservationsViewModel.LoadReservationsForDayRangeAsync(Convert.ToDateTime(strings[1]));
            }
            //Inventory message
            if (strings[0] == "2")
            {
                LoadInventoryAsync();
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
                                foreach (var tourType in inventoryData.Result.TourTypes)
                                {
                                    if (tourType.Visible)
                                        TourTypes.Add(TourTypeHelper.CreateTourTypeWrapper(tourType));
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

                                foreach (var agency in inventoryData.Result.Agencies)
                                {
                                    Agencies.Add(agency);
                                }

                                _mainTabViewModel.ReservationsViewModel.TourTypes = TourTypes;
                                _mainTabViewModel.ReservationsViewModel.Hotels = Hotels;
                                _mainTabViewModel.ReservationsViewModel.Optionals = Optionals;
                                _mainTabViewModel.ReservationsViewModel.Agencies = Agencies;

                                _mainTabViewModel.AdminViewModel.TourTypes = TourTypes;
                                _mainTabViewModel.AdminViewModel.Hotels = Hotels;
                                _mainTabViewModel.AdminViewModel.Optionals = Optionals;
                                _mainTabViewModel.AdminViewModel.Agencies = Agencies;
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
