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
                "dbo.Group",
                c => new
                    {
                        GroupId = c.Int(nullable: false, identity: true),
                        ExternalId = c.String(),
                    })
                .PrimaryKey(t => t.GroupId);
            
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
                        GroupID = c.Int(nullable: false),
                        CreationTime = c.DateTime(nullable: false),
                        UpdateTime = c.DateTime(nullable: false),
                        RowVersion = c.Binary(nullable: false, fixedLength: true, timestamp: true, storeType: "rowversion"),
                    })
                .PrimaryKey(t => t.ReservationId)
                .ForeignKey("dbo.Agency", t => t.AgencyId)
                .ForeignKey("dbo.Agent", t => t.AgentId)
                .ForeignKey("dbo.Group", t => t.GroupID, cascadeDelete: true)
                .ForeignKey("dbo.Operator", t => t.OperatorId)
                .Index(t => t.OperatorId)
                .Index(t => t.AgencyId)
                .Index(t => t.AgentId)
                .Index(t => t.GroupID);
            
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
                "dbo.SubTour",
                c => new
                    {
                        SubTourId = c.Int(nullable: false, identity: true),
                        DestinationName = c.String(),
                        Tour_TourId = c.Int(),
                    })
                .PrimaryKey(t => t.SubTourId)
                .ForeignKey("dbo.Tour", t => t.Tour_TourId)
                .Index(t => t.Tour_TourId);
            
            CreateTable(
                "dbo.TourHotel",
                c => new
                    {
                        TourHotelId = c.Int(nullable: false, identity: true),
                        HotelId = c.Int(),
                        Tour_TourId = c.Int(),
                    })
                .PrimaryKey(t => t.TourHotelId)
                .ForeignKey("dbo.Hotel", t => t.HotelId)
                .ForeignKey("dbo.Tour", t => t.Tour_TourId)
                .Index(t => t.HotelId)
                .Index(t => t.Tour_TourId);
            
            CreateTable(
                "dbo.TourHotelRoomType",
                c => new
                    {
                        TourHotelRoomTypeId = c.Int(nullable: false, identity: true),
                        HotelRoomTypeId = c.Int(nullable: false),
                        Capacity = c.Int(nullable: false),
                        Persons = c.Int(nullable: false),
                        TourHotel_TourHotelId = c.Int(),
                    })
                .PrimaryKey(t => t.TourHotelRoomTypeId)
                .ForeignKey("dbo.HotelRoomType", t => t.HotelRoomTypeId, cascadeDelete: true)
                .ForeignKey("dbo.TourHotel", t => t.TourHotel_TourHotelId)
                .Index(t => t.HotelRoomTypeId)
                .Index(t => t.TourHotel_TourHotelId);
            
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
                        AdultPrices = c.String(),
                        ChildPrices = c.String(),
                        Destinations = c.String(),
                        Private = c.Boolean(nullable: false),
                        Days = c.Byte(nullable: false),
                        IncramentExternalId = c.Boolean(nullable: false),
                        Visible = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.TourTypeId);
            
            CreateTable(
                "dbo.TourTypeDescription",
                c => new
                    {
                        TourTypeDescriptionId = c.Int(nullable: false, identity: true),
                        Description = c.String(),
                        TourType_TourTypeId = c.Int(),
                    })
                .PrimaryKey(t => t.TourTypeDescriptionId)
                .ForeignKey("dbo.TourType", t => t.TourType_TourTypeId)
                .Index(t => t.TourType_TourTypeId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Tour", "Reservation_ReservationId", "dbo.Reservation");
            DropForeignKey("dbo.Tour", "TourTypeId", "dbo.TourType");
            DropForeignKey("dbo.TourTypeDescription", "TourType_TourTypeId", "dbo.TourType");
            DropForeignKey("dbo.TourOptional", "TourId", "dbo.Tour");
            DropForeignKey("dbo.TourOptional", "OptionalId", "dbo.Optional");
            DropForeignKey("dbo.TourHotel", "Tour_TourId", "dbo.Tour");
            DropForeignKey("dbo.TourHotelRoomType", "TourHotel_TourHotelId", "dbo.TourHotel");
            DropForeignKey("dbo.TourHotelRoomType", "HotelRoomTypeId", "dbo.HotelRoomType");
            DropForeignKey("dbo.TourHotel", "HotelId", "dbo.Hotel");
            DropForeignKey("dbo.SubTour", "Tour_TourId", "dbo.Tour");
            DropForeignKey("dbo.Reservation", "OperatorId", "dbo.Operator");
            DropForeignKey("dbo.Reservation", "GroupID", "dbo.Group");
            DropForeignKey("dbo.Customer", "Reservation_ReservationId", "dbo.Reservation");
            DropForeignKey("dbo.Reservation", "AgentId", "dbo.Agent");
            DropForeignKey("dbo.Reservation", "AgencyId", "dbo.Agency");
            DropForeignKey("dbo.HotelRoomType", "HotelId", "dbo.Hotel");
            DropForeignKey("dbo.HotelRoomType", "RoomTypeId", "dbo.RoomType");
            DropForeignKey("dbo.Agent", "Agency_AgencyId", "dbo.Agency");
            DropIndex("dbo.TourTypeDescription", new[] { "TourType_TourTypeId" });
            DropIndex("dbo.TourOptional", new[] { "OptionalId" });
            DropIndex("dbo.TourOptional", new[] { "TourId" });
            DropIndex("dbo.TourHotelRoomType", new[] { "TourHotel_TourHotelId" });
            DropIndex("dbo.TourHotelRoomType", new[] { "HotelRoomTypeId" });
            DropIndex("dbo.TourHotel", new[] { "Tour_TourId" });
            DropIndex("dbo.TourHotel", new[] { "HotelId" });
            DropIndex("dbo.SubTour", new[] { "Tour_TourId" });
            DropIndex("dbo.Tour", new[] { "Reservation_ReservationId" });
            DropIndex("dbo.Tour", new[] { "TourTypeId" });
            DropIndex("dbo.Reservation", new[] { "GroupID" });
            DropIndex("dbo.Reservation", new[] { "AgentId" });
            DropIndex("dbo.Reservation", new[] { "AgencyId" });
            DropIndex("dbo.Reservation", new[] { "OperatorId" });
            DropIndex("dbo.HotelRoomType", new[] { "RoomTypeId" });
            DropIndex("dbo.HotelRoomType", new[] { "HotelId" });
            DropIndex("dbo.Customer", new[] { "Reservation_ReservationId" });
            DropIndex("dbo.Agent", new[] { "Agency_AgencyId" });
            DropTable("dbo.TourTypeDescription");
            DropTable("dbo.TourType");
            DropTable("dbo.TourOptional");
            DropTable("dbo.TourHotelRoomType");
            DropTable("dbo.TourHotel");
            DropTable("dbo.SubTour");
            DropTable("dbo.Tour");
            DropTable("dbo.Reservation");
            DropTable("dbo.Optional");
            DropTable("dbo.Operator");
            DropTable("dbo.Hotel");
            DropTable("dbo.RoomType");
            DropTable("dbo.HotelRoomType");
            DropTable("dbo.Group");
            DropTable("dbo.Customer");
            DropTable("dbo.Agent");
            DropTable("dbo.Agency");
        }
    }
}
