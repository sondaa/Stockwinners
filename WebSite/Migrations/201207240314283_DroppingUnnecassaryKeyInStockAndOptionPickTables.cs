namespace WebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class DroppingUnnecassaryKeyInStockAndOptionPickTables : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.OptionPicks", "OptionPickId");
            DropColumn("dbo.StockPicks", "StockPickId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.StockPicks", "StockPickId", c => c.Int(nullable: false));
            AddColumn("dbo.OptionPicks", "OptionPickId", c => c.Int(nullable: false));
        }
    }
}
