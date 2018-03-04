using Core.Common.Contracts;
using Core.Common.UI.Core;
using System.ComponentModel.Composition;

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

        public override string ViewTitle
        {
            get { return "Dashboard"; }
        }

        protected override void OnViewLoaded()
        {
        }
    }
}
