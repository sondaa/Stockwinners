namespace WebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingLongVersusShortStockPicks : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.StockPicks", "IsLongPosition", c => c.Boolean(nullable: false, defaultValue: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.StockPicks", "IsLongPosition");
        }
    }
}
