using AutoMapper;
using Core.Common.Core;
using FluentValidation;
using System;

namespace EchoDesertTrips.Client.Entities
{
    public class TourDestination : ObjectBase
    {
        public int TourDestinationId { get; set; }
        private string _tourDestinationName;

        public string TourDestinationName
        {
            get
            {
                return _tourDestinationName;
            }
            set
            {
                if (_tourDestinationName != value)
                {
                    _tourDestinationName = value;
                    OnPropertyChanged(() => TourDestinationName, true);
                }
            }
        }

        //public bool Visible { get; set; }

        class TourDestinationValidator : AbstractValidator<TourDestination>
        {
            public TourDestinationValidator()
            {
                RuleFor(obj => obj.TourDestinationName).NotEmpty();
            }
        }

        protected override IValidator GetValidator()
        {
            return new TourDestinationValidator();
        }
    }

    //public class TourDestinationHelper
    //{
    //    public static TourDestinationWrapper CreateTourDestinationWrapper(TourDestination tourDestination)
    //    {
    //        var config = new MapperConfiguration(cfg => {
    //            cfg.CreateMap<TourDestination, TourDestinationWrapper>();
    //        });

    //        IMapper iMapper = config.CreateMapper();
    //        var tourDestinationWrapper = iMapper.Map<TourDestination, TourDestinationWrapper>(tourDestination);
    //        return tourDestinationWrapper;
    //    }

    //    public static TourDestination CreateTourDestination(TourDestinationWrapper tourDestinationWrapper)
    //    {
    //        var config = new MapperConfiguration(cfg => {
    //            cfg.CreateMap<TourDestinationWrapper, TourDestination>();
    //        });

    //        IMapper iMapper = config.CreateMapper();
    //        var tourDestination = iMapper.Map<TourDestinationWrapper, TourDestination>(tourDestinationWrapper);
    //        return tourDestination;
    //    }
    //}
}
