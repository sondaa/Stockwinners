namespace WebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakingPhoneNumberAndAddressLine2NotRequired : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Addresses", "AddressLine2", c => c.String(maxLength: 100));
            AlterColumn("dbo.Addresses", "PhoneNumber", c => c.String(maxLength: 20));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Addresses", "PhoneNumber", c => c.String(nullable: false, maxLength: 20));
            AlterColumn("dbo.Addresses", "AddressLine2", c => c.String(nullable: false, maxLength: 100));
        }
    }
}
