using Core.Common.Contracts;
using Core.Common.Core;
using Core.Common.Utils;
using EchoDesertTrips.Client.Entities;
using System.Linq;

namespace Core.Common.UI.Core
{
    public class InventoriesSingle : ObjectBase
    {
        private static readonly InventoriesSingle INSTANCE = new InventoriesSingle()
        {
            Hotels = new RangeObservableCollection<Hotel>(),
            TourTypes = new RangeObservableCollection<TourType>(),
            Optionals = new RangeObservableCollection<Optional>(),
            RoomTypes = new RangeObservableCollection<RoomType>(),
            Agencies = new RangeObservableCollection<Agency>(),
            Operators = new RangeObservableCollection<Operator>(),
        };

        private InventoriesSingle() {}

        public static InventoriesSingle Instance
        {
            get
            {
                return INSTANCE;
            }
        }

        public RangeObservableCollection<Hotel> Hotels { get; set; }
        public RangeObservableCollection<TourType> TourTypes { get; set; }
        public RangeObservableCollection<Optional> Optionals { get; set; }
        public RangeObservableCollection<RoomType> RoomTypes { get; set; }
        public RangeObservableCollection<Agency> Agencies { get; set; }
        public RangeObservableCollection<Operator> Operators { get; set; }

        public void Update<T>(T entity, RangeObservableCollection<T> rangeObservableCollection) where T: ObjectBase
        {
            var existingT = rangeObservableCollection.FirstOrDefault(t => t.EntityId == entity.EntityId);
            if (existingT != null)
            {
                var index = rangeObservableCollection.IndexOf(existingT);
                rangeObservableCollection[index] = AutoMapperUtil.Map<T, T>(existingT);
            }
            else
            {
                rangeObservableCollection.Add(entity);
            }
        }
    }
}
