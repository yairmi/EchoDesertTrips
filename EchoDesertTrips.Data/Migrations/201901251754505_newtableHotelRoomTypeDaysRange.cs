namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newtableHotelRoomTypeDaysRange : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HotelRoomTypeDaysRange",
                c => new
                    {
                        HotelRoomTypeDaysRangeId = c.Int(nullable: false, identity: true),
                        StartDaysRange = c.DateTime(nullable: false),
                        EndDaysRange = c.DateTime(nullable: false),
                        PricePerPerson = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.HotelRoomTypeDaysRangeId);
            
            DropColumn("dbo.HotelRoomType", "StartDaysRange");
            DropColumn("dbo.HotelRoomType", "EndDaysRange");
            DropColumn("dbo.HotelRoomType", "PricePerPerson");
        }
        
        public override void Down()
        {
            AddColumn("dbo.HotelRoomType", "PricePerPerson", c => c.Single(nullable: false));
            AddColumn("dbo.HotelRoomType", "EndDaysRange", c => c.DateTime(nullable: false));
            AddColumn("dbo.HotelRoomType", "StartDaysRange", c => c.DateTime(nullable: false));
            DropTable("dbo.HotelRoomTypeDaysRange");
        }
    }
}
