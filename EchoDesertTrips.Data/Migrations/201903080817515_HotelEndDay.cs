namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HotelEndDay : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TourHotel", "HotelEndDay", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TourHotel", "HotelEndDay");
        }
    }
}
