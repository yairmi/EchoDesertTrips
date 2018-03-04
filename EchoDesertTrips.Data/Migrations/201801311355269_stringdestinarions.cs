namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class stringdestinarions : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TourTypeDestination", "TourTypeId", "dbo.TourType");
            DropIndex("dbo.TourTypeDestination", new[] { "TourTypeId" });
            AddColumn("dbo.TourType", "Destinations", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TourType", "Destinations");
            CreateIndex("dbo.TourTypeDestination", "TourTypeId");
            AddForeignKey("dbo.TourTypeDestination", "TourTypeId", "dbo.TourType", "TourTypeId", cascadeDelete: true);
        }
    }
}
