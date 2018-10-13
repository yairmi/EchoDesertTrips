namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class newfieldlockedbyid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservation", "LockedById", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reservation", "LockedById");
        }
    }
}
