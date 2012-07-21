namespace WebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingUserSubscriptionExpiryDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "SubscriptionExpiryDate", c => c.DateTime());
            AddColumn("dbo.Subscriptions", "CreditCardId", c => c.Int(nullable: false));
            AddForeignKey("dbo.Subscriptions", "CreditCardId", "dbo.CreditCards", "CreditCardId", cascadeDelete: true);
            CreateIndex("dbo.Subscriptions", "CreditCardId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Subscriptions", new[] { "CreditCardId" });
            DropForeignKey("dbo.Subscriptions", "CreditCardId", "dbo.CreditCards");
            DropColumn("dbo.Subscriptions", "CreditCardId");
            DropColumn("dbo.Users", "SubscriptionExpiryDate");
        }
    }
}
