﻿using Core.Common.Contracts;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Entities;
using System.ComponentModel.Composition;
using System.Linq;

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AgencyViewModel : ViewModelBase
    {
        private IServiceFactory _serviceFactory;
        private bool bLoaded;
        [ImportingConstructor]
        public AgencyViewModel(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        protected override void OnViewLoaded()
        {
            bLoaded = false;
            SelectedAgency = Reservation.Agency != null ? Agencies.FirstOrDefault(n => n.AgencyId == Reservation.Agency.AgencyId) : null;
            if (SelectedAgency != null)
                SelectedAgent = Reservation.Agent != null ? SelectedAgency.Agents.FirstOrDefault(n => n.AgentId == Reservation.Agent.AgentId) : null;
            bLoaded = true;
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
        private Agency _selectedAgency;
        public Agency SelectedAgency
        {
            get
            {
                return _selectedAgency;
            }
            set
            {
                if ((value != null && Reservation.Agency == null)
                    || (value != null && ((Agency)value).AgencyId != Reservation.Agency.AgencyId)
                    || (value == null && Reservation.Agency != null)
                    || bLoaded == false)
                {
                    if (bLoaded)
                        Reservation.Agency = value;
                    _selectedAgency = value;
                    OnPropertyChanged(() => SelectedAgency);
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
                if ((value != null && Reservation.Agent == null)
                    || (value != null && ((Agent)value).AgentId != Reservation.Agent.AgentId)
                    || (value == null && Reservation.Agent != null)
                    || bLoaded == false)
                {
                    if (bLoaded)
                        Reservation.Agent = value;
                    _selectedAgent = value;
                    OnPropertyChanged(() => SelectedAgent);
                }
            }
        }
    }
    }
