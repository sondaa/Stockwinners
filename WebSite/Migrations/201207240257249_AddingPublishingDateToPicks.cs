namespace WebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingPublishingDateToPicks : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Picks", "PublishingDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Picks", "PublishingDate");
        }
    }
}
