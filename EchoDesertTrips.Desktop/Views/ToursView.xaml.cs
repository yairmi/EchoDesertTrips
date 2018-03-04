using System;
using System.Collections.Generic;
using System.Linq;
using Core.Common.UI.Core;
using EchoDesertTrips.Desktop.ViewModels;
using System.ComponentModel;
using System.Windows;


namespace EchoDesertTrips.Desktop.Views
{
    /// <summary>
    /// Interaction logic for TripsView.xaml
    /// </summary>
    public partial class ToursView : UserControlViewBase
    {
        public ToursView()
        {
            InitializeComponent();
        }

        protected override void OnUnwireViewModelEvents(ViewModelBase viewModel)
        {
            ToursViewModel vm = viewModel as ToursViewModel;
            if (vm != null)
            {
                vm.ConfirmDelete -= OnConfirmDelete;
                vm.ErrorOccured -= OnErrorOccured;
            }
        }

        protected override void OnWireViewModelEvents(ViewModelBase viewModel)
        {
            ToursViewModel vm = viewModel as ToursViewModel;
            if (vm != null)
            {
                vm.ConfirmDelete += OnConfirmDelete;
                vm.ErrorOccured += OnErrorOccured;
            }
        }

        void OnConfirmDelete(object sender, CancelEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Are you sure you want to delete this car?", "Confirm Delete",
                                                      MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.No)
                e.Cancel = true;
        }

        void OnErrorOccured(object sender, Core.Common.ErrorMessageEventArgs e)
        {
            MessageBox.Show(e.ErrorMessage, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
