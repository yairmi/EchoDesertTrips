namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TourTypeDestination", "TourDestinationId", "dbo.TourDestination");
            DropIndex("dbo.TourTypeDestination", new[] { "TourDestinationId" });
            AddColumn("dbo.TourType", "AdultPrices", c => c.String());
            AddColumn("dbo.TourType", "ChildPrices", c => c.String());
            DropColumn("dbo.TourType", "PricePerChild");
            DropColumn("dbo.TourType", "PricePerAdult");
            DropTable("dbo.TourDestination");
            DropTable("dbo.TourTypeDestination");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TourTypeDestination",
                c => new
                    {
                        TourTypeDestinationId = c.Int(nullable: false, identity: true),
                        TourTypeId = c.Int(nullable: false),
                        TourDestinationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.TourTypeDestinationId);
            
            CreateTable(
                "dbo.TourDestination",
                c => new
                    {
                        TourDestinationId = c.Int(nullable: false, identity: true),
                        TourDestinationName = c.String(),
                        Visible = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.TourDestinationId);
            
            AddColumn("dbo.TourType", "PricePerAdult", c => c.Single(nullable: false));
            AddColumn("dbo.TourType", "PricePerChild", c => c.Single(nullable: false));
            DropColumn("dbo.TourType", "ChildPrices");
            DropColumn("dbo.TourType", "AdultPrices");
            CreateIndex("dbo.TourTypeDestination", "TourDestinationId");
            AddForeignKey("dbo.TourTypeDestination", "TourDestinationId", "dbo.TourDestination", "TourDestinationId", cascadeDelete: true);
        }
    }
}
