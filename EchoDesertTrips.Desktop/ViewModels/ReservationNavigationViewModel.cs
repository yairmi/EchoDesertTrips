using Core.Common.UI.Core;
using EchoDesertTrips.Desktop.CustomEventArgs;
using System.ComponentModel.Composition;

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class ReservationNavigationViewModel : ViewModelBase
    {
        [ImportingConstructor]
        public ReservationNavigationViewModel()
        {

        }

        [Import]
        private ReservationsViewModel _reservationsViewModel { get; set; }
        [Import]
        private EditReservationViewModel _editReservationViewModel { get; set; }

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

        protected override void OnViewLoaded()
        {
            CurrentViewModel = _reservationsViewModel;
        }

        private void RegisterEvents()
        {
            _editReservationViewModel.ReservationUpdated -= _editReservationViewModel_ReservationUpdated;
            _editReservationViewModel.ReservationUpdated += _editReservationViewModel_ReservationUpdated;
            _editReservationViewModel.ReservationCancelled -= _editReservationViewModel_ReservationCancelled;
            _editReservationViewModel.ReservationCancelled += _editReservationViewModel_ReservationCancelled;
        }

        private void _editReservationViewModel_ReservationUpdated(object sender, ReservationEventArgs e)
        {
            _reservationsViewModel.UpdateReservations(e);
            CurrentViewModel = _reservationsViewModel;
        }

        private void _editReservationViewModel_ReservationCancelled(object sender, ReservationEventArgs e)
        {
            CurrentViewModel = _reservationsViewModel;
        }
    }
}
