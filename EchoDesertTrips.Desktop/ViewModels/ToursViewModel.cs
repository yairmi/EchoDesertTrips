using Core.Common.Contracts;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System;
using EchoDesertTrips.Desktop.Support;
using System.Linq;
using System.ComponentModel;
using Core.Common;
using System.ServiceModel;

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ToursViewModel : ViewModelBase
    {
        private IServiceFactory _serviceFactory;
        private ObservableCollection<Tour> _trips;
        private EditTripViewModel _editTripViewModel;

        [ImportingConstructor]
        public ToursViewModel(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
            EditTripCommand = new DelegateCommand<Tour>(OnEditCommand);
            DeleteTripCommand = new DelegateCommand<Tour>(OnDeleteCommand);
            AddTripCommand = new DelegateCommand<object>(OnAddCommand);
        }

        [Import]
        public EditTripViewModel EditTripViewModel { get; set; }

        public override string ViewTitle
        {
            get { return "Tours"; }
        }

        public ObservableCollection<Tour> Trips
        {
            get
            {
                return _trips;
            }
            set
            {
                if (value != _trips)
                {
                    _trips = value;
                    OnPropertyChanged(() => Trips, false);
                }
            }
        }

        public EditTripViewModel CurrentTripViewModel
        {
            get { return _editTripViewModel; }
            set
            {
                if (_editTripViewModel != value)
                {
                    _editTripViewModel = value;
                    OnPropertyChanged(() => CurrentTripViewModel, false);
                }
            }
        }

        public DelegateCommand<Tour> EditTripCommand { get; private set; }
        public DelegateCommand<object> AddTripCommand { get; private set; }
        public DelegateCommand<Tour> DeleteTripCommand { get; private set; }

        public event CancelEventHandler ConfirmDelete;
        public event EventHandler<ErrorMessageEventArgs> ErrorOccured;

        protected override void OnViewLoaded()
        {
            _trips = new ObservableCollection<Tour>();
            //ITripService
            WithClient<ITourService>(_serviceFactory.CreateClient<ITourService>(), tripClient =>
            {
                Tour[] trips = tripClient.GetAllTours();
                if (trips != null)
                {
                    foreach (Tour trip in trips)
                    {
                        _trips.Add(trip);
                    }
                }
            });
        }

        private void OnAddCommand(object arg)
        {
            EditTripViewModel.Trip = new Tour();
            EditTripViewModel.TripUpdated += CurrentTripViewModel_TripUpdated;
            EditTripViewModel.TripCancelled += CurrentTripViewModel_TripCancelled;
            CurrentTripViewModel = EditTripViewModel;
        }

        private void OnDeleteCommand(Tour trip)
        {
            //Below is the argument that is going to be carried by the CancelEventHandler
            var args = new CancelEventArgs();
            //Rasie the event
            if (ConfirmDelete != null)
                ConfirmDelete(this, args);
            //If the cancel property of args come back with false then continue with the deletion
            if (!args.Cancel)
            {
                //TODO : Add it to all methods
                try
                {
                    //ITripService
                    WithClient<ITourService>(_serviceFactory.CreateClient<ITourService>(), tripClient =>
                    {
                        tripClient.DeleteTour(trip);
                        _trips.Remove(trip);
                    });
                }
                catch(FaultException ex)
                {
                    if (ErrorOccured != null)
                        ErrorOccured(this, new ErrorMessageEventArgs(ex.Message));
                }
                catch(Exception ex)
                {
                    if (ErrorOccured != null)
                        ErrorOccured(this, new ErrorMessageEventArgs(ex.Message));
                }
            }
        }

        private void OnEditCommand(Tour trip)
        {
            EditTripViewModel.Trip = trip;
            EditTripViewModel.TripUpdated += CurrentTripViewModel_TripUpdated;
            EditTripViewModel.TripCancelled += CurrentTripViewModel_TripCancelled;
            CurrentTripViewModel = EditTripViewModel;
        }

        private void CurrentTripViewModel_TripUpdated(object sender, TripEventArgs e)
        {
            if (!e.IsNew)
            {
                //This is done in order to update the Grid. Remember that in EditTripViewModel the updated trip
                //Is a temporary object and it is not part of the Grid collection trips.
                Tour trip = Trips.FirstOrDefault(item => item.TourId == e.Trip.TourId);
                if (trip != null)
                {
                    trip.TourId = e.Trip.TourId;
                    //trip.TourDescription = e.Trip.TourDescription;
                    trip.StartDate = e.Trip.StartDate;
                    trip.EndDate = e.Trip.EndDate;
                    trip.TourTypeId = e.Trip.TourTypeId;
                    //trip.HotelId = e.Trip.HotelId;
                    //trip.Hotel = e.Trip.Hotel;
                    trip.CollectionAddress = e.Trip.CollectionAddress;
                    //trip.PricePerAdult = e.Trip.PricePerAdult;
                    //trip.PricePerChild = e.Trip.PricePerChild;
                }
            }
            else
            {
                //var tw = new TourWrapper();
                //var trip = new Tour();
                //tw.UpdateTour(e.Trip, trip);
                //Trips.Add(trip);
            }

            CurrentTripViewModel = null;
        }



        private void CurrentTripViewModel_TripCancelled(object sender, EventArgs e)
        {
            CurrentTripViewModel = null;
        }
    }
}
