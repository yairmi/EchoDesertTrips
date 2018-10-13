namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removelockedby : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Reservation", "LockedBy");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reservation", "LockedBy", c => c.String());
        }
    }
}
