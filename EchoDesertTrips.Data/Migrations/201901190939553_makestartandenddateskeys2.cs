namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class makestartandenddateskeys2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TourHotelRoomType", new[] { "HotelRoomType_HotelRoomTypeId", "HotelRoomType_StartDaysRange", "HotelRoomType_EndDaysRange" }, "dbo.HotelRoomType");
            DropIndex("dbo.TourHotelRoomType", new[] { "HotelRoomType_HotelRoomTypeId", "HotelRoomType_StartDaysRange", "HotelRoomType_EndDaysRange" });
            DropColumn("dbo.TourHotelRoomType", "HotelRoomTypeId");
            RenameColumn(table: "dbo.TourHotelRoomType", name: "HotelRoomType_HotelRoomTypeId", newName: "HotelRoomTypeId");
            DropPrimaryKey("dbo.HotelRoomType");
            AlterColumn("dbo.HotelRoomType", "HotelRoomTypeId", c => c.Int(nullable: false, identity: true));
            AlterColumn("dbo.HotelRoomType", "StartDaysRange", c => c.DateTime());
            AlterColumn("dbo.HotelRoomType", "EndDaysRange", c => c.DateTime());
            AlterColumn("dbo.TourHotelRoomType", "HotelRoomTypeId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.HotelRoomType", "HotelRoomTypeId");
            CreateIndex("dbo.TourHotelRoomType", "HotelRoomTypeId");
            AddForeignKey("dbo.TourHotelRoomType", "HotelRoomTypeId", "dbo.HotelRoomType", "HotelRoomTypeId", cascadeDelete: true);
            DropColumn("dbo.TourHotelRoomType", "HotelRoomType_StartDaysRange");
            DropColumn("dbo.TourHotelRoomType", "HotelRoomType_EndDaysRange");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TourHotelRoomType", "HotelRoomType_EndDaysRange", c => c.DateTime());
            AddColumn("dbo.TourHotelRoomType", "HotelRoomType_StartDaysRange", c => c.DateTime());
            DropForeignKey("dbo.TourHotelRoomType", "HotelRoomTypeId", "dbo.HotelRoomType");
            DropIndex("dbo.TourHotelRoomType", new[] { "HotelRoomTypeId" });
            DropPrimaryKey("dbo.HotelRoomType");
            AlterColumn("dbo.TourHotelRoomType", "HotelRoomTypeId", c => c.Int());
            AlterColumn("dbo.HotelRoomType", "EndDaysRange", c => c.DateTime(nullable: false));
            AlterColumn("dbo.HotelRoomType", "StartDaysRange", c => c.DateTime(nullable: false));
            AlterColumn("dbo.HotelRoomType", "HotelRoomTypeId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.HotelRoomType", new[] { "HotelRoomTypeId", "StartDaysRange", "EndDaysRange" });
            RenameColumn(table: "dbo.TourHotelRoomType", name: "HotelRoomTypeId", newName: "HotelRoomType_HotelRoomTypeId");
            AddColumn("dbo.TourHotelRoomType", "HotelRoomTypeId", c => c.Int(nullable: false));
            CreateIndex("dbo.TourHotelRoomType", new[] { "HotelRoomType_HotelRoomTypeId", "HotelRoomType_StartDaysRange", "HotelRoomType_EndDaysRange" });
            AddForeignKey("dbo.TourHotelRoomType", new[] { "HotelRoomType_HotelRoomTypeId", "HotelRoomType_StartDaysRange", "HotelRoomType_EndDaysRange" }, "dbo.HotelRoomType", new[] { "HotelRoomTypeId", "StartDaysRange", "EndDaysRange" });
        }
    }
}
