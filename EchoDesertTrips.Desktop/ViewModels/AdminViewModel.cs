﻿using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Common.UI.Core;
using EchoDesertTrips.Client.Contracts;
using Core.Common.Contracts;

namespace EchoDesertTrips.Desktop.ViewModels
{
    [Export]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class AdminViewModel : ViewModelBase
    {
        private IServiceFactory _serviceFactory;

        [ImportingConstructor]
        public AdminViewModel(IServiceFactory serviceFactory)
        {
            _serviceFactory = serviceFactory;
        }
        public override string ViewTitle => "Administrator";

        [Import]
        public TourTypeViewModel TourTypeViewModel { get; set; }
        [Import]
        public HotelViewModel HotelViewModel { get; set; }
        [Import]
        public AgentsViewModel AgentsViewModel { get; set; }
        [Import]
        public OptionalsViewModel OptionalsViewModel { get; set; }
        [Import]
        public RoomTypeViewModel RoomTypeViewModel { get; set; }
        [Import]
        public OperatorViewModel OperatorViewModel { get; set; }


        protected override void OnViewLoaded()
        {
            TourTypeViewModel.Client = Client;
            HotelViewModel.Client = Client;
            OptionalsViewModel.Client = Client;
            AgentsViewModel.Client = Client;
            OperatorViewModel.Client = Client;
            RoomTypeViewModel.Client = Client;

            TourTypeViewModel.Operator = Operator;
            HotelViewModel.Operator = Operator;
            OptionalsViewModel.Operator = Operator;
            AgentsViewModel.Operator = Operator;
            OperatorViewModel.Operator = Operator;
            RoomTypeViewModel.Operator = Operator;

            TourTypeViewModel.TourTypes = TourTypes;
            HotelViewModel.Hotels = Hotels;
            OptionalsViewModel.Optionals = Optionals;
            AgentsViewModel.Agencies = Agencies;
            OperatorViewModel.Operators = Operators;

            RoomTypeViewModel.RoomTypes = RoomTypes;
            HotelViewModel.RoomTypes = RoomTypes;
        }
    }
}
