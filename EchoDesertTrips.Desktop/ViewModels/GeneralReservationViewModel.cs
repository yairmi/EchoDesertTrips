using Core.Common.Contracts;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Entities;

namespace EchoDesertTrips.Desktop.ViewModels
{
    public class GeneralReservationViewModel : ViewModelBase
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IMessageDialogService _messageDialogService;

        public GeneralReservationViewModel(IServiceFactory serviceFactory,
            IMessageDialogService messageBoxDialogService,
            ReservationWrapper reservation)
        {
            _serviceFactory = serviceFactory;
            _messageDialogService = messageBoxDialogService;
            Reservation = reservation;

            AgencyViewModel = new AgencyViewModel(_serviceFactory, Reservation);
        }

        public override string ViewTitle => "General";

        public AgencyViewModel AgencyViewModel { get; set; }

        public DelegateCommand<bool> CheckBoxAgreeChecked { get; }

        private void OnCheckBoxAgreeChecked(bool checkBoxCheked)
        {
            AgencyViewModel.IsEnabled = checkBoxCheked;
        }

        protected override void OnViewLoaded()
        {
#if DEBUG
            log.Debug("EditOrderViewModel OnViewLoaded start");
#endif
            AgencyViewModel.Agencies = Agencies;
#if DEBUG
            log.Debug("EditOrderViewModel OnViewLoaded end");
#endif
        }

        private ReservationWrapper _reservation;

        public ReservationWrapper Reservation
        {
            get
            {
                return _reservation;
            }
            set
            {
                _reservation = value;
                OnPropertyChanged(() => Reservation);
            }
        }
    }
}
