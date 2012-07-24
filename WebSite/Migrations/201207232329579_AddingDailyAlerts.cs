namespace WebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingDailyAlerts : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PickFigures", "Pick_PickId", "dbo.Picks");
            DropIndex("dbo.PickFigures", new[] { "Pick_PickId" });
            CreateTable(
                "dbo.DailyAlerts",
                c => new
                    {
                        DailyAlertId = c.Int(nullable: false, identity: true),
                        Content = c.String(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        PublishDate = c.DateTime(),
                        IsPublished = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.DailyAlertId);
            
            CreateTable(
                "dbo.Figures",
                c => new
                    {
                        FigureId = c.Int(nullable: false, identity: true),
                        BlobGuidUri = c.String(nullable: false, maxLength: 36),
                        DailyAlerts_DailyAlertId = c.Int(),
                        Pick_PickId = c.Int(),
                    })
                .PrimaryKey(t => t.FigureId)
                .ForeignKey("dbo.DailyAlerts", t => t.DailyAlerts_DailyAlertId)
                .ForeignKey("dbo.Picks", t => t.Pick_PickId)
                .Index(t => t.DailyAlerts_DailyAlertId)
                .Index(t => t.Pick_PickId);
            
            DropTable("dbo.PickFigures");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PickFigures",
                c => new
                    {
                        PickFigureId = c.Int(nullable: false, identity: true),
                        Caption = c.String(nullable: false, maxLength: 150),
                        BlobGuidUri = c.String(nullable: false, maxLength: 36),
                        Pick_PickId = c.Int(),
                    })
                .PrimaryKey(t => t.PickFigureId);
            
            DropIndex("dbo.Figures", new[] { "Pick_PickId" });
            DropIndex("dbo.Figures", new[] { "DailyAlerts_DailyAlertId" });
            DropForeignKey("dbo.Figures", "Pick_PickId", "dbo.Picks");
            DropForeignKey("dbo.Figures", "DailyAlerts_DailyAlertId", "dbo.DailyAlerts");
            DropTable("dbo.Figures");
            DropTable("dbo.DailyAlerts");
            CreateIndex("dbo.PickFigures", "Pick_PickId");
            AddForeignKey("dbo.PickFigures", "Pick_PickId", "dbo.Picks", "PickId");
        }
    }
}
