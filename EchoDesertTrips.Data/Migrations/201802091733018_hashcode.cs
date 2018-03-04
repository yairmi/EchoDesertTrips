namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hashcode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservation", "GroupID", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reservation", "GroupID");
        }
    }
}
