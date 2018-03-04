namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTourTypeIncramentGroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TourType", "IncramentExternalId", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TourType", "IncramentExternalId");
        }
    }
}
