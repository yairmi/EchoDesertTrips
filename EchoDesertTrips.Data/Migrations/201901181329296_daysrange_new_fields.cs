namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class daysrange_new_fields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HotelRoomType", "StartDaysRange", c => c.DateTime(nullable: false));
            AddColumn("dbo.HotelRoomType", "EndDaysRange", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.HotelRoomType", "EndDaysRange");
            DropColumn("dbo.HotelRoomType", "StartDaysRange");
        }
    }
}
