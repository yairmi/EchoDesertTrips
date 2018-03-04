using System.Windows;
using Core.Common.UI.Core;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace EchoDesertTrips.Desktop.Views
{
    /// <summary>
    /// Interaction logic for TripGridView.xaml
    /// </summary>
    public partial class TourGridView : UserControlViewBase
    {
        public TourGridView()
        {
            InitializeComponent();
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {

            // Do not load your data at design time.
            // if (!System.ComponentModel.DesignerProperties.GetIsInDesignMode(this))
            // {
            // 	//Load your data here and assign the result to the CollectionViewSource.
            // 	System.Windows.Data.CollectionViewSource myCollectionViewSource = (System.Windows.Data.CollectionViewSource)this.Resources["Resource Key for CollectionViewSource"];
            // 	myCollectionViewSource.Source = your data
            // }
        }
    }
}
