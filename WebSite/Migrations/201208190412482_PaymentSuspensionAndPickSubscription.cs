namespace WebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PaymentSuspensionAndPickSubscription : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PickUsers",
                c => new
                    {
                        Pick_PickId = c.Int(nullable: false),
                        User_UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Pick_PickId, t.User_UserId })
                .ForeignKey("dbo.Picks", t => t.Pick_PickId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.User_UserId, cascadeDelete: true)
                .Index(t => t.Pick_PickId)
                .Index(t => t.User_UserId);
            
            AddColumn("dbo.Subscriptions", "IsSuspended", c => c.Boolean(nullable: false, defaultValue: false));
        }
        
        public override void Down()
        {
            DropIndex("dbo.PickUsers", new[] { "User_UserId" });
            DropIndex("dbo.PickUsers", new[] { "Pick_PickId" });
            DropForeignKey("dbo.PickUsers", "User_UserId", "dbo.Users");
            DropForeignKey("dbo.PickUsers", "Pick_PickId", "dbo.Picks");
            DropColumn("dbo.Subscriptions", "IsSuspended");
            DropTable("dbo.PickUsers");
        }
    }
}
