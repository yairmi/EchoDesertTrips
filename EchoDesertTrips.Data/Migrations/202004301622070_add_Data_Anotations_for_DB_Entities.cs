namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_Data_Anotations_for_DB_Entities : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TourHotel", "HotelId", "dbo.Hotel");
            DropIndex("dbo.TourHotel", new[] { "HotelId" });
            AlterColumn("dbo.Agency", "AgencyName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Agency", "AgencyAddress", c => c.String(maxLength: 50));
            AlterColumn("dbo.Agency", "Phone1", c => c.String(nullable: false, maxLength: 30, unicode: false));
            AlterColumn("dbo.Agency", "Phone2", c => c.String(maxLength: 30, unicode: false));
            AlterColumn("dbo.Agent", "FirstName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Agent", "LastName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Agent", "Phone1", c => c.String(nullable: false, maxLength: 30, unicode: false));
            AlterColumn("dbo.Agent", "Phone2", c => c.String(maxLength: 30, unicode: false));
            AlterColumn("dbo.Customer", "IdentityId", c => c.String(maxLength: 10, unicode: false));
            AlterColumn("dbo.Customer", "FirstName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Customer", "LastName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Customer", "Phone1", c => c.String(nullable: false, maxLength: 30, unicode: false));
            AlterColumn("dbo.Customer", "Phone2", c => c.String(maxLength: 30, unicode: false));
            AlterColumn("dbo.Customer", "PassportNumber", c => c.String(maxLength: 30, unicode: false));
            AlterColumn("dbo.Customer", "Nationality", c => c.String(maxLength: 10));
            AlterColumn("dbo.Group", "ExternalId", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.RoomType", "RoomTypeName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Hotel", "HotelName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Hotel", "HotelAddress", c => c.String(maxLength: 100));
            AlterColumn("dbo.Operator", "OperatorName", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Operator", "Password", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Operator", "OperatorFullName", c => c.String(maxLength: 100));
            AlterColumn("dbo.Optional", "OptionalDescription", c => c.String(nullable: false, maxLength: 50));
            AlterColumn("dbo.Reservation", "Comments", c => c.String(maxLength: 500));
            AlterColumn("dbo.Reservation", "Messages", c => c.String(maxLength: 500));
            AlterColumn("dbo.Reservation", "Car", c => c.String(maxLength: 100));
            AlterColumn("dbo.Reservation", "Guide", c => c.String(maxLength: 100));
            AlterColumn("dbo.Reservation", "EndIn", c => c.String(maxLength: 100));
            AlterColumn("dbo.Tour", "PickupAddress", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.Tour", "TourTypePrice", c => c.String(maxLength: 20, unicode: false));
            AlterColumn("dbo.SubTour", "DestinationName", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.TourHotel", "HotelId", c => c.Int(nullable: false));
            AlterColumn("dbo.TourType", "TourTypeName", c => c.String(nullable: false, maxLength: 100));
            AlterColumn("dbo.TourType", "AdultPrices", c => c.String(maxLength: 300, unicode: false));
            AlterColumn("dbo.TourType", "ChildPrices", c => c.String(maxLength: 300, unicode: false));
            AlterColumn("dbo.TourType", "InfantPrices", c => c.String(maxLength: 300, unicode: false));
            AlterColumn("dbo.TourType", "Destinations", c => c.String(maxLength: 300));
            AlterColumn("dbo.TourTypeDescription", "Description", c => c.String(maxLength: 500));
            CreateIndex("dbo.TourHotel", "HotelId");
            AddForeignKey("dbo.TourHotel", "HotelId", "dbo.Hotel", "HotelId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TourHotel", "HotelId", "dbo.Hotel");
            DropIndex("dbo.TourHotel", new[] { "HotelId" });
            AlterColumn("dbo.TourTypeDescription", "Description", c => c.String());
            AlterColumn("dbo.TourType", "Destinations", c => c.String());
            AlterColumn("dbo.TourType", "InfantPrices", c => c.String());
            AlterColumn("dbo.TourType", "ChildPrices", c => c.String());
            AlterColumn("dbo.TourType", "AdultPrices", c => c.String());
            AlterColumn("dbo.TourType", "TourTypeName", c => c.String());
            AlterColumn("dbo.TourHotel", "HotelId", c => c.Int());
            AlterColumn("dbo.SubTour", "DestinationName", c => c.String());
            AlterColumn("dbo.Tour", "TourTypePrice", c => c.String());
            AlterColumn("dbo.Tour", "PickupAddress", c => c.String());
            AlterColumn("dbo.Reservation", "EndIn", c => c.String());
            AlterColumn("dbo.Reservation", "Guide", c => c.String());
            AlterColumn("dbo.Reservation", "Car", c => c.String());
            AlterColumn("dbo.Reservation", "Messages", c => c.String());
            AlterColumn("dbo.Reservation", "Comments", c => c.String());
            AlterColumn("dbo.Optional", "OptionalDescription", c => c.String());
            AlterColumn("dbo.Operator", "OperatorFullName", c => c.String());
            AlterColumn("dbo.Operator", "Password", c => c.String());
            AlterColumn("dbo.Operator", "OperatorName", c => c.String());
            AlterColumn("dbo.Hotel", "HotelAddress", c => c.String());
            AlterColumn("dbo.Hotel", "HotelName", c => c.String());
            AlterColumn("dbo.RoomType", "RoomTypeName", c => c.String());
            AlterColumn("dbo.Group", "ExternalId", c => c.String());
            AlterColumn("dbo.Customer", "Nationality", c => c.String());
            AlterColumn("dbo.Customer", "PassportNumber", c => c.String());
            AlterColumn("dbo.Customer", "Phone2", c => c.String());
            AlterColumn("dbo.Customer", "Phone1", c => c.String());
            AlterColumn("dbo.Customer", "LastName", c => c.String());
            AlterColumn("dbo.Customer", "FirstName", c => c.String());
            AlterColumn("dbo.Customer", "IdentityId", c => c.String());
            AlterColumn("dbo.Agent", "Phone2", c => c.String());
            AlterColumn("dbo.Agent", "Phone1", c => c.String());
            AlterColumn("dbo.Agent", "LastName", c => c.String());
            AlterColumn("dbo.Agent", "FirstName", c => c.String());
            AlterColumn("dbo.Agency", "Phone2", c => c.String());
            AlterColumn("dbo.Agency", "Phone1", c => c.String());
            AlterColumn("dbo.Agency", "AgencyAddress", c => c.String());
            AlterColumn("dbo.Agency", "AgencyName", c => c.String());
            CreateIndex("dbo.TourHotel", "HotelId");
            AddForeignKey("dbo.TourHotel", "HotelId", "dbo.Hotel", "HotelId");
        }
    }
}
