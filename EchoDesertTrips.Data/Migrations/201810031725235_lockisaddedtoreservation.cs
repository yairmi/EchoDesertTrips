namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class lockisaddedtoreservation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservation", "Lock", c => c.Boolean(nullable: false));
            AddColumn("dbo.Reservation", "LockTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reservation", "LockTime");
            DropColumn("dbo.Reservation", "Lock");
        }
    }
}
