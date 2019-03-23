using Core.Common.UI.Core;
using System.ComponentModel.Composition;

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class MainTabViewModel : ViewModelBase
    {
        [ImportingConstructor]
        public MainTabViewModel()
        {
        }

        [Import]
        public DashboardViewModel DashboardViewModel { get; private set; }
        [Import]
        public ReservationsViewModel ReservationsViewModel { get; set; }
        [Import]
        public AdminViewModel AdminViewModel { get; set; }

        protected override void OnViewLoaded()
        {
        }

        public bool AdminTabVisible
        {
            get
            {
                return CurrentOperator.Operator.Admin;
            }
        }
    }
}
