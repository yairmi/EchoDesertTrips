namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TourTypePriceMaxKength900 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tour", "TourTypePrice", c => c.String(maxLength: 900, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tour", "TourTypePrice", c => c.String(maxLength: 20, unicode: false));
        }
    }
}
