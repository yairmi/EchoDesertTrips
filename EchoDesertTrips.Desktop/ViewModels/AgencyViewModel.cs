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
            //Agency = agency;
            //Agent = agent;
        }

        //public DelegateCommand<Agency> SelectionAgencyChangedCommand { get; private set; }
        //public DelegateCommand<Agent> SelectionAgentChangedCommand { get; set; }
        //public event EventHandler<AgencyEventArgs> AgencyChanged;

        /////Try Start
                //public Agency Agency { get; private set; }
                //public Agent Agent { get; private set; }

                public ReservationWrapper Reservation { get; set; }

                protected override void OnViewLoaded()
                {
                    if (Reservation.Agency != null && Reservation.Agent != null)
                    {
                        Reservation.Agency = Agencies.FirstOrDefault(n => n.AgencyId == Reservation.Agency.AgencyId);
                        Reservation.Agent = Reservation.Agency.Agents.FirstOrDefault(n => n.AgentId == Reservation.Agent.AgentId);
                    }
                    CleanAll();
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
        /////Try End

        /*        public Agency Agency { get; private set; }
                public Agent Agent { get; private set; }

                private Agency _selectedAgency;

                public Agency SelectedAgency
                {
                    get
                    {
                        return _selectedAgency;
                    }
                    set
                    {
                        if (value != _selectedAgency)
                        {
                            _selectedAgency = value;
                            if (_bFristTime == false)
                                //Send notification only after user interaction. This will set the "Save" button to enable. 
                                //For the first time (View is loaded) we don't want to enable the "Save" button 
                                AgencyChanged?.Invoke(this, new AgencyEventArgs(SelectedAgency, SelectedAgent, false));
                            OnPropertyChanged(() => SelectedAgency, false);
                        }
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
                        if (value != _selectedAgent)
                        {
                            _selectedAgent = value;
                            if (_bFristTime == false)
                                //Send notification only after user interaction. This will set the "Save" button to enable. 
                                //For the first time (View is loaded) we don't want to enable the "Save" button 
                                AgencyChanged?.Invoke(this, new AgencyEventArgs(SelectedAgency, SelectedAgent, false));
                            OnPropertyChanged(() => SelectedAgent, false);
                        }
                    }
                }

                private bool _isEnabled;

                public bool IsEnabled
                {
                    get { return _isEnabled || _selectedAgency != null; }
                    set
                    {
                        _isEnabled = value;
                        if (_isEnabled == false)
                        {
                            SelectedAgency = null;
                            SelectedAgent = null;
                        }
                        OnPropertyChanged(() => IsEnabled, false);
                    }
                }

                protected override void OnViewLoaded()
                {
                    _bFristTime = true;
                    if (Agency != null && Agent != null)
                    {
                        SelectedAgency = Agencies.FirstOrDefault(n => n.AgencyId == Agency.AgencyId);
                        SelectedAgent = SelectedAgency.Agents.FirstOrDefault(n => n.AgentId == Agent.AgentId);
                    }
                    _bFristTime = false; 

                    CleanAll();
                }

                private bool _bFristTime;
            }*/
    }
