namespace WebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingPickTypeToStockPicks : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StockPicks", "StockPickTypeId", c => c.Int(nullable: false));
            AddForeignKey("dbo.StockPicks", "StockPickTypeId", "dbo.StockPickTypes", "StockPickTypeId", cascadeDelete: true);
            CreateIndex("dbo.StockPicks", "StockPickTypeId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.StockPicks", new[] { "StockPickTypeId" });
            DropForeignKey("dbo.StockPicks", "StockPickTypeId", "dbo.StockPickTypes");
            DropColumn("dbo.StockPicks", "StockPickTypeId");
        }
    }
}
