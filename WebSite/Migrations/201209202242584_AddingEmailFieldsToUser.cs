namespace WebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingEmailFieldsToUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "SentInactiveReminder", c => c.Boolean(nullable: false, defaultValue: false));
            AddColumn("dbo.Users", "SentFeedbackRequest", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "SentFeedbackRequest");
            DropColumn("dbo.Users", "SentInactiveReminder");
        }
    }
}
