namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newfieldsforcontinualreservation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservation", "Car", c => c.String());
            AddColumn("dbo.Reservation", "Guide", c => c.String());
            AddColumn("dbo.Reservation", "EndIn", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reservation", "EndIn");
            DropColumn("dbo.Reservation", "Guide");
            DropColumn("dbo.Reservation", "Car");
        }
    }
}
