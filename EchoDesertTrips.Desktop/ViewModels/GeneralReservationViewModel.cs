using Core.Common.Contracts;
using Core.Common.UI.Core;
using System.ComponentModel.Composition;

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class GeneralReservationViewModel : ViewModelBase
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IMessageDialogService _messageDialogService;
        [ImportingConstructor]
        public GeneralReservationViewModel(IServiceFactory serviceFactory,
            IMessageDialogService messageBoxDialogService)
        {
            _serviceFactory = serviceFactory;
            _messageDialogService = messageBoxDialogService;
        }

        public override string ViewTitle => "General";
        [Import]
        public AgencyViewModel AgencyViewModel { get; set; }

        public DelegateCommand<bool> CheckBoxAgreeChecked { get; }

        private void OnCheckBoxAgreeChecked(bool checkBoxCheked)
        {
            AgencyViewModel.IsEnabled = checkBoxCheked;
        }

        protected override void OnViewLoaded()
        {
            log.Debug("GeneralReservationViewModel OnViewLoaded start");
            AgencyViewModel.Agencies = Agencies;
            AgencyViewModel.Reservation = Reservation;
            AgencyViewModel.SelectedAgency = null;
            AgencyViewModel.SelectedAgent = null;
            log.Debug("GeneralReservationViewModel OnViewLoaded end");
        }
    }
}
