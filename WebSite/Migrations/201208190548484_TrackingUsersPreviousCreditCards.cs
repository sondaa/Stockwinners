namespace WebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TrackingUsersPreviousCreditCards : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.CreditCards", "User_UserId", c => c.Int());
            AddForeignKey("dbo.CreditCards", "User_UserId", "dbo.Users", "UserId");
            CreateIndex("dbo.CreditCards", "User_UserId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.CreditCards", new[] { "User_UserId" });
            DropForeignKey("dbo.CreditCards", "User_UserId", "dbo.Users");
            DropColumn("dbo.CreditCards", "User_UserId");
        }
    }
}
