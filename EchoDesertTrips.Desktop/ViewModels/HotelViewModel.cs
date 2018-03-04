using System;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using Core.Common.Contracts;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Data;
using System.Windows.Controls;
using Core.Common.Core;
using EchoDesertTrips.Desktop.Support;
using Core.Common.Utils;

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class HotelViewModel : ViewModelBase
    {
        private IServiceFactory _serviceFactory;
        private IMessageDialogService _messageDialogService;

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
            CurrentHotelViewModel = new EditHotelViewModel(_serviceFactory, _messageDialogService, null);
            CurrentHotelViewModel.RoomTypes = RoomTypes;
            RegisterEvents();
        }

        private void OnEditHotelCommand(Hotel obj)
        {
            CurrentHotelViewModel = new EditHotelViewModel(_serviceFactory, _messageDialogService, obj);
            CurrentHotelViewModel.RoomTypes = RoomTypes;
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

        protected override void OnViewLoaded()
        {
            try
            {
                WithClient<IInventoryService>(_serviceFactory.CreateClient<IInventoryService>(), inventoryClient =>
                {
                    //Hotels.Clear();
                    RoomTypes.Clear();
                    InventoryData inventoryData = inventoryClient.GetHotelsData();
                    //foreach (var hotel in inventoryData.Hotels)
                    //    Hotels.Add(hotel);
                    foreach (var roomType in inventoryData.RoomTypes)
                        RoomTypes.Add(roomType);
                });
            }
            catch(Exception ex)
            {
                log.Error("Exception load hotels: " + ex.Message);
            }
        }

        private void CurrentHotelViewModel_HotelUpdated(object sender, HotelEventArgs e)
        {
            var mappedHotel = AutoMapperUtil.Map<Hotel, Hotel>(e.Hotel);
            if (!e.IsNew)
            {
                //This is done in order to update the Grid. Remember that in EditTripViewModel the updated trip
                //Is a temporary object and it is not part of the Grid collection trips.
                var hotel = Hotels.FirstOrDefault(item => item.HotelId == e.Hotel.HotelId);
                if (hotel != null)
                {
                    var index = Hotels.IndexOf(hotel);
                    Hotels[index] = mappedHotel;
                }
            }
            else
            {
                Hotels.Add(mappedHotel);
            }

            NotifyServer("CurrentHotelViewModel_HotelUpdated", 2);
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

    public class RoomTypeIdToRoomTypeNameConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var items = ((System.Windows.Data.CollectionViewGroup)(value))?.Items;
            var customers = new ObservableCollection<Customer>();
            foreach (var reservation in items.Cast<Reservation>())
            {
                foreach (var customer in reservation.Customers)
                {
                    customers.Add(customer);
                }
            }
            return customers;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /*public class HotelValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value,
            System.Globalization.CultureInfo cultureInfo)
        {
            Hotel hotel = (value as BindingGroup).Items[0] as Hotel;
            if (hotel.HotelName == string.Empty)
            {
                return new ValidationResult(false,
                    "Hotel name should not be empty");
            }
            else
            if (hotel.HotelRoomTypes.Capacity == 0)
            {
                return new ValidationResult(false,
                    "Room Types are empty");
            }
            else
            {
                var validationResult = ValidationResult.ValidResult;
                return validationResult;
            }
        }
    }*/
}
