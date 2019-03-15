using Core.Common.Contracts;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Entities;
using System.ComponentModel.Composition;
using System.Linq;

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class GeneralReservationViewModel : ViewModelBase
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IMessageDialogService _messageDialogService;
        private bool bLoaded;

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

        protected override void OnViewLoaded()
        {
            bLoaded = false;
            AgencyViewModel.Reservation = Reservation;
            SelectedAgency = Reservation.Agency != null ? Inventories.Agencies.FirstOrDefault(n => n.AgencyId == Reservation.Agency.AgencyId) : null;
            if (SelectedAgency != null)
                SelectedAgent = Reservation.Agent != null ? SelectedAgency.Agents.FirstOrDefault(n => n.AgentId == Reservation.Agent.AgentId) : null;
            _isChecked = false;
            bLoaded = true;
        }

        private Agency _selectedAgency;

        public Agency SelectedAgency
        {
            get
            {
                return _selectedAgency;
            }
            set
            {
                _selectedAgency = value;
                if (bLoaded)
                    Reservation.Agency = _selectedAgency;
                OnPropertyChanged(() => SelectedAgency);
            }
        }

        private Agent _selectedAgent;

        public Agent SelectedAgent
        {
            get
            {
                return _selectedAgent;
            }
            set
            {
                _selectedAgent = value;
                if (bLoaded)
                    Reservation.Agent = _selectedAgent;
                OnPropertyChanged(() => SelectedAgent);
            }
        }

        private bool _isChecked;

        public bool IsChecked
        {
            get { return _isChecked || Reservation.Agency != null; }
            set
            {
                _isChecked = value;
                if (_isChecked == false)
                {
                    Reservation.Agency = null;
                    Reservation.Agent = null;
                    Reservation.AgencyId = null;
                    Reservation.AgentId = null;
                }
                OnPropertyChanged(() => IsChecked, false);
            }
        }
    }
}
