namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tourhotelroomtypeid : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.TourHotelRoomType");
            AddColumn("dbo.TourHotelRoomType", "TourHotelRoomTypeId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.TourHotelRoomType", "TourHotelRoomTypeId");
            DropColumn("dbo.TourHotelRoomType", "TourId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TourHotelRoomType", "TourId", c => c.Int(nullable: false));
            DropPrimaryKey("dbo.TourHotelRoomType");
            DropColumn("dbo.TourHotelRoomType", "TourHotelRoomTypeId");
            AddPrimaryKey("dbo.TourHotelRoomType", new[] { "HotelRoomTypeId", "TourId" });
        }
    }
}
