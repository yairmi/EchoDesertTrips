using Core.Common.UI.Core;
using Core.Common.Core;
using EchoDesertTrips.Desktop.ViewModels;

namespace EchoDesertTrips.Desktop.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : UserControlViewBase
    {
        public MainView() 
        {
            InitializeComponent();
            DataContext = ObjectBase.Container.GetExportedValue<MainViewModel>();
            
        }
    }
}
