using Core.Common.Contracts;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Entities;
using System.Linq;

namespace EchoDesertTrips.Desktop.ViewModels
{
    public class AgencyViewModel : ViewModelBase
    {
        private IServiceFactory _serviceFactory;

        public AgencyViewModel(IServiceFactory serviceFactory, ReservationWrapper reservation)
        {
            _serviceFactory = serviceFactory;
            Reservation = reservation;
        }

        public ReservationWrapper Reservation { get; set; }

        protected override void OnViewLoaded()
        {
            if (Reservation.Agency != null && Reservation.Agent != null)
            {
                Reservation.Agency = Agencies.FirstOrDefault(n => n.AgencyId == Reservation.Agency.AgencyId);
                Reservation.Agent = Reservation.Agency.Agents.FirstOrDefault(n => n.AgentId == Reservation.Agent.AgentId);
            }
        }

        private bool _isEnabled;

        public bool IsEnabled
        {
            get { return _isEnabled || Reservation.Agency != null; }
            set
            {
                _isEnabled = value;
                if (_isEnabled == false)
                {
                    Reservation.Agency = null;
                    Reservation.Agent = null;
                }
                OnPropertyChanged(() => IsEnabled, false);
            }
        }
    }
    }
