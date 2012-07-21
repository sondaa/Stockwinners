namespace WebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingPhoneNumberToBillingAddress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Addresses", "PhoneNumber", c => c.String(nullable: false, maxLength: 20));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Addresses", "PhoneNumber");
        }
    }
}
