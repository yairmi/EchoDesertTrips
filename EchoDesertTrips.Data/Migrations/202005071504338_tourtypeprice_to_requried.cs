namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tourtypeprice_to_requried : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tour", "TourTypePrice", c => c.String(nullable: false, maxLength: 900, unicode: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Tour", "TourTypePrice", c => c.String(maxLength: 900, unicode: false));
        }
    }
}
