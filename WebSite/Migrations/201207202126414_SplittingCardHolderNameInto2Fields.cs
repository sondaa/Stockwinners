namespace WebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class SplittingCardHolderNameInto2Fields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CreditCards", "CardholderFirstName", c => c.String(nullable: false, maxLength: 50));
            AddColumn("dbo.CreditCards", "CardholderLastName", c => c.String(nullable: false, maxLength: 50));
            DropColumn("dbo.CreditCards", "CardholderName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.CreditCards", "CardholderName", c => c.String(nullable: false, maxLength: 100));
            DropColumn("dbo.CreditCards", "CardholderLastName");
            DropColumn("dbo.CreditCards", "CardholderFirstName");
        }
    }
}
