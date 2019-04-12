using System;
using System.ComponentModel.Composition;
using Core.Common.Contracts;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Entities;
using System.Linq;
using static Core.Common.Core.Const;
using Core.Common.UI.PubSubEvent;
using Core.Common.UI.CustomEventArgs;
using EchoDesertTrips.Client.Contracts;

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class HotelViewModel : ViewModelBase
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IMessageDialogService _messageDialogService;

        [ImportingConstructor]
        public HotelViewModel(IServiceFactory serviceFactory,
            IMessageDialogService messageDialogService)
        {
            _serviceFactory = serviceFactory;
            _messageDialogService = messageDialogService;
            _eventAggregator.GetEvent<HotelUpdatedEvent>().Subscribe(HotelUpdated);
            _eventAggregator.GetEvent<HotelCancelledEvent>().Subscribe(HotelCancelled);
            EditHotelCommand = new DelegateCommand<Hotel>(OnEditHotelCommand);
            AddHotelCommand = new DelegateCommand<object>(OnAddHotelCommand);
        }

        private void OnAddHotelCommand(object obj)
        {
            CurrentHotelViewModel =
                new EditHotelViewModel(_serviceFactory, _messageDialogService, new Hotel());
        }

        private void OnEditHotelCommand(Hotel hotel)
        {
            Hotel dbHotel = null;
            WithClient(_serviceFactory.CreateClient<IInventoryService>(), hotelClient =>
            {
                dbHotel = hotelClient.GetHotelById(hotel.HotelId);
            });
            if (dbHotel == null)
            {
                _messageDialogService.ShowInfoDialog("Could not load Hotel,\nHotel was not found.", "Info");
                return;
            }
            CurrentHotelViewModel =
                new EditHotelViewModel(_serviceFactory, _messageDialogService, dbHotel);
        }

        public DelegateCommand<Hotel> EditHotelCommand { get; private set; }
        public DelegateCommand<object> AddHotelCommand { get; private set; }

        private EditHotelViewModel _editHotelViewModel;

        public EditHotelViewModel CurrentHotelViewModel
        {
            get { return _editHotelViewModel; }
            set
            {
                if (_editHotelViewModel != value)
                {
                    _editHotelViewModel = value;
                    OnPropertyChanged(() => CurrentHotelViewModel, false);
                }
            }
        }

        public DelegateCommand<Hotel> SaveHotelCommand { get; set; }
        public DelegateCommand<Hotel> UpdateHotelCommand { get; set; }

        public override string ViewTitle => "Hotels";

        private void HotelUpdated(HotelEventArgs e)
        {
            Inventories.Update(e.Hotel);
            if (e.bSendUpdateToClients)
            {
                try
                {
                    Client.NotifyServer(
                        SerializeInventoryMessage(eInventoryTypes.E_HOTEL, eOperation.E_UPDATED, e.Hotel.HotelId), eMsgTypes.E_INVENTORY);
                }
                catch (Exception ex)
                {
                    log.Error("Notify Server Error: " + ex.Message);
                }
                CurrentHotelViewModel = null;
            }
        }

        private void HotelCancelled(HotelEventArgs obj)
        {
            CurrentHotelViewModel = null;
        }
    }
}
