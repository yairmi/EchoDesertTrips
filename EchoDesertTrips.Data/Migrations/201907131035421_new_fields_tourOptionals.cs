namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class new_fields_tourOptionals : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TourOptional", "OriginalPricePerPerson", c => c.Single(nullable: false));
            AddColumn("dbo.TourOptional", "OriginalPriceInclusive", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.TourOptional", "OriginalPriceInclusive");
            DropColumn("dbo.TourOptional", "OriginalPricePerPerson");
        }
    }
}
