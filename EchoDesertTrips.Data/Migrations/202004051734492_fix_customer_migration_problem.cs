namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fix_customer_migration_problem : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Customer", "EntityId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Customer", "EntityId", c => c.Int(nullable: false));
        }
    }
}
