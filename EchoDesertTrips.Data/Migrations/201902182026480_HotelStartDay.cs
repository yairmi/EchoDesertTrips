namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class HotelStartDay : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Reservation", "TotalPrice");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reservation", "TotalPrice", c => c.Double(nullable: false));
        }
    }
}
