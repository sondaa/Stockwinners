namespace WebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialDatabase : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        UserId = c.Int(nullable: false, identity: true),
                        IdentityProvider = c.Int(nullable: false),
                        IdentityProviderIssuedUserId = c.String(nullable: false, maxLength: 50),
                        EmailAddress = c.String(nullable: false, maxLength: 100),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        LastName = c.String(nullable: false, maxLength: 50),
                        IsBanned = c.Boolean(nullable: false),
                        SignUpDate = c.DateTime(nullable: false),
                        LastLoginDate = c.DateTime(nullable: false),
                        TrialExpiryDate = c.DateTime(nullable: false),
                        SubscriptionId = c.Int(),
                        SubscriptionExpiryDate = c.DateTime(),
                        NotificationSettingsId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.UserId)
                .ForeignKey("dbo.Subscriptions", t => t.SubscriptionId)
                .ForeignKey("dbo.NotificationSettings", t => t.NotificationSettingsId, cascadeDelete: true)
                .Index(t => t.SubscriptionId)
                .Index(t => t.NotificationSettingsId);
            
            CreateTable(
                "dbo.Subscriptions",
                c => new
                    {
                        SubscriptionId = c.Int(nullable: false, identity: true),
                        SubscriptionTypeId = c.Int(nullable: false),
                        AuthorizeNETSubscriptionId = c.String(nullable: false, maxLength: 100),
                        ActivationDate = c.DateTime(nullable: false),
                        CancellationDate = c.DateTime(),
                        CreditCardId = c.Int(nullable: false),
                        User_UserId = c.Int(),
                    })
                .PrimaryKey(t => t.SubscriptionId)
                .ForeignKey("dbo.SubscriptionTypes", t => t.SubscriptionTypeId, cascadeDelete: true)
                .ForeignKey("dbo.CreditCards", t => t.CreditCardId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.User_UserId)
                .Index(t => t.SubscriptionTypeId)
                .Index(t => t.CreditCardId)
                .Index(t => t.User_UserId);
            
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
                "dbo.CreditCards",
                c => new
                    {
                        CreditCardId = c.Int(nullable: false, identity: true),
                        CardholderFirstName = c.String(nullable: false, maxLength: 50),
                        CardholderLastName = c.String(nullable: false, maxLength: 50),
                        Number = c.String(nullable: false),
                        ExpirationMonth = c.Short(nullable: false),
                        ExpirationYear = c.Short(nullable: false),
                        CVV = c.String(maxLength: 4),
                        AddressId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CreditCardId)
                .ForeignKey("dbo.Addresses", t => t.AddressId, cascadeDelete: true)
                .Index(t => t.AddressId);
            
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        AddressId = c.Int(nullable: false, identity: true),
                        AddressLine1 = c.String(nullable: false, maxLength: 100),
                        AddressLine2 = c.String(maxLength: 100),
                        City = c.String(nullable: false, maxLength: 70),
                        ProvinceOrState = c.String(nullable: false, maxLength: 30),
                        PostalCode = c.String(nullable: false, maxLength: 20),
                        PhoneNumber = c.String(maxLength: 20),
                        CountryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AddressId)
                .ForeignKey("dbo.Countries", t => t.CountryId, cascadeDelete: true)
                .Index(t => t.CountryId);
            
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        CountryId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.CountryId);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        RoleId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 20),
                    })
                .PrimaryKey(t => t.RoleId);
            
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
            
            CreateTable(
                "dbo.StockwinnersMembers",
                c => new
                    {
                        MemberId = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        LastName = c.String(nullable: false, maxLength: 50),
                        EmailAddress = c.String(nullable: false, maxLength: 150),
                        Password = c.String(nullable: false, maxLength: 100),
                        IsLegacyMember = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.MemberId);
            
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
                        DailyAlert_DailyAlertId = c.Int(),
                        Pick_PickId = c.Int(),
                    })
                .PrimaryKey(t => t.FigureId)
                .ForeignKey("dbo.DailyAlerts", t => t.DailyAlert_DailyAlertId)
                .ForeignKey("dbo.Picks", t => t.Pick_PickId)
                .Index(t => t.DailyAlert_DailyAlertId)
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
                        PublishingDate = c.DateTime(),
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
                        ClosingDate = c.DateTime(),
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
                "dbo.RoleUsers",
                c => new
                    {
                        Role_RoleId = c.Int(nullable: false),
                        User_UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Role_RoleId, t.User_UserId })
                .ForeignKey("dbo.Roles", t => t.Role_RoleId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.User_UserId, cascadeDelete: true)
                .Index(t => t.Role_RoleId)
                .Index(t => t.User_UserId);
            
            CreateTable(
                "dbo.OptionPicks",
                c => new
                    {
                        PickId = c.Int(nullable: false),
                        OptionPickTypeId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PickId)
                .ForeignKey("dbo.Picks", t => t.PickId)
                .ForeignKey("dbo.OptionPickTypes", t => t.OptionPickTypeId, cascadeDelete: true)
                .Index(t => t.PickId)
                .Index(t => t.OptionPickTypeId);
            
            CreateTable(
                "dbo.StockPicks",
                c => new
                    {
                        PickId = c.Int(nullable: false),
                        EntryPrice = c.Decimal(nullable: false, storeType: "money"),
                        ExitPrice = c.Decimal(storeType: "money"),
                        StockPickTypeId = c.Int(nullable: false),
                        IsLongPosition = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.PickId)
                .ForeignKey("dbo.Picks", t => t.PickId)
                .ForeignKey("dbo.StockPickTypes", t => t.StockPickTypeId, cascadeDelete: true)
                .Index(t => t.PickId)
                .Index(t => t.StockPickTypeId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.StockPicks", new[] { "StockPickTypeId" });
            DropIndex("dbo.StockPicks", new[] { "PickId" });
            DropIndex("dbo.OptionPicks", new[] { "OptionPickTypeId" });
            DropIndex("dbo.OptionPicks", new[] { "PickId" });
            DropIndex("dbo.RoleUsers", new[] { "User_UserId" });
            DropIndex("dbo.RoleUsers", new[] { "Role_RoleId" });
            DropIndex("dbo.OptionPickLegs", new[] { "OptionPick_PickId" });
            DropIndex("dbo.PickUpdates", new[] { "Pick_PickId" });
            DropIndex("dbo.Figures", new[] { "Pick_PickId" });
            DropIndex("dbo.Figures", new[] { "DailyAlert_DailyAlertId" });
            DropIndex("dbo.Addresses", new[] { "CountryId" });
            DropIndex("dbo.CreditCards", new[] { "AddressId" });
            DropIndex("dbo.SubscriptionTypes", new[] { "SubscriptionFrequencyId" });
            DropIndex("dbo.Subscriptions", new[] { "User_UserId" });
            DropIndex("dbo.Subscriptions", new[] { "CreditCardId" });
            DropIndex("dbo.Subscriptions", new[] { "SubscriptionTypeId" });
            DropIndex("dbo.Users", new[] { "NotificationSettingsId" });
            DropIndex("dbo.Users", new[] { "SubscriptionId" });
            DropForeignKey("dbo.StockPicks", "StockPickTypeId", "dbo.StockPickTypes");
            DropForeignKey("dbo.StockPicks", "PickId", "dbo.Picks");
            DropForeignKey("dbo.OptionPicks", "OptionPickTypeId", "dbo.OptionPickTypes");
            DropForeignKey("dbo.OptionPicks", "PickId", "dbo.Picks");
            DropForeignKey("dbo.RoleUsers", "User_UserId", "dbo.Users");
            DropForeignKey("dbo.RoleUsers", "Role_RoleId", "dbo.Roles");
            DropForeignKey("dbo.OptionPickLegs", "OptionPick_PickId", "dbo.OptionPicks");
            DropForeignKey("dbo.PickUpdates", "Pick_PickId", "dbo.Picks");
            DropForeignKey("dbo.Figures", "Pick_PickId", "dbo.Picks");
            DropForeignKey("dbo.Figures", "DailyAlert_DailyAlertId", "dbo.DailyAlerts");
            DropForeignKey("dbo.Addresses", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.CreditCards", "AddressId", "dbo.Addresses");
            DropForeignKey("dbo.SubscriptionTypes", "SubscriptionFrequencyId", "dbo.SubscriptionFrequencies");
            DropForeignKey("dbo.Subscriptions", "User_UserId", "dbo.Users");
            DropForeignKey("dbo.Subscriptions", "CreditCardId", "dbo.CreditCards");
            DropForeignKey("dbo.Subscriptions", "SubscriptionTypeId", "dbo.SubscriptionTypes");
            DropForeignKey("dbo.Users", "NotificationSettingsId", "dbo.NotificationSettings");
            DropForeignKey("dbo.Users", "SubscriptionId", "dbo.Subscriptions");
            DropTable("dbo.StockPicks");
            DropTable("dbo.OptionPicks");
            DropTable("dbo.RoleUsers");
            DropTable("dbo.StockPickTypes");
            DropTable("dbo.OptionPickTypes");
            DropTable("dbo.OptionPickLegs");
            DropTable("dbo.Picks");
            DropTable("dbo.PickUpdates");
            DropTable("dbo.Figures");
            DropTable("dbo.DailyAlerts");
            DropTable("dbo.StockwinnersMembers");
            DropTable("dbo.NotificationSettings");
            DropTable("dbo.Roles");
            DropTable("dbo.Countries");
            DropTable("dbo.Addresses");
            DropTable("dbo.CreditCards");
            DropTable("dbo.SubscriptionFrequencies");
            DropTable("dbo.SubscriptionTypes");
            DropTable("dbo.Subscriptions");
            DropTable("dbo.Users");
        }
    }
}
