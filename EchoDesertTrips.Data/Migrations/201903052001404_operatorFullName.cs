namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class operatorFullName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Operator", "OperatorFullName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Operator", "OperatorFullName");
        }
    }
}
