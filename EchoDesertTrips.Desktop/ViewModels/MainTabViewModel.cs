using Core.Common.Contracts;
using Core.Common.UI.Core;
using EchoDesertTrips.Desktop.CustomEventArgs;
using System;
using System.ComponentModel.Composition;

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MainTabViewModel : ViewModelBase
    {
        [ImportingConstructor]
        public MainTabViewModel()
        {
        }

        [Import]
        public DashboardViewModel DashboardViewModel { get; private set; }
        [Import]
        public ReservationsViewModel ReservationsViewModel { get; set; }
        [Import]
        public AdminViewModel AdminViewModel { get; set; }

        protected override void OnViewLoaded()
        {
            ReservationsViewModel.TourTypes = TourTypes;
            ReservationsViewModel.Hotels = Hotels;
            ReservationsViewModel.Optionals = Optionals;
            ReservationsViewModel.Agencies = Agencies;

            AdminViewModel.TourTypes = TourTypes;
            AdminViewModel.Hotels = Hotels;
            AdminViewModel.Optionals = Optionals;
            AdminViewModel.Agencies = Agencies;
        }
    }
}
