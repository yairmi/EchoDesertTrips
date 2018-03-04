namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class idfortourhotel : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.TourHotel", name: "Hotel_HotelId", newName: "HotelId");
            RenameIndex(table: "dbo.TourHotel", name: "IX_Hotel_HotelId", newName: "IX_HotelId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.TourHotel", name: "IX_HotelId", newName: "IX_Hotel_HotelId");
            RenameColumn(table: "dbo.TourHotel", name: "HotelId", newName: "Hotel_HotelId");
        }
    }
}
