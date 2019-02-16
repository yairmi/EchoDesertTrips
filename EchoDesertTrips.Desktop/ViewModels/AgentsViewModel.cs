using System;
using System.ComponentModel.Composition;
using System.Windows.Data;
using Core.Common.Contracts;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Entities;
using System.Linq;
using Core.Common.Utils;
using EchoDesertTrips.Desktop.CustomEventArgs;
using static Core.Common.Core.Const;

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
            ExpandedCommand = new DelegateCommand<object>(OnExpandedCommand);
            //RowEditEndingAgencyCommand = new DelegateCommand<Agency>(OnRowEditEndingCommand);
            //RowEditEndingAgentCommand = new DelegateCommand<Agency>(OnRowEditEndingCommand);
        }

        private void OnAddAgencyCommand(object obj)
        {
            CurrentAgentsViewModel = new EditAgentsViewModel(_serviceFactory, _messageDialogService,null);
            RegisterEvents();
        }

        private void OnEditAgencyCommand(Agency obj)
        {
            CurrentAgentsViewModel = new EditAgentsViewModel(_serviceFactory, _messageDialogService, obj);
            RegisterEvents();
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

        public DelegateCommand<Hotel> EditHotelCommand { get; private set; }
        public DelegateCommand<object> AddHotelCommand { get; private set; }

        public DelegateCommand<object> ExpandedCommand { get; private set; }

        private void OnExpandedCommand(object value)
        {
        }

        public override string ViewTitle => "Agencies & Agents";

        private void CurrentAgentsViewModel_AgencyUpdated(object sender, AgencyEventArgs e)
        {
            //var mappedAgency = AutoMapperUtil.Map<Agency, Agency>(e.Agency);
            if (!e.IsNew)
            {
                //This is done in order to update the Grid. Remember that in EditTripViewModel the updated trip
                //Is a temporary object and it is not part of the Grid collection trips.
                var agency = Agencies.FirstOrDefault(item => item.AgencyId == e.Agency.AgencyId);
                if (agency != null)
                {
                    var index = Agencies.IndexOf(agency);
                    Agencies[index] = e.Agency;//mappedAgency;
                }
            }
            else
            {
                Agencies.Add(/*mappedAgency*/e.Agency);
            }

            NotifyServer("CurrentAgentsViewModel_AgencyUpdated", 
                SerializeInventoryMessage(eInventoryTypes.E_AGENCY, e.Agency.AgencyId), eMsgTypes.E_INVENTORY);
            CurrentAgentsViewModel = null;
        }

        private void CurrentAgentsViewModel_AgencyCancelled(object sender, AgencyEventArgs e)
        {
            CurrentAgentsViewModel = null;
        }

        private void RegisterEvents()
        {
            CurrentAgentsViewModel.AgencyUpdated -= CurrentAgentsViewModel_AgencyUpdated;
            CurrentAgentsViewModel.AgencyUpdated += CurrentAgentsViewModel_AgencyUpdated;
            CurrentAgentsViewModel.AgencyCancelled -= CurrentAgentsViewModel_AgencyCancelled;
            CurrentAgentsViewModel.AgencyCancelled += CurrentAgentsViewModel_AgencyCancelled;
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
