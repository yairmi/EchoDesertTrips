namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_numberofcustomers_to_reservation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservation", "NumberOfCustomers", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reservation", "NumberOfCustomers");
        }
    }
}
