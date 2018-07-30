namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Infants : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservation", "Adults", c => c.Int(nullable: false));
            AddColumn("dbo.Reservation", "Childs", c => c.Int(nullable: false));
            AddColumn("dbo.Reservation", "Infants", c => c.Int(nullable: false));
            AddColumn("dbo.TourType", "InfantPrices", c => c.String());
            DropColumn("dbo.Reservation", "NumberOfCustomers");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reservation", "NumberOfCustomers", c => c.Int(nullable: false));
            DropColumn("dbo.TourType", "InfantPrices");
            DropColumn("dbo.Reservation", "Infants");
            DropColumn("dbo.Reservation", "Childs");
            DropColumn("dbo.Reservation", "Adults");
        }
    }
}
