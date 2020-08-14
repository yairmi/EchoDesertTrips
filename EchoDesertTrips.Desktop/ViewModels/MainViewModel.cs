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
using EchoDesertTrips.Client.Entities;

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
            try
            {
                Client.UnRegisterClient();
            }
            catch(Exception ex)
            {
                log.Error("Failed to UnRegisterClient", ex);
            }
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
            try
            {
                bool bSucceeded = Client.UnRegisterClient();
                log.Info($"UnRegisterClient Succeeded for Operator = {CurrentOperator.Operator.OperatorName}"); ;

                CurrentOperator.Operator = null;
                CurrentViewModel = _loginControlViewModel;
            }
            catch(Exception ex)
            {
                log.Error(ex.StackTrace);
                _messageBoxDialogService.ShowInfoDialog("Failed to Logout User", "Error!");
            }
        }

        private void Authenticated(AuthenticationEventArgs e)
        {
            log.Debug("Start");
            bool bSucceeded = false;
            
            try
            {
                bSucceeded = Client.RegisterClient(HandleBroadcast);
                if (bSucceeded == false)
                {
                    OperatorForegroundColor = System.Windows.Media.Brushes.DarkRed;
                    throw new Exception();
                }
                else
                {
                    log.Info($"Client registration succeeded for Operator = {CurrentOperator.Operator.OperatorName}");
                    OperatorForegroundColor = System.Windows.Media.Brushes.Black;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.StackTrace);
                _messageBoxDialogService.ShowInfoDialog("Failed To Register client.\nWorking in ReadOnly mode", "Error!");
            }
            finally
            {
                try
                {
                    LoadInventoryAsync();
                }
                catch(Exception ex)
                {
                    log.Error(ex.StackTrace);
                }

                CurrentViewModel = _mainTabViewModel;
            }

            log.Debug("Finished");
        }

        private System.Windows.Media.Brush _operatorForegroundColor;
        public System.Windows.Media.Brush OperatorForegroundColor
        {
            get
            {
                return _operatorForegroundColor;
            }
            set
            {
                _operatorForegroundColor = value;
                OnPropertyChanged(()=> OperatorForegroundColor);
            }
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
            log.Debug("Start");
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
                    log.Debug($"Hotel Updated - {inventories.Hotels[0].HotelName}");
                    Inventories.Update<Hotel>(inventories.Hotels[0], Inventories.Hotels);
                }
                else if (inventories.Operators != null)
                {
                    log.Debug($"Operator Updated - {inventories.Operators[0].OperatorFullName}");
                    Inventories.Update<Operator>(inventories.Operators[0], Inventories.Operators);
                }
                else if (inventories.Optionals != null)
                {
                    log.Debug($"Optional Updated - {inventories.Optionals[0].OptionalDescription}");
                    Inventories.Update<Optional>(inventories.Optionals[0], Inventories.Optionals);
                    _eventAggregator.GetEvent<OptionalUpdatedEvent>().Publish(new OptionalEventArgs(inventories.Optionals[0], false));
                }
                else if (inventories.RoomTypes != null)
                {
                    log.Debug($"RoomType Updated - {inventories.RoomTypes[0].RoomTypeName}");
                    Inventories.Update<RoomType>(inventories.RoomTypes[0], Inventories.RoomTypes);
                }
                else if (inventories.TourTypes != null)
                {
                    log.Debug($"TourType Updated - {inventories.TourTypes[0].TourTypeName}");
                    Inventories.Update<TourType>(inventories.TourTypes[0], Inventories.TourTypes);
                }
                else if (inventories.Agencies != null)
                {
                    log.Debug($"Agency Updated - {inventories.Agencies[0].AgencyName}");
                    Inventories.Update<Agency>(inventories.Agencies[0], Inventories.Agencies);
                }
            }
            catch(Exception ex)
            {
                log.Error("Failed to update inventory", ex);
            }
        }

        private async void LoadInventoryAsync()
        {
            IInventoryService inventoryClient = null;
            try
            {
                inventoryClient = _serviceFactory.CreateClient<IInventoryService>();
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
                                log.Debug("Inventories Loading");
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
                            }, null);
                        }
                    });
                }
            }
            catch(Exception)
            {
                throw;
            }
            finally
            {
                var disposableClient = inventoryClient as IDisposable;
                disposableClient?.Dispose();
            }
        }
    }
}
