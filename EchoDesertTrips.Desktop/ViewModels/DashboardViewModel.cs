using Core.Common.Contracts;
using Core.Common.UI.Core;
using System.ComponentModel.Composition;
using System;
using EchoDesertTrips.Client.Contracts;
using System.Linq;
using EchoDesertTrips.Client.Entities;
using System.Diagnostics;

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class DashboardViewModel : ViewModelBase
    {
        private IServiceFactory _serviceFactory;
        [ImportingConstructor]
        public DashboardViewModel(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }

        public override string ViewTitle => "Dashboard";

        protected override void OnViewLoaded()
        {
        }
    }
}
