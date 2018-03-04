using Core.Common.Contracts;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using System;
using System.Collections.Generic;
using Core.Common.Core;
using EchoDesertTrips.Desktop.Support;
using System.ComponentModel.Composition;

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class EditTripViewModel : ViewModelBase
    {
        private IServiceFactory _serviceFactory;
        private Tour _trip;
        
        public event EventHandler<TripEventArgs> TripUpdated; 
        public event EventHandler TripCancelled;

        //private TourViewModel _tripViewModel;

        [ImportingConstructor]
        public EditTripViewModel(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
            //_innerViewModel = TourViewModel;
            SaveCommand = new DelegateCommand<object>(OnSaveCommand, OnSaveCommandCanExecute);
            CancelCommand = new DelegateCommand<object>(OnCancelCommand);
            TourViewModel = new TourViewModel(_serviceFactory);
        }

        public TourViewModel TourViewModel { get; set; }

        //public EditTripViewModel(IServiceFactory serviceFactory, Trip trip)
        //{
        //    _serviceFactory = serviceFactory;
        //    _trip = trip;
        //    TourViewModel = new TourViewModel(_serviceFactory, _trip);
        //    SaveCommand = new DelegateCommand<object>(OnSaveCommand, OnSaveCommandCanExecute);
        //    CancelCommand = new DelegateCommand<object>(OnCancelCommand);
        //}

        //Provide ViewModelBase model or what properties inside 
        //The ViewModel. ValidateModel will preform a check on all registered models/properties
        protected override void AddModels(List<ObjectBase> models)
        {
            //models.Add(TourViewModel.Trip);
        }

        private bool OnSaveCommandCanExecute(object obj)
        {
            //return TourViewModel.Trip.IsDirty;
            return true;
        }

        private void OnCancelCommand(object obj)
        {
            TripCancelled?.Invoke(this, EventArgs.Empty);
        }

        //private void OnSaveCommand(object obj)
        //{
        //    ValidateModel();
        //    if (IsValid)
        //    {
        //        WithClient<ITripService>(_serviceFactory.CreateClient<ITripService>(), tripClient =>
        //        {
        //            bool isNewTrip = (_trip.TripId == 0);
        //            var savedTrip = tripClient.UpdateTrip(Trip); //Update or Add
        //            if (savedTrip != null)
        //            {
        //                TripUpdated?.Invoke(this, new TripEventArgs(savedTrip, isNewTrip));
        //            }
        //        });
        //    }
        //}

        private void OnSaveCommand(object obj)
        {
            TourViewModel.ValidateCurrentModel();
            if (TourViewModel.IsValid)
            {
                //ITripService
                WithClient<ITourService>(_serviceFactory.CreateClient<ITourService>(), tripClient =>
                {
                    //bool isNewTrip = (TourViewModel.Trip.TourId == 0);
                    //var tw = new TourWrapper();
                    //var tour = new Tour();
                    //tw.UpdateTour(TourViewModel.Trip, tour);
                    //var savedTrip = tripClient.UpdateTour(tour); //Update or Add
                    //if (savedTrip != null)
                    //{
                    //    TourViewModel.Trip.TourId = savedTrip.TourId;
                    //    TripUpdated?.Invoke(this, new TripEventArgs(TourViewModel.Trip, isNewTrip));
                    //}
                });
            }
        }

        public DelegateCommand<object> SaveCommand { get; private set; }
        public DelegateCommand<object> CancelCommand { get; private set; }

        public Tour Trip
        {
            get
            {
                return _trip;
            }
            //TODO : Remove
            set
            {
                _trip = value;
                TourViewModel.SetTrip(_trip);
            }
        }

         //public TourViewModel TourViewModel
        //{
        //    get
        //    {
        //        return _tripViewModel;
        //    }

        //    set
        //    {
        //        _tripViewModel = value;
        //    }
        //}

        //public ObservableCollection<TripType> TourTypes
        //{
        //    get
        //    {
        //        return _tripTypes;
        //    }

        //    set
        //    {
        //        _tripTypes = value;
        //        OnPropertyChanged(() => _tripTypes, false);
        //    }
        //}

        //public TripType CurrentTourType
        //{
        //    get
        //    {
        //        return _currentTripType;
        //    }
        //    set
        //    {
        //        if (value != null)
        //        {
        //            _currentTripType = value;
        //            Trip.TripTypeId = CurrentTourType.TripTypeId;
        //            Trip.TripType = CurrentTourType;
        //            OnPropertyChanged(() => CurrentTourType, false);
        //        }
        //    }
        //}
    }
}

