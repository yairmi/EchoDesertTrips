namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTableIndexAndTourTypePricesProperty : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tour", "TourTypePrice", c => c.String());
            AddColumn("dbo.TourHotelRoomType", "PricePerPerson", c => c.Single(nullable: false));
            AddColumn("dbo.TourOptional", "PricePerPerson", c => c.Single(nullable: false));
            AddColumn("dbo.TourOptional", "PriceInclusiveValue", c => c.Single(nullable: false));
            CreateIndex("dbo.Tour", new[] { "StartDate", "EndDate" });
            DropColumn("dbo.TourOptional", "OriginalPricePerPerson");
            DropColumn("dbo.TourOptional", "OriginalPriceInclusive");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TourOptional", "OriginalPriceInclusive", c => c.Single(nullable: false));
            AddColumn("dbo.TourOptional", "OriginalPricePerPerson", c => c.Single(nullable: false));
            DropIndex("dbo.Tour", new[] { "StartDate", "EndDate" });
            DropColumn("dbo.TourOptional", "PriceInclusiveValue");
            DropColumn("dbo.TourOptional", "PricePerPerson");
            DropColumn("dbo.TourHotelRoomType", "PricePerPerson");
            DropColumn("dbo.Tour", "TourTypePrice");
        }
    }
}
