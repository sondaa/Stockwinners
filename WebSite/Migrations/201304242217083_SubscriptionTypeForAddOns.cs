namespace WebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SubscriptionTypeForAddOns : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "AutoTradingSubscriptionId", c => c.Int());
            AddColumn("dbo.SubscriptionTypes", "IsAddOn", c => c.Boolean(nullable: false, defaultValue: false));
            AddColumn("dbo.SubscriptionTypes", "Name", c => c.String(nullable: false, defaultValue: ""));
            AddColumn("dbo.SubscriptionTypes", "Description", c => c.String());
            AddForeignKey("dbo.Users", "AutoTradingSubscriptionId", "dbo.Subscriptions", "SubscriptionId");
            CreateIndex("dbo.Users", "AutoTradingSubscriptionId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Users", new[] { "AutoTradingSubscriptionId" });
            DropForeignKey("dbo.Users", "AutoTradingSubscriptionId", "dbo.Subscriptions");
            DropColumn("dbo.SubscriptionTypes", "Description");
            DropColumn("dbo.SubscriptionTypes", "Name");
            DropColumn("dbo.SubscriptionTypes", "IsAddOn");
            DropColumn("dbo.Users", "AutoTradingSubscriptionId");
        }
    }
}
