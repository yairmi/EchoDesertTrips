using Core.Common.Contracts;
using Core.Common.UI.Core;
using Core.Common.Utils;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using EchoDesertTrips.Client.Proxies.Service_Proxies;
using EchoDesertTrips.Desktop.Support;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Threading;
using Microsoft.Practices.Prism.Commands;

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MainViewModel : ViewModelBase
    {
        private readonly IServiceFactory _serviceFactory;
        [ImportingConstructor]
        public MainViewModel(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
            LogOutCommand = new Core.Common.UI.Core.DelegateCommand<object>(OnLogOutCommand);
            log4net.Config.XmlConfigurator.Configure();
        }

        ~MainViewModel()
        {
            UnRegisterClient();
        }

        [Import]
        public DashboardViewModel DashboardViewModel { get; private set; }
        [Import]
        public ReservationsViewModel ReservationsViewModel { get; set; }
        [Import]
        public AdminViewModel AdminViewModel { get; set; }
        private LoginControlViewModel _loginControlViewModel;
        public LoginControlViewModel LoginControlViewModel
        {
            get
            {
                return _loginControlViewModel;
            }
            set
            {
                _loginControlViewModel = value;
                OnPropertyChanged(() => LoginControlViewModel);
            }
        }

        public Core.Common.UI.Core.DelegateCommand<object> LogOutCommand { get; protected set; }    

        private void OnLogOutCommand(object obj)
        {
            UnRegisterClient();
            Operator = null;
            MainWindowVisible = false;
            LoginVisible = true;
        }

        protected override void OnViewLoaded()
        {
            LoginControlViewModel = new LoginControlViewModel(_serviceFactory);
            LoginControlViewModel.Authenticated += LoginControlViewModel_Authenticated;
            LoginVisible = true;
        }

        private void LoginControlViewModel_Authenticated(object sender, AuthenticationEventArgs e)
        {
            log.Info("Loggin Successfully. Operator Name: = " + e.Operator.OperatorName + ". Operator ID: =" + e.Operator.OperatorId);
            Operator = e.Operator;
            DashboardViewModel.Operator = e.Operator;
            ReservationsViewModel.Operator = e.Operator;
            AdminViewModel.Operator = e.Operator;

            MainWindowVisible = true;
            LoginVisible = !MainWindowVisible;

            AdminVisible = e.Operator.Admin;
            log.Debug("LoginControlViewModel_Authenticated: Start Register Client");
            try
            {
                RegisterClient();
                ReservationsViewModel.Client = Client;
                AdminViewModel.Client = Client;
            }
            catch (Exception ex)
            {
                log.Error("Exception in Register Clients: " + ex.Message);
            }

            LoadInventoryAsync();

            log.Debug("OnViewLoaded: Client Registration finished");
        }

        private bool _mainWindowVisible;

        public bool MainWindowVisible
        {
            get
            {
                return _mainWindowVisible;
            }
            set
            {
                _mainWindowVisible = value;
                OnPropertyChanged(() => MainWindowVisible);
            }
        }

        private bool _loginVisible;

        public bool LoginVisible
        {
            get
            {
                return _loginVisible;
            }
            set
            {
                _loginVisible = value;
                OnPropertyChanged(() => LoginVisible);
            }
        }

        private bool _adminVisible;

        public bool AdminVisible
        {
            get
            {
                return _adminVisible;
            }
            set
            {
                _adminVisible = value;
                OnPropertyChanged(() => AdminVisible);
            }
        }

        public void HandleBroadcast(object sender, EventDataType e)
        {
            log.Debug("HandleBroadcast");
            string[] separators = { ";" };
            string[] strings = SimpleSplitter.Split(e.EventMessage, separators);
            //Reservations message
            if (strings[0] == "1")
            {
                var isDayInRange = ReservationsViewModel.IsDayInRange(Convert.ToDateTime(strings[1]));
                if (isDayInRange == true)
                    ReservationsViewModel.LoadReservationsForDayRangeAsync(Convert.ToDateTime(strings[1]));
            }
            //Inventory message
            if (strings[0] == "2")
            {
                LoadInventoryAsync();
            }
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
                }
                else
                if (Client != null && Client.InnerDuplexChannel.State == System.ServiceModel.CommunicationState.Faulted)
                {
                        Client.Abort();
                        Client = null;
                        CreateClient();
                }
                Client.UnRegisterClient(operatorNameId);
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

                            ReservationsViewModel.TourTypes = TourTypes;
                            ReservationsViewModel.Hotels = Hotels;
                            ReservationsViewModel.Optionals = Optionals;
                            ReservationsViewModel.Agencies = Agencies;

                            AdminViewModel.TourTypes = TourTypes;
                            AdminViewModel.Hotels = Hotels;
                            AdminViewModel.Optionals = Optionals;
                            AdminViewModel.Agencies = Agencies;

                        }, null);
                    }
                });
            }

            var disposableClient = inventoryClient as IDisposable;
            disposableClient?.Dispose();
        }
    }
}
