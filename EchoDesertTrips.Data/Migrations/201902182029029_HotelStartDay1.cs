namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HotelStartDay1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TourHotel", "HotelStartDay", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TourHotel", "HotelStartDay");
        }
    }
}
