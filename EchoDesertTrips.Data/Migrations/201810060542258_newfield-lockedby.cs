namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newfieldlockedby : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservation", "LockedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reservation", "LockedBy");
        }
    }
}
