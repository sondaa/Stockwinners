namespace WebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingTrackingForTrialExpiryEmails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "SentTrialExpiryEmail", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "SentTrialExpiryEmail");
        }
    }
}
