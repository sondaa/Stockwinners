namespace WebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingClosingDateToOptionLegs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OptionPickLegs", "ClosingDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OptionPickLegs", "ClosingDate");
        }
    }
}
