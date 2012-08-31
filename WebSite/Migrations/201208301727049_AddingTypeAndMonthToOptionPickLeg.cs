namespace WebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingTypeAndMonthToOptionPickLeg : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OptionPickLegs", "Type", c => c.Short(nullable: false, defaultValue: 1));
            AddColumn("dbo.OptionPickLegs", "ExpirationMonth", c => c.Short(nullable: false, defaultValue: 1));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OptionPickLegs", "ExpirationMonth");
            DropColumn("dbo.OptionPickLegs", "Type");
        }
    }
}
