namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class totalPrice : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customer", "EntityId", c => c.Int(nullable: false));
            AddColumn("dbo.Reservation", "TotalPrice", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reservation", "TotalPrice");
            DropColumn("dbo.Customer", "EntityId");
        }
    }
}
