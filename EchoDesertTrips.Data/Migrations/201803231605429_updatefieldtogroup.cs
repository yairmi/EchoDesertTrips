namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatefieldtogroup : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Group", "Updated", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Group", "Updated");
        }
    }
}
