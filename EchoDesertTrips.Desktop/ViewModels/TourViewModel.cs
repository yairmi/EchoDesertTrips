using Core.Common.Contracts;
using Core.Common.Core;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;

namespace EchoDesertTrips.Desktop.ViewModels
{
    public class TourViewModel : ViewModelBase
    {
        private IServiceFactory _serviceFactory;
 
        public TourViewModel(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        public void SetTrip(Tour trip)
        {
            //var tw = new TourWrapper();
            //Trip = tw.CopyTour(trip);
        }

        private TourWrapper _tour;

        public TourWrapper Tour
        {
            get
            {
                return _tour;
            }
            //TODO : remove
            set
            {
                _tour = value;
                OnPropertyChanged(() => Tour, false);
            }
        }

        private ObservableCollection<TourType> _tripTypes;

        public ObservableCollection<TourType> TripTypes
        {
            get
            {
                return _tripTypes;
            }

            set
            {
                _tripTypes = value;
                OnPropertyChanged(() => TripTypes, false);
            }
        }

        private ObservableCollection<Hotel> _hotels;

        public ObservableCollection<Hotel> Hotels
        {
            get
            {
                return _hotels;
            }

            set
            {
                _hotels = value;
                OnPropertyChanged(() => Hotels, false);
            }
        }

        private TourType _currentTourType;

        public TourType CurrentTourType
        {
            get
            {
                return _currentTourType;
            }
            set
            {
                if (value != null)
                {
                    _currentTourType = value;
                    Tour.TourTypeId = CurrentTourType.TourTypeId;
                    Tour.TourType = CurrentTourType;
                    OnPropertyChanged(() => CurrentTourType, false);
                }
            }
        }

        //private Hotel _currentHotel;

        //public Hotel CurrentHotel
        //{
        //    get
        //    {
        //        return _currentHotel;
        //    }
        //    set
        //    {
        //        if (value != null)
        //        {
        //            _currentHotel = value;
        //            Trip.HotelId = CurrentHotel.HotelId;
        //            Trip.Hotel = CurrentHotel;
        //            OnPropertyChanged(() => CurrentHotel, false);
        //        }
        //    }
        //}

        //Provide ViewModelBase model or what properties inside 
        //The ViewModel. ValidateModel will preform a check on all registered models/properties
        protected override void AddModels(List<ObjectBase> models)
        {
            models.Add(Tour);
        }

        public void ValidateCurrentModel()
        {
            ValidateModel();
        }

        protected override void OnViewLoaded()
        {
            WithClient<IInventoryService>(_serviceFactory.CreateClient<IInventoryService>(), inventoryClient =>
            {
                var inventoryData = inventoryClient.GetInventoryData();
                //_hotels = new ObservableCollection<Hotel>(inventoryData.Hotels);
                _tripTypes = new ObservableCollection<TourType>(inventoryData.TourTypes);
            });
            CurrentTourType = _tripTypes.FirstOrDefault(n => n.TourTypeId == Tour.TourTypeId);
            //CurrentHotel = _hotels.FirstOrDefault(n => n.HotelId == Trip.HotelId);
            //Init();
        }
    }
}
