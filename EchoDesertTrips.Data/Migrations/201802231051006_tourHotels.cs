namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tourHotels : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TourHotelRoomType", "TourId", "dbo.Tour");
            DropIndex("dbo.TourHotelRoomType", new[] { "TourId" });
            CreateTable(
                "dbo.TourHotel",
                c => new
                    {
                        TourHotelId = c.Int(nullable: false, identity: true),
                        Hotel_HotelId = c.Int(),
                        Tour_TourId = c.Int(),
                    })
                .PrimaryKey(t => t.TourHotelId)
                .ForeignKey("dbo.Hotel", t => t.Hotel_HotelId)
                .ForeignKey("dbo.Tour", t => t.Tour_TourId)
                .Index(t => t.Hotel_HotelId)
                .Index(t => t.Tour_TourId);
            
            AddColumn("dbo.TourHotelRoomType", "TourHotel_TourHotelId", c => c.Int());
            CreateIndex("dbo.TourHotelRoomType", "TourHotel_TourHotelId");
            AddForeignKey("dbo.TourHotelRoomType", "TourHotel_TourHotelId", "dbo.TourHotel", "TourHotelId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.TourHotel", "Tour_TourId", "dbo.Tour");
            DropForeignKey("dbo.TourHotelRoomType", "TourHotel_TourHotelId", "dbo.TourHotel");
            DropForeignKey("dbo.TourHotel", "Hotel_HotelId", "dbo.Hotel");
            DropIndex("dbo.TourHotelRoomType", new[] { "TourHotel_TourHotelId" });
            DropIndex("dbo.TourHotel", new[] { "Tour_TourId" });
            DropIndex("dbo.TourHotel", new[] { "Hotel_HotelId" });
            DropColumn("dbo.TourHotelRoomType", "TourHotel_TourHotelId");
            DropTable("dbo.TourHotel");
            CreateIndex("dbo.TourHotelRoomType", "TourId");
            AddForeignKey("dbo.TourHotelRoomType", "TourId", "dbo.Tour", "TourId", cascadeDelete: true);
        }
    }
}
