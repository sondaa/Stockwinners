namespace WebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingModelsForStockAndOptionPicks : DbMigration
    {
        public override void Up()
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
                .PrimaryKey(t => t.PickFigureId)
                .ForeignKey("dbo.Picks", t => t.Pick_PickId)
                .Index(t => t.Pick_PickId);
            
            CreateTable(
                "dbo.PickUpdates",
                c => new
                    {
                        PickUpdateId = c.Int(nullable: false, identity: true),
                        Description = c.String(nullable: false),
                        IssueDate = c.DateTime(nullable: false),
                        Pick_PickId = c.Int(),
                    })
                .PrimaryKey(t => t.PickUpdateId)
                .ForeignKey("dbo.Picks", t => t.Pick_PickId)
                .Index(t => t.Pick_PickId);
            
            CreateTable(
                "dbo.Picks",
                c => new
                    {
                        PickId = c.Int(nullable: false, identity: true),
                        Symbol = c.String(nullable: false, maxLength: 6),
                        Description = c.String(nullable: false),
                        IsPublished = c.Boolean(nullable: false),
                        CreationDate = c.DateTime(nullable: false),
                        ClosingDate = c.DateTime(),
                    })
                .PrimaryKey(t => t.PickId);
            
            CreateTable(
                "dbo.OptionPickLegs",
                c => new
                    {
                        OptionPickLegId = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 50),
                        EntryPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ExitPrice = c.Decimal(precision: 18, scale: 2),
                        Quantity = c.Int(nullable: false),
                        OptionPick_PickId = c.Int(),
                    })
                .PrimaryKey(t => t.OptionPickLegId)
                .ForeignKey("dbo.OptionPicks", t => t.OptionPick_PickId)
                .Index(t => t.OptionPick_PickId);
            
            CreateTable(
                "dbo.OptionPickTypes",
                c => new
                    {
                        OptionPickTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.OptionPickTypeId);
            
            CreateTable(
                "dbo.StockPickTypes",
                c => new
                    {
                        StockPickTypeId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.StockPickTypeId);
            
            CreateTable(
                "dbo.OptionPicks",
                c => new
                    {
                        PickId = c.Int(nullable: false),
                        OptionPickId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PickId)
                .ForeignKey("dbo.Picks", t => t.PickId)
                .Index(t => t.PickId);
            
            CreateTable(
                "dbo.StockPicks",
                c => new
                    {
                        PickId = c.Int(nullable: false),
                        StockPickId = c.Int(nullable: false),
                        EntryPrice = c.Decimal(nullable: false, storeType: "money"),
                        ExitPrice = c.Decimal(storeType: "money"),
                    })
                .PrimaryKey(t => t.PickId)
                .ForeignKey("dbo.Picks", t => t.PickId)
                .Index(t => t.PickId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.StockPicks", new[] { "PickId" });
            DropIndex("dbo.OptionPicks", new[] { "PickId" });
            DropIndex("dbo.OptionPickLegs", new[] { "OptionPick_PickId" });
            DropIndex("dbo.PickUpdates", new[] { "Pick_PickId" });
            DropIndex("dbo.PickFigures", new[] { "Pick_PickId" });
            DropForeignKey("dbo.StockPicks", "PickId", "dbo.Picks");
            DropForeignKey("dbo.OptionPicks", "PickId", "dbo.Picks");
            DropForeignKey("dbo.OptionPickLegs", "OptionPick_PickId", "dbo.OptionPicks");
            DropForeignKey("dbo.PickUpdates", "Pick_PickId", "dbo.Picks");
            DropForeignKey("dbo.PickFigures", "Pick_PickId", "dbo.Picks");
            DropTable("dbo.StockPicks");
            DropTable("dbo.OptionPicks");
            DropTable("dbo.StockPickTypes");
            DropTable("dbo.OptionPickTypes");
            DropTable("dbo.OptionPickLegs");
            DropTable("dbo.Picks");
            DropTable("dbo.PickUpdates");
            DropTable("dbo.PickFigures");
        }
    }
}
