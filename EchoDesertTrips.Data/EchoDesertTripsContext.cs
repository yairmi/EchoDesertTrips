using Core.Common.Contracts;
using EchoDesertTrips.Business.Entities;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Runtime.Serialization;

namespace EchoDesertTrips.Data
{
    public class EchoDesertTripsContext : DbContext
    {
        public EchoDesertTripsContext()
            : base("EchoDesertTrips")
        {
            Database.SetInitializer<EchoDesertTripsContext>(null);
            //Configuration.LazyLoadingEnabled = false;
            Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<Agency> AgencySet { get; set; }
        public DbSet<Agent> AgentSet { get; set; }
        public DbSet<Customer> CustomerSet { get; set; }
        public DbSet<Hotel> HotelSet { get; set; }
        public DbSet<Operator> OperatorSet { get; set; }
        public DbSet<Reservation> ReservationSet { get; set; }
        public DbSet<Tour> TourSet { get; set; }
        public DbSet<TourType> TourTypeSet { get; set; }
        public DbSet<Optional> OptionalSet { get; set; }
        public DbSet<TourOptional> TourOptionalSet { get; set; }
        public DbSet<RoomType> RoomTypeSet { get; set; }
        public DbSet<HotelRoomType> HotelRoomTypeSet { get; set; }
        public DbSet<TourHotelRoomType> TourHotelRoomTypesSet { get; set; }
        public DbSet<SubTour> SubTourSet { get; set; }
        public DbSet<TourTypeDescription> TourTypeDescriptionSet { get; set; }
        public DbSet<Group> GroupSet { get; set; }
        public DbSet<TourHotel> TourHotelSet { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Ignore<ExtensionDataObject>();
            modelBuilder.Ignore<IIdentifiableEntity>();

            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            modelBuilder.Ignore<ExtensionDataObject>();
            modelBuilder.Ignore<IIdentifiableEntity>();
            modelBuilder.Entity<Agency>().HasKey<int>(e => e.AgencyId).Ignore(e => e.EntityId);
            modelBuilder.Entity<Agent>().HasKey<int>(e => e.AgentId).Ignore(e => e.EntityId).Ignore(e => e.FullName);
            modelBuilder.Entity<Customer>().HasKey<int>(e => e.CustomerId).Ignore(e => e.EntityId).Ignore(e => e.FullName);
            modelBuilder.Entity<Hotel>().HasKey<int>(e => e.HotelId).Ignore(e => e.EntityId);
            modelBuilder.Entity<Operator>().HasKey<int>(e => e.OperatorId).Ignore(e => e.EntityId);
            modelBuilder.Entity<Reservation>().HasKey<int>(e => e.ReservationId)
                .Ignore(e => e.EntityId)
                .Property(f => f.RowVersion).IsConcurrencyToken();
            modelBuilder.Entity<Tour>().HasKey<int>(e => e.TourId).Ignore(e => e.EntityId);
            modelBuilder.Entity<TourType>().HasKey<int>(e => e.TourTypeId).Ignore(e => e.EntityId);
            modelBuilder.Entity<Optional>().HasKey<int>(e => e.OptionalId).Ignore(e => e.EntityId);
            modelBuilder.Entity<RoomType>().HasKey<int>(e => e.RoomTypeId).Ignore(e => e.EntityId);
            modelBuilder.Entity<SubTour>().HasKey<int>(e => e.SubTourId).Ignore(e => e.EntityId);
            modelBuilder.Entity<TourTypeDescription>().HasKey<int>(e => e.TourTypeDescriptionId).Ignore(e => e.EntityId);
            modelBuilder.Entity<Group>().HasKey<int>(e => e.GroupId).Ignore(e => e.EntityId);
            modelBuilder.Entity<TourHotel>().HasKey<int>(e => e.TourHotelId).Ignore(e => e.EntityId);
            modelBuilder.Entity<TourHotelRoomType>().HasKey<int>(e => e.TourHotelRoomTypeId).Ignore(e => e.EntityId);
            modelBuilder.Entity<HotelRoomTypeDaysRange>().HasKey<int>(e => e.HotelRoomTypeDaysRangeId).Ignore(e => e.EntityId);
            modelBuilder.Entity<TourOptional>().HasKey(q =>
            new
            {
                q.TourId,
                q.OptionalId
            }).Ignore(e => e.EntityId);

            modelBuilder.Entity<HotelRoomType>().HasKey(q =>
            new
            {
                q.HotelRoomTypeId,
                //q.StartDaysRange,
                //q.EndDaysRange
            }).Ignore(e => e.EntityId);
        }
    }
}
