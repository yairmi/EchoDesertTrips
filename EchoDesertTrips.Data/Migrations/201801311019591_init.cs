namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Agency",
                c => new
                    {
                        AgencyId = c.Int(nullable: false, identity: true),
                        AgencyName = c.String(),
                        AgencyAddress = c.String(),
                        Phone1 = c.String(),
                        Phone2 = c.String(),
                    })
                .PrimaryKey(t => t.AgencyId);
            
            CreateTable(
                "dbo.Agent",
                c => new
                    {
                        AgentId = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Phone1 = c.String(),
                        Phone2 = c.String(),
                        Agency_AgencyId = c.Int(),
                    })
                .PrimaryKey(t => t.AgentId)
                .ForeignKey("dbo.Agency", t => t.Agency_AgencyId)
                .Index(t => t.Agency_AgencyId);
            
            CreateTable(
                "dbo.Customer",
                c => new
                    {
                        CustomerId = c.Int(nullable: false, identity: true),
                        IdentityId = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Phone1 = c.String(),
                        Phone2 = c.String(),
                        DateOfBirdth = c.DateTime(nullable: false),
                        PassportNumber = c.String(),
                        IssueData = c.DateTime(nullable: false),
                        ExpireyDate = c.DateTime(nullable: false),
                        Nationality = c.String(),
                        HasVisa = c.Boolean(nullable: false),
                        Reservation_ReservationId = c.Int(),
                    })
                .PrimaryKey(t => t.CustomerId)
                .ForeignKey("dbo.Reservation", t => t.Reservation_ReservationId)
                .Index(t => t.Reservation_ReservationId);
            
            CreateTable(
                "dbo.HotelRoomType",
                c => new
                    {
                        HotelRoomTypeId = c.Int(nullable: false, identity: true),
                        HotelId = c.Int(nullable: false),
                        RoomTypeId = c.Int(nullable: false),
                        PricePerPerson = c.Single(nullable: false),
                        Visible = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.HotelRoomTypeId)
                .ForeignKey("dbo.RoomType", t => t.RoomTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Hotel", t => t.HotelId, cascadeDelete: true)
                .Index(t => t.HotelId)
                .Index(t => t.RoomTypeId);
            
            CreateTable(
                "dbo.RoomType",
                c => new
                    {
                        RoomTypeId = c.Int(nullable: false, identity: true),
                        RoomTypeName = c.String(),
                        Visible = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.RoomTypeId);
            
            CreateTable(
                "dbo.Hotel",
                c => new
                    {
                        HotelId = c.Int(nullable: false, identity: true),
                        HotelName = c.String(),
                        HotelAddress = c.String(),
                        Visible = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.HotelId);
            
            CreateTable(
                "dbo.Operator",
                c => new
                    {
                        OperatorId = c.Int(nullable: false, identity: true),
                        OperatorName = c.String(),
                        Password = c.String(),
                        Admin = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.OperatorId);
            
            CreateTable(
                "dbo.Optional",
                c => new
                    {
                        OptionalId = c.Int(nullable: false, identity: true),
                        OptionalDescription = c.String(),
                        PricePerPerson = c.Single(nullable: false),
                        PriceInclusive = c.Single(nullable: false),
                        Visible = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.OptionalId);
            
            CreateTable(
                "dbo.Reservation",
                c => new
                    {
                        ReservationId = c.Int(nullable: false, identity: true),
                        OperatorId = c.Int(),
                        AgencyId = c.Int(),
                        AgentId = c.Int(),
                        AdvancePayment = c.Double(nullable: false),
                        PickUpTime = c.DateTime(nullable: false),
                        Comments = c.String(),
                        Messages = c.String(),
                        CreationTime = c.DateTime(nullable: false),
                        UpdateTime = c.DateTime(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.ReservationId)
                .ForeignKey("dbo.Agency", t => t.AgencyId)
                .ForeignKey("dbo.Agent", t => t.AgentId)
                .ForeignKey("dbo.Operator", t => t.OperatorId)
                .Index(t => t.OperatorId)
                .Index(t => t.AgencyId)
                .Index(t => t.AgentId);
            
            CreateTable(
                "dbo.Tour",
                c => new
                    {
                        TourId = c.Int(nullable: false, identity: true),
                        TourTypeId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        EndDate = c.DateTime(nullable: false),
                        PickupAddress = c.String(),
                        Reservation_ReservationId = c.Int(),
                    })
                .PrimaryKey(t => t.TourId)
                .ForeignKey("dbo.TourType", t => t.TourTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Reservation", t => t.Reservation_ReservationId)
                .Index(t => t.TourTypeId)
                .Index(t => t.Reservation_ReservationId);
            
            CreateTable(
                "dbo.TourHotelRoomType",
                c => new
                    {
                        HotelRoomTypeId = c.Int(nullable: false),
                        TourId = c.Int(nullable: false),
                        Capacity = c.Int(nullable: false),
                        Persons = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.HotelRoomTypeId, t.TourId })
                .ForeignKey("dbo.HotelRoomType", t => t.HotelRoomTypeId, cascadeDelete: true)
                .ForeignKey("dbo.Tour", t => t.TourId, cascadeDelete: true)
                .Index(t => t.HotelRoomTypeId)
                .Index(t => t.TourId);
            
            CreateTable(
                "dbo.TourOptional",
                c => new
                    {
                        TourId = c.Int(nullable: false),
                        OptionalId = c.Int(nullable: false),
                        PriceInclusive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.TourId, t.OptionalId })
                .ForeignKey("dbo.Optional", t => t.OptionalId, cascadeDelete: true)
                .ForeignKey("dbo.Tour", t => t.TourId, cascadeDelete: true)
                .Index(t => t.TourId)
                .Index(t => t.OptionalId);
            
            CreateTable(
                "dbo.TourType",
                c => new
                    {
                        TourTypeId = c.Int(nullable: false, identity: true),
                        TourTypeName = c.String(),
                        PricePerChild = c.Single(nullable: false),
                        PricePerAdult = c.Single(nullable: false),
                        Private = c.Boolean(nullable: false),
                        TourDescription = c.String(),
                        Days = c.Byte(nullable: false),
                        Visible = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.TourTypeId);
            
            CreateTable(
                "dbo.TourTypeDestination",
                c => new
                    {
                        TourTypeId = c.Int(nullable: false),
                        TourDestinationId = c.Int(nullable: false),
                        TourTypeDestinationId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.TourTypeId, t.TourDestinationId })
                .ForeignKey("dbo.TourDestination", t => t.TourDestinationId, cascadeDelete: true)
                .ForeignKey("dbo.TourType", t => t.TourTypeId, cascadeDelete: true)
                .Index(t => t.TourTypeId)
                .Index(t => t.TourDestinationId);
            
            CreateTable(
                "dbo.TourDestination",
                c => new
                    {
                        TourDestinationId = c.Int(nullable: false, identity: true),
                        TourDestinationName = c.String(),
                        Visible = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.TourDestinationId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tour", "Reservation_ReservationId", "dbo.Reservation");
            DropForeignKey("dbo.Tour", "TourTypeId", "dbo.TourType");
            DropForeignKey("dbo.TourTypeDestination", "TourTypeId", "dbo.TourType");
            DropForeignKey("dbo.TourTypeDestination", "TourDestinationId", "dbo.TourDestination");
            DropForeignKey("dbo.TourOptional", "TourId", "dbo.Tour");
            DropForeignKey("dbo.TourOptional", "OptionalId", "dbo.Optional");
            DropForeignKey("dbo.TourHotelRoomType", "TourId", "dbo.Tour");
            DropForeignKey("dbo.TourHotelRoomType", "HotelRoomTypeId", "dbo.HotelRoomType");
            DropForeignKey("dbo.Reservation", "OperatorId", "dbo.Operator");
            DropForeignKey("dbo.Customer", "Reservation_ReservationId", "dbo.Reservation");
            DropForeignKey("dbo.Reservation", "AgentId", "dbo.Agent");
            DropForeignKey("dbo.Reservation", "AgencyId", "dbo.Agency");
            DropForeignKey("dbo.HotelRoomType", "HotelId", "dbo.Hotel");
            DropForeignKey("dbo.HotelRoomType", "RoomTypeId", "dbo.RoomType");
            DropForeignKey("dbo.Agent", "Agency_AgencyId", "dbo.Agency");
            DropIndex("dbo.TourTypeDestination", new[] { "TourDestinationId" });
            DropIndex("dbo.TourTypeDestination", new[] { "TourTypeId" });
            DropIndex("dbo.TourOptional", new[] { "OptionalId" });
            DropIndex("dbo.TourOptional", new[] { "TourId" });
            DropIndex("dbo.TourHotelRoomType", new[] { "TourId" });
            DropIndex("dbo.TourHotelRoomType", new[] { "HotelRoomTypeId" });
            DropIndex("dbo.Tour", new[] { "Reservation_ReservationId" });
            DropIndex("dbo.Tour", new[] { "TourTypeId" });
            DropIndex("dbo.Reservation", new[] { "AgentId" });
            DropIndex("dbo.Reservation", new[] { "AgencyId" });
            DropIndex("dbo.Reservation", new[] { "OperatorId" });
            DropIndex("dbo.HotelRoomType", new[] { "RoomTypeId" });
            DropIndex("dbo.HotelRoomType", new[] { "HotelId" });
            DropIndex("dbo.Customer", new[] { "Reservation_ReservationId" });
            DropIndex("dbo.Agent", new[] { "Agency_AgencyId" });
            DropTable("dbo.TourDestination");
            DropTable("dbo.TourTypeDestination");
            DropTable("dbo.TourType");
            DropTable("dbo.TourOptional");
            DropTable("dbo.TourHotelRoomType");
            DropTable("dbo.Tour");
            DropTable("dbo.Reservation");
            DropTable("dbo.Optional");
            DropTable("dbo.Operator");
            DropTable("dbo.Hotel");
            DropTable("dbo.RoomType");
            DropTable("dbo.HotelRoomType");
            DropTable("dbo.Customer");
            DropTable("dbo.Agent");
            DropTable("dbo.Agency");
        }
    }
}
