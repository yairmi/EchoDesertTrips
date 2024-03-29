﻿using System;
using System.ComponentModel.Composition;
using System.Windows.Data;
using Core.Common.Contracts;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Entities;
using static Core.Common.Core.Const;
using Core.Common.UI.PubSubEvent;
using Core.Common.UI.CustomEventArgs;
using EchoDesertTrips.Client.Contracts;

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AgentsViewModel : ViewModelBase
    {
        private readonly IServiceFactory _serviceFactory;
        private readonly IMessageDialogService _messageDialogService;

        [ImportingConstructor]
        public AgentsViewModel(IServiceFactory serviceFactory,
            IMessageDialogService messageDialogService)
        {
            _serviceFactory = serviceFactory;
            _messageDialogService = messageDialogService;
            EditAgencyCommand = new DelegateCommand<Agency>(OnEditAgencyCommand);
            AddAgencyCommand = new DelegateCommand<object>(OnAddAgencyCommand);
            _eventAggregator.GetEvent<AgencyUpdatedEvent>().Subscribe(AgencyUpdated);
            _eventAggregator.GetEvent<AgencyCancelledEvent>().Subscribe(AgencyCancelled);
        }

        private void OnAddAgencyCommand(object obj)
        {
            CurrentAgentsViewModel = new EditAgentsViewModel(_serviceFactory, _messageDialogService,new Agency());
        }

        private void OnEditAgencyCommand(Agency agency)
        {
            Agency dbAgency = null;
            try
            {
                WithClient(_serviceFactory.CreateClient<IInventoryService>(), agencyClient =>
                {
                    dbAgency = agencyClient.GetAgencyById(agency.AgencyId);

                    if (dbAgency == null)
                    {
                        _messageDialogService.ShowInfoDialog("Could not load Agency,\nTourType was not found.", "Info");
                        return;
                    }

                    CurrentAgentsViewModel = new EditAgentsViewModel(_serviceFactory, _messageDialogService, agency);
                });
            }
            catch(Exception ex)
            {
                log.Error(string.Empty, ex);
            }
        }

        public DelegateCommand<Agency> EditAgencyCommand { get; private set; }
        public DelegateCommand<object> AddAgencyCommand { get; private set; }

        private EditAgentsViewModel _editAgentsViewModel;

        public EditAgentsViewModel CurrentAgentsViewModel
        {
            get { return _editAgentsViewModel; }
            set
            {
                if (_editAgentsViewModel != value)
                {
                    _editAgentsViewModel = value;
                    OnPropertyChanged(() => CurrentAgentsViewModel, false);
                }
            }
        }

        public override string ViewTitle => "Agencies & Agents";

        private void AgencyUpdated(AgencyEventArgs e)
        {
            if (e.Agency != null)
            {
                Inventories.Update<Agency>(e.Agency, Inventories.Agencies);

                if (e.bSendUpdateToClients)
                {
                    try
                    {
                        Client.NotifyServer(
                            SerializeInventoryMessage(eInventoryTypes.E_AGENCY, eOperation.E_UPDATED, e.Agency.AgencyId), eMsgTypes.E_INVENTORY);
                    }
                    catch (Exception ex)
                    {
                        log.Error($"Failed to notify server for Agency update.\n{ex.StackTrace}");
                    }
                }
            }

            if (e.bSendUpdateToClients)
            {
                CurrentAgentsViewModel = null;
            }
        }

        private void AgencyCancelled(AgencyEventArgs obj)
        {
            CurrentAgentsViewModel = null;
        }
    }

    public class AgentsConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            var item = (Agency)value;
            //if (item != null) return item?.Customers.Count * item.Trip.Price;
            return null;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
