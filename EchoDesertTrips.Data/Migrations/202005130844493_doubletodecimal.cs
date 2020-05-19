namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class doubletodecimal : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.HotelRoomTypeDaysRange", "PricePerPerson", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Optional", "PricePerPerson", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Optional", "PriceInclusive", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Reservation", "AdvancePayment", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Reservation", "TotalPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.TourHotelRoomType", "PricePerPerson", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.TourOptional", "PricePerPerson", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.TourOptional", "PriceInclusiveValue", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.TourOptional", "PriceInclusiveValue", c => c.Single(nullable: false));
            AlterColumn("dbo.TourOptional", "PricePerPerson", c => c.Single(nullable: false));
            AlterColumn("dbo.TourHotelRoomType", "PricePerPerson", c => c.Single(nullable: false));
            AlterColumn("dbo.Reservation", "TotalPrice", c => c.Double(nullable: false));
            AlterColumn("dbo.Reservation", "AdvancePayment", c => c.Double(nullable: false));
            AlterColumn("dbo.Optional", "PriceInclusive", c => c.Single(nullable: false));
            AlterColumn("dbo.Optional", "PricePerPerson", c => c.Single(nullable: false));
            AlterColumn("dbo.HotelRoomTypeDaysRange", "PricePerPerson", c => c.Single(nullable: false));
        }
    }
}
