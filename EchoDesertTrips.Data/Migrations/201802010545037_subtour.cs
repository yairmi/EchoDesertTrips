namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class subtour : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SubTour",
                c => new
                    {
                        SubTourId = c.Int(nullable: false, identity: true),
                        DestinationName = c.String(),
                        Private = c.Boolean(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        Tour_TourId = c.Int(),
                    })
                .PrimaryKey(t => t.SubTourId)
                .ForeignKey("dbo.Tour", t => t.Tour_TourId)
                .Index(t => t.Tour_TourId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SubTour", "Tour_TourId", "dbo.Tour");
            DropIndex("dbo.SubTour", new[] { "Tour_TourId" });
            DropTable("dbo.SubTour");
        }
    }
}
