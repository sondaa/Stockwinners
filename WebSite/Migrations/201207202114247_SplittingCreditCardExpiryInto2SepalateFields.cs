namespace WebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SplittingCreditCardExpiryInto2SepalateFields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CreditCards", "ExpirationMonth", c => c.Short(nullable: false));
            AddColumn("dbo.CreditCards", "ExpirationYear", c => c.Short(nullable: false));
            DropColumn("dbo.CreditCards", "Expiry");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CreditCards", "Expiry", c => c.DateTime(nullable: false));
            DropColumn("dbo.CreditCards", "ExpirationYear");
            DropColumn("dbo.CreditCards", "ExpirationMonth");
        }
    }
}
