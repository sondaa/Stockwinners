namespace WebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakingOptionLegClosingDateOptional : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.OptionPickLegs", "ClosingDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.OptionPickLegs", "ClosingDate", c => c.DateTime(nullable: false));
        }
    }
}
