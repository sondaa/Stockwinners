namespace WebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenamingDailyAlerts : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Figures", "DailyAlerts_DailyAlertId", "dbo.DailyAlerts");
            DropIndex("dbo.Figures", new[] { "DailyAlerts_DailyAlertId" });
            AddColumn("dbo.Figures", "DailyAlert_DailyAlertId", c => c.Int());
            AddForeignKey("dbo.Figures", "DailyAlert_DailyAlertId", "dbo.DailyAlerts", "DailyAlertId");
            CreateIndex("dbo.Figures", "DailyAlert_DailyAlertId");
            DropColumn("dbo.Figures", "DailyAlerts_DailyAlertId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Figures", "DailyAlerts_DailyAlertId", c => c.Int());
            DropIndex("dbo.Figures", new[] { "DailyAlert_DailyAlertId" });
            DropForeignKey("dbo.Figures", "DailyAlert_DailyAlertId", "dbo.DailyAlerts");
            DropColumn("dbo.Figures", "DailyAlert_DailyAlertId");
            CreateIndex("dbo.Figures", "DailyAlerts_DailyAlertId");
            AddForeignKey("dbo.Figures", "DailyAlerts_DailyAlertId", "dbo.DailyAlerts", "DailyAlertId");
        }
    }
}
