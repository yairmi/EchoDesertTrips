namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class makestartandenddateskeys1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TourHotelRoomType", "HotelRoomTypeId", "dbo.HotelRoomType");
            DropIndex("dbo.TourHotelRoomType", new[] { "HotelRoomTypeId" });
            DropPrimaryKey("dbo.HotelRoomType");
            AddColumn("dbo.TourHotelRoomType", "HotelRoomType_HotelRoomTypeId", c => c.Int());
            AddColumn("dbo.TourHotelRoomType", "HotelRoomType_StartDaysRange", c => c.DateTime());
            AddColumn("dbo.TourHotelRoomType", "HotelRoomType_EndDaysRange", c => c.DateTime());
            AlterColumn("dbo.HotelRoomType", "HotelRoomTypeId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.HotelRoomType", new[] { "HotelRoomTypeId", "StartDaysRange", "EndDaysRange" });
            CreateIndex("dbo.TourHotelRoomType", new[] { "HotelRoomType_HotelRoomTypeId", "HotelRoomType_StartDaysRange", "HotelRoomType_EndDaysRange" });
            AddForeignKey("dbo.TourHotelRoomType", new[] { "HotelRoomType_HotelRoomTypeId", "HotelRoomType_StartDaysRange", "HotelRoomType_EndDaysRange" }, "dbo.HotelRoomType", new[] { "HotelRoomTypeId", "StartDaysRange", "EndDaysRange" });
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TourHotelRoomType", new[] { "HotelRoomType_HotelRoomTypeId", "HotelRoomType_StartDaysRange", "HotelRoomType_EndDaysRange" }, "dbo.HotelRoomType");
            DropIndex("dbo.TourHotelRoomType", new[] { "HotelRoomType_HotelRoomTypeId", "HotelRoomType_StartDaysRange", "HotelRoomType_EndDaysRange" });
            DropPrimaryKey("dbo.HotelRoomType");
            AlterColumn("dbo.HotelRoomType", "HotelRoomTypeId", c => c.Int(nullable: false, identity: true));
            DropColumn("dbo.TourHotelRoomType", "HotelRoomType_EndDaysRange");
            DropColumn("dbo.TourHotelRoomType", "HotelRoomType_StartDaysRange");
            DropColumn("dbo.TourHotelRoomType", "HotelRoomType_HotelRoomTypeId");
            AddPrimaryKey("dbo.HotelRoomType", "HotelRoomTypeId");
            CreateIndex("dbo.TourHotelRoomType", "HotelRoomTypeId");
            AddForeignKey("dbo.TourHotelRoomType", "HotelRoomTypeId", "dbo.HotelRoomType", "HotelRoomTypeId", cascadeDelete: true);
        }
    }
}
