namespace WebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSubscriptionTypeAndTrackSubscriptionsForUser : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.SubscriptionTypes",
                c => new
                    {
                        SubscriptionTypeId = c.Int(nullable: false, identity: true),
                        Price = c.Decimal(nullable: false, storeType: "money"),
                        SubscriptionFrequencyId = c.Int(nullable: false),
                        IsAvailableToUsers = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.SubscriptionTypeId)
                .ForeignKey("dbo.SubscriptionFrequencies", t => t.SubscriptionFrequencyId, cascadeDelete: true)
                .Index(t => t.SubscriptionFrequencyId);
            
            CreateTable(
                "dbo.SubscriptionFrequencies",
                c => new
                    {
                        SubscriptionFrequencyId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.SubscriptionFrequencyId);
            
            CreateTable(
                "dbo.NotificationSettings",
                c => new
                    {
                        NotificationSettingId = c.Int(nullable: false, identity: true),
                        ReceiveDailyAlerts = c.Boolean(nullable: false),
                        ReceiveOptionPicks = c.Boolean(nullable: false),
                        ReceiveStockPicks = c.Boolean(nullable: false),
                        ReceiveWeeklyAlerts = c.Boolean(nullable: false),
                        ReceiveGeneralAnnouncements = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.NotificationSettingId);
            
            AddColumn("dbo.Users", "NotificationSettingsId", c => c.Int(nullable: false));
            AddColumn("dbo.Subscriptions", "SubscriptionTypeId", c => c.Int(nullable: false));
            AddColumn("dbo.Subscriptions", "AuthorizeNETSubscriptionId", c => c.String(nullable: false, maxLength: 100));
            AddColumn("dbo.Subscriptions", "ActivationDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Subscriptions", "CancellationDate", c => c.DateTime());
            AddColumn("dbo.Subscriptions", "User_UserId", c => c.Int());
            AddColumn("dbo.Addresses", "City", c => c.String(nullable: false, maxLength: 70));
            AddForeignKey("dbo.Users", "NotificationSettingsId", "dbo.NotificationSettings", "NotificationSettingId", cascadeDelete: true);
            AddForeignKey("dbo.Subscriptions", "SubscriptionTypeId", "dbo.SubscriptionTypes", "SubscriptionTypeId", cascadeDelete: true);
            AddForeignKey("dbo.Subscriptions", "User_UserId", "dbo.Users", "UserId");
            CreateIndex("dbo.Users", "NotificationSettingsId");
            CreateIndex("dbo.Subscriptions", "SubscriptionTypeId");
            CreateIndex("dbo.Subscriptions", "User_UserId");
            DropColumn("dbo.Users", "SubscriptionActivationDate");
            DropColumn("dbo.Users", "SubscriptionCancellationDate");
            DropColumn("dbo.Subscriptions", "Price");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Subscriptions", "Price", c => c.Decimal(nullable: false, storeType: "money"));
            AddColumn("dbo.Users", "SubscriptionCancellationDate", c => c.DateTime());
            AddColumn("dbo.Users", "SubscriptionActivationDate", c => c.DateTime());
            DropIndex("dbo.SubscriptionTypes", new[] { "SubscriptionFrequencyId" });
            DropIndex("dbo.Subscriptions", new[] { "User_UserId" });
            DropIndex("dbo.Subscriptions", new[] { "SubscriptionTypeId" });
            DropIndex("dbo.Users", new[] { "NotificationSettingsId" });
            DropForeignKey("dbo.SubscriptionTypes", "SubscriptionFrequencyId", "dbo.SubscriptionFrequencies");
            DropForeignKey("dbo.Subscriptions", "User_UserId", "dbo.Users");
            DropForeignKey("dbo.Subscriptions", "SubscriptionTypeId", "dbo.SubscriptionTypes");
            DropForeignKey("dbo.Users", "NotificationSettingsId", "dbo.NotificationSettings");
            DropColumn("dbo.Addresses", "City");
            DropColumn("dbo.Subscriptions", "User_UserId");
            DropColumn("dbo.Subscriptions", "CancellationDate");
            DropColumn("dbo.Subscriptions", "ActivationDate");
            DropColumn("dbo.Subscriptions", "AuthorizeNETSubscriptionId");
            DropColumn("dbo.Subscriptions", "SubscriptionTypeId");
            DropColumn("dbo.Users", "NotificationSettingsId");
            DropTable("dbo.NotificationSettings");
            DropTable("dbo.SubscriptionFrequencies");
            DropTable("dbo.SubscriptionTypes");
        }
    }
}
