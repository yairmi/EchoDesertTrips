using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Core.Common.Contracts;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using System.Linq;
using System.Windows.Data;
using Core.Common.Utils;
using EchoDesertTrips.Desktop.CustomEventArgs;
using static Core.Common.Core.Const;

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
            EditHotelCommand = new DelegateCommand<Hotel>(OnEditHotelCommand);
            AddHotelCommand = new DelegateCommand<object>(OnAddHotelCommand);
        }

        private void OnAddHotelCommand(object obj)
        {
            CurrentHotelViewModel =
                new EditHotelViewModel(_serviceFactory, _messageDialogService, null) {RoomTypes = RoomTypes};
            RegisterEvents();
        }

        private void OnEditHotelCommand(Hotel obj)
        {
            CurrentHotelViewModel =
                new EditHotelViewModel(_serviceFactory, _messageDialogService, obj) {RoomTypes = RoomTypes};
            RegisterEvents();
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

        private void CurrentHotelViewModel_HotelUpdated(object sender, HotelEventArgs e)
        {
            if (!e.IsNew)
            {
                var hotel = Hotels.FirstOrDefault(item => item.HotelId == e.Hotel.HotelId);
                if (hotel != null)
                {
                    var index = Hotels.IndexOf(hotel);
                    Hotels[index] = e.Hotel;
                }
            }
            else
            {
                Hotels.Add(e.Hotel);
            }

            NotifyServer("CurrentHotelViewModel_HotelUpdated",
                SerializeInventoryMessage(eInventoryTypes.E_HOTEL, eOperation.E_UPDATED, e.Hotel.HotelId), eMsgTypes.E_INVENTORY);

            CurrentHotelViewModel = null;
        }

        private void CurrentHotelViewModel_HotelCancelled(object sender, HotelEventArgs e)
        {
            CurrentHotelViewModel = null;
        }

        private void RegisterEvents()
        {
            CurrentHotelViewModel.HotelUpdated -= CurrentHotelViewModel_HotelUpdated;
            CurrentHotelViewModel.HotelUpdated += CurrentHotelViewModel_HotelUpdated;
            CurrentHotelViewModel.HotelCancelled -= CurrentHotelViewModel_HotelCancelled;
            CurrentHotelViewModel.HotelCancelled += CurrentHotelViewModel_HotelCancelled;
        }
    }
}
