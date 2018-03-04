namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GroupAdded : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Group", "EntityId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Group", "EntityId", c => c.Int(nullable: false));
        }
    }
}
