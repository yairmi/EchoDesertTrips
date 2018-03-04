using Core.Common.Core;
using FluentValidation;

namespace EchoDesertTrips.Client.Entities
{
    public class RoomTypeWrapper : ObjectBase
    {
        private int _roomTypeId;

        public int RoomTypeId
        {
            get
            {
                return _roomTypeId;
            }

            set
            {
                if (_roomTypeId != value)
                {
                    _roomTypeId = value;
                    OnPropertyChanged(() => RoomTypeId, true);
                }
            }
        }

        private string _roomTypeName;

        public string RoomTypeName
        {
            get
            {
                return _roomTypeName;
            }

            set
            {
                if (_roomTypeName != value)
                {
                    //bool dirty = _roomTypeName != null;
                    _roomTypeName = value;
                    OnPropertyChanged(() => RoomTypeName, true);
                }
            }
        }

        class RoomTypeValidator : AbstractValidator<RoomTypeWrapper>
        {
            public RoomTypeValidator()
            {
                RuleFor(obj => obj.RoomTypeName).NotEmpty();
            }
        }

        protected override IValidator GetValidator()
        {
            return new RoomTypeValidator();
        }
    }
}
