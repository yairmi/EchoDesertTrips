namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newfieldtotalprice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservation", "TotalPrice", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reservation", "TotalPrice");
        }
    }
}
