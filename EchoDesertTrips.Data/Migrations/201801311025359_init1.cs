namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init1 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.TourTypeDestination");
            AlterColumn("dbo.TourTypeDestination", "TourTypeDestinationId", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.TourTypeDestination", "TourTypeDestinationId");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.TourTypeDestination");
            AlterColumn("dbo.TourTypeDestination", "TourTypeDestinationId", c => c.Int(nullable: false));
            AddPrimaryKey("dbo.TourTypeDestination", new[] { "TourTypeId", "TourDestinationId" });
        }
    }
}
