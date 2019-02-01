namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class listtopublic : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HotelRoomTypeDaysRange", "HotelRoomType_HotelRoomTypeId", c => c.Int());
            CreateIndex("dbo.HotelRoomTypeDaysRange", "HotelRoomType_HotelRoomTypeId");
            AddForeignKey("dbo.HotelRoomTypeDaysRange", "HotelRoomType_HotelRoomTypeId", "dbo.HotelRoomType", "HotelRoomTypeId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HotelRoomTypeDaysRange", "HotelRoomType_HotelRoomTypeId", "dbo.HotelRoomType");
            DropIndex("dbo.HotelRoomTypeDaysRange", new[] { "HotelRoomType_HotelRoomTypeId" });
            DropColumn("dbo.HotelRoomTypeDaysRange", "HotelRoomType_HotelRoomTypeId");
        }
    }
}
