using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows.Data;
using Core.Common.Contracts;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Contracts;
using EchoDesertTrips.Client.Entities;
using System.Linq;
using System.Windows.Controls;
using EchoDesertTrips.Desktop.ViewModels;
using EchoDesertTrips.Desktop.Support;
using Core.Common.Utils;

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AgentsViewModel : ViewModelBase
    {
        private IServiceFactory _serviceFactory;
        private IMessageDialogService _messageDialogService;

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
            //if (!(value is ReadOnlyObservableCollection<Object>)) return;
            //var items = (ReadOnlyObservableCollection<Object>)value;
            //Agents.Clear();
            //foreach (Agency agency in items)
            //{
            //    foreach (var agent in agency.Agents)
            //    {
            //        Agents.Add(agent);
            //    }
            //}
        }

        //public DelegateCommand<Agency> RowEditEndingAgencyCommand { get; set; }
       // public DelegateCommand<Agency> RowEditEndingAgentCommand { get; set; }

        //private void OnRowEditEndingCommand(Agency agency)
        //{
        //    if (agency.IsDirty)
        //        OnSaveCommand(agency);
        //}

        //private void OnSaveCommand(Agency agency)
        //{
        //    WithClient(_serviceFactory.CreateClient<IInventoryService>(), inventoryClient =>
        //    {
        //        if (agency.AgencyId == 0)
        //        {
        //            agency = inventoryClient.UpdateAgency(agency);
        //        }
        //        else
        //        {
        //            var savedAgency = inventoryClient.UpdateAgency(agency);
        //        }
        //    });
        //}

        //public ICollectionView ItemsView { get; set; }

        public override string ViewTitle => "Agencies & Agents";

        //private ObservableCollection<Agency> _agencies;

        //public ObservableCollection<Agency> Agencies
        //{
        //    get
        //    {
        //        return _agencies;
        //    }

        //    set
        //    {
        //        _agencies = value;
        //        OnPropertyChanged(() => Agencies, false);
        //    }
        //}

        protected override void OnViewLoaded()
        {
            WithClient<IInventoryService>(_serviceFactory.CreateClient<IInventoryService>(), inventoryClient =>
            {
                Agencies.Clear();
                var agencies = inventoryClient.GetAllAgencies();
                foreach (var agency in agencies)
                    Agencies.Add(agency);
                //Agencies = new ObservableCollection<Agency>(agencies);
            });

            //ItemsView = CollectionViewSource.GetDefaultView(Agencies);
            //ItemsView.GroupDescriptions.Add(new PropertyGroupDescription("AgencyName"));
        }

        private void CurrentAgentsViewModel_AgencyUpdated(object sender, AgencyEventArgs e)
        {
            var mappedAgency = AutoMapperUtil.Map<Agency, Agency>(e.Agency);
            if (!e.IsNew)
            {
                //This is done in order to update the Grid. Remember that in EditTripViewModel the updated trip
                //Is a temporary object and it is not part of the Grid collection trips.
                var agency = Agencies.FirstOrDefault(item => item.AgencyId == e.Agency.AgencyId);
                if (agency != null)
                {
                    var index = Agencies.IndexOf(agency);
                    Agencies[index] = mappedAgency;
                }
            }
            else
            {
                Agencies.Add(mappedAgency);
            }

            NotifyServer("CurrentAgentsViewModel_AgencyUpdated", 2);

            //ReservationsView.Refresh();
            //try
            //{
            //    _client.NotifyServer(new EventDataType()
            //    {
            //        ClientName = Operator.OperatorName + "-" + Operator.OperatorId,
            //        EventMessage = "Stam"
            //    });
            //}
            //catch (Exception ex)
            //{
            //    log.Error("CurrentReservationViewModel_ReservationUpdated: Failed to notify server");
            //}
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

    /*public class AgencyValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value,
            System.Globalization.CultureInfo cultureInfo)
        {
            Agency agency = (value as BindingGroup).Items[0] as Agency;
            if (agency.AgencyName == string.Empty)
            {
                return new ValidationResult(false,
                    "Agency name should not be empty");
            }
            else
            {
                var validationResult = ValidationResult.ValidResult;
                return validationResult;
            }
        }
    }*/
}
