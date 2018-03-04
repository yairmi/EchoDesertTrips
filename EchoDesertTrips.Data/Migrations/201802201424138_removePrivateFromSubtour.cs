namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removePrivateFromSubtour : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.SubTour", "Private");
            DropColumn("dbo.SubTour", "StartDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SubTour", "StartDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.SubTour", "Private", c => c.Boolean(nullable: false));
        }
    }
}
