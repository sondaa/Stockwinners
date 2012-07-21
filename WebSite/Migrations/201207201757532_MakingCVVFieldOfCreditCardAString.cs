namespace WebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakingCVVFieldOfCreditCardAString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CreditCards", "CVV", c => c.String(maxLength: 4));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CreditCards", "CVV", c => c.Int());
        }
    }
}
