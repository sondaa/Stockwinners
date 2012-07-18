namespace WebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakeSubscriptionOptionalForUsers : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Users", "SubscriptionId", "dbo.Subscriptions");
            DropIndex("dbo.Users", new[] { "SubscriptionId" });
            AlterColumn("dbo.Users", "SubscriptionId", c => c.Int());
            AddForeignKey("dbo.Users", "SubscriptionId", "dbo.Subscriptions", "SubscriptionId");
            CreateIndex("dbo.Users", "SubscriptionId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Users", new[] { "SubscriptionId" });
            DropForeignKey("dbo.Users", "SubscriptionId", "dbo.Subscriptions");
            AlterColumn("dbo.Users", "SubscriptionId", c => c.Int(nullable: false));
            CreateIndex("dbo.Users", "SubscriptionId");
            AddForeignKey("dbo.Users", "SubscriptionId", "dbo.Subscriptions", "SubscriptionId", cascadeDelete: true);
        }
    }
}
