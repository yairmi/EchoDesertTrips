using Core.Common.Contracts;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Contracts;
using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Threading;
using static Core.Common.Core.Const;
using Core.Common.UI.PubSubEvent;
using Core.Common.UI.CustomEventArgs;

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MainViewModel : ViewModelBase
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IMessageDialogService _messageBoxDialogService;
        
        [ImportingConstructor]
        public MainViewModel(IServiceFactory serviceFactory, 
            IMessageDialogService messageBoxDialogService)
        {
            _serviceFactory = serviceFactory;
            _messageBoxDialogService = messageBoxDialogService;
            LogOutCommand = new DelegateCommand<object>(OnLogOutCommand);
            _eventAggregator.GetEvent<ReservationCancelledEvent>().Subscribe(ReservationCancelled);
            _eventAggregator.GetEvent<ReservationUpdatedFinishedEvent>().Subscribe(ReservationUpdatedFinished);
            _eventAggregator.GetEvent<ReservationEditSelectedEvent>().Subscribe(ReservationEditSelected);
            _eventAggregator.GetEvent<AuthenticatedEvent>().Subscribe(Authenticated);
            log4net.Config.XmlConfigurator.Configure();
        }

        protected override void OnViewLoaded()
        {
            CurrentViewModel = _loginControlViewModel;
        }

        ~MainViewModel()
        {
            Client.UnRegisterClient();
        }

        [Import]
        private MainTabViewModel _mainTabViewModel { get; set; }
        [Import]
        private LoginControlViewModel _loginControlViewModel { get; set; }
        [Import]
        private EditReservationViewModel _editReservationViewModel { get; set; }//It is here since I want that the edit reservation screen to replace the Main Tab screen after the user edit a reservation

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

        public DelegateCommand<object> LogOutCommand { get; protected set; }

        private void OnLogOutCommand(object obj)
        {
            Client.UnRegisterClient();
            CurrentOperator.Operator = null;
            CurrentViewModel = _loginControlViewModel;
        }

        private void Authenticated(AuthenticationEventArgs e)
        {
            log.Info("Loggin Successfully. Operator Name: = " + e.Operator.OperatorName + ". Operator ID: =" + e.Operator.OperatorId);
            CurrentOperator.Operator = e.Operator;
            CurrentViewModel = _mainTabViewModel;

            log.Debug("LoginControlViewModel_Authenticated: Start Register Client");
            try
            {
                Client.RegisterClient(HandleBroadcast);
            }
            catch (Exception ex)
            {
                log.Error("Exception in Register Clients: " + ex.Message);
            }

            LoadInventoryAsync();

            log.Debug("OnViewLoaded: Client Registration finished");
        }

        private void ReservationEditSelected(EditReservationEventArgs e)
        {
            _eventAggregator.GetEvent<ReservationEditSelectedFinishedEvent>().Publish(e);
            CurrentViewModel = _editReservationViewModel;
        }

        private void ReservationCancelled(ReservationEventArgs obj)
        {
            CurrentViewModel = _mainTabViewModel;
        }

        private void ReservationUpdatedFinished(object obj)
        {
            CurrentViewModel = _mainTabViewModel;
        }

        public void HandleBroadcast(object sender, BroadcastMessage Message)
        {
            log.Debug("HandleBroadcast");
            if (Message.MessageType == eMsgTypes.E_RESERVATION)
            {
                foreach (var reservation in Message.ReservationsResult)
                    _eventAggregator.GetEvent<ReservationUpdatedEvent>().Publish(reservation);
                foreach (var reservationId in Message.ReservationsIdsToDelete)
                    _eventAggregator.GetEvent<ReservationRemovedEvent>().Publish(reservationId);
            }
            else if (Message.MessageType == eMsgTypes.E_INVENTORY)
                UpdateInventory(Message.Inventories);
        }

        private void UpdateInventory(InventoryData inventories)
        {
            try
            {
                if (inventories.Hotels != null)
                {
                    log.Debug("UpdateInventory: Hotel Updated - " + inventories.Hotels[0].HotelName);
                    Inventories.Update(inventories.Hotels[0]);
                }
                else if (inventories.Operators != null)
                {
                    log.Debug("UpdateInventory: Operators - " + inventories.Operators[0].OperatorFullName);
                    Inventories.Update(inventories.Operators[0]);
                }
                else if (inventories.Optionals != null)
                {
                    log.Debug("UpdateInventory: Optionals - " + inventories.Optionals[0].OptionalDescription);
                    Inventories.Update(inventories.Optionals[0]);
                    _eventAggregator.GetEvent<OptionalUpdatedEvent>().Publish(new OptionalEventArgs(inventories.Optionals[0], false));
                }
                else if (inventories.RoomTypes != null)
                {
                    log.Debug("UpdateInventory: RoomTypes - " + inventories.RoomTypes[0].RoomTypeName);
                    Inventories.Update(inventories.RoomTypes[0]);
                }
                else if (inventories.TourTypes != null)
                {
                    log.Debug("UpdateInventory: TourTypes - " + inventories.TourTypes[0].TourTypeName);
                    Inventories.Update(inventories.TourTypes[0]);
                }
                else if (inventories.Agencies != null)
                {
                    log.Debug("UpdateInventory: Hotel Updated - " + inventories.Agencies[0].AgencyName);
                    Inventories.Update(inventories.Agencies[0]);
                }
            }
            catch(Exception ex)
            {
                log.Error("Error! failed to update inventory: " + ex.Message);
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
                                Inventories.TourTypes.Clear();
                                Inventories.Hotels.Clear();
                                Inventories.Optionals.Clear();
                                Inventories.Agencies.Clear();
                                Inventories.Operators.Clear();
                                Inventories.TourTypes.AddRange(inventoryData.Result.TourTypes);
                                Inventories.Hotels.AddRange(inventoryData.Result.Hotels);
                                Inventories.Optionals.AddRange(inventoryData.Result.Optionals);
                                Inventories.Agencies.AddRange(inventoryData.Result.Agencies);
                                Inventories.Operators.AddRange(inventoryData.Result.Operators);
                                Inventories.RoomTypes.AddRange(inventoryData.Result.RoomTypes);
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
