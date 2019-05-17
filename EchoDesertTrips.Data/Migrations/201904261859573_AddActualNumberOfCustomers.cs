namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddActualNumberOfCustomers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservation", "ActualNumberOfCustomers", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reservation", "ActualNumberOfCustomers");
        }
    }
}
