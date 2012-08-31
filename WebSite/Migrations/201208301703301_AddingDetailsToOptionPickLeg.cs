namespace WebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingDetailsToOptionPickLeg : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OptionPickLegs", "ExpirationYear", c => c.Short(nullable: false, defaultValue: 2012));
            AddColumn("dbo.OptionPickLegs", "StrikePrice", c => c.Decimal(nullable: false, precision: 18, scale: 2, defaultValue: 50));
            DropColumn("dbo.OptionPickLegs", "Name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.OptionPickLegs", "Name", c => c.String(maxLength: 50));
            DropColumn("dbo.OptionPickLegs", "StrikePrice");
            DropColumn("dbo.OptionPickLegs", "ExpirationYear");
        }
    }
}
