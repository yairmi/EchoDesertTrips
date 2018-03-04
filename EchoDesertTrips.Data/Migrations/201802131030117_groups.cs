namespace EchoDesertTrips.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class groups : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Group",
                c => new
                    {
                        GroupId = c.Int(nullable: false, identity: true),
                        ExternalId = c.String(),
                        EntityId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.GroupId);
            
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
            
            AlterColumn("dbo.Reservation", "GroupID", c => c.Int(nullable: false));
            CreateIndex("dbo.Reservation", "GroupID");
            AddForeignKey("dbo.Reservation", "GroupID", "dbo.Group", "GroupId", cascadeDelete: true);
            DropColumn("dbo.TourType", "TourDescription");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TourType", "TourDescription", c => c.String());
            DropForeignKey("dbo.TourTypeDescription", "TourType_TourTypeId", "dbo.TourType");
            DropForeignKey("dbo.Reservation", "GroupID", "dbo.Group");
            DropIndex("dbo.TourTypeDescription", new[] { "TourType_TourTypeId" });
            DropIndex("dbo.Reservation", new[] { "GroupID" });
            AlterColumn("dbo.Reservation", "GroupID", c => c.String());
            DropTable("dbo.TourTypeDescription");
            DropTable("dbo.Group");
        }
    }
}
