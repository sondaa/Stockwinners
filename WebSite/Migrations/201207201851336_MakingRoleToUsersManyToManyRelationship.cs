namespace WebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MakingRoleToUsersManyToManyRelationship : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Roles", "User_UserId", "dbo.Users");
            DropIndex("dbo.Roles", new[] { "User_UserId" });
            CreateTable(
                "dbo.RoleUsers",
                c => new
                    {
                        Role_RoleId = c.Int(nullable: false),
                        User_UserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Role_RoleId, t.User_UserId })
                .ForeignKey("dbo.Roles", t => t.Role_RoleId, cascadeDelete: true)
                .ForeignKey("dbo.Users", t => t.User_UserId, cascadeDelete: true)
                .Index(t => t.Role_RoleId)
                .Index(t => t.User_UserId);
            
            DropColumn("dbo.Roles", "User_UserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Roles", "User_UserId", c => c.Int());
            DropIndex("dbo.RoleUsers", new[] { "User_UserId" });
            DropIndex("dbo.RoleUsers", new[] { "Role_RoleId" });
            DropForeignKey("dbo.RoleUsers", "User_UserId", "dbo.Users");
            DropForeignKey("dbo.RoleUsers", "Role_RoleId", "dbo.Roles");
            DropTable("dbo.RoleUsers");
            CreateIndex("dbo.Roles", "User_UserId");
            AddForeignKey("dbo.Roles", "User_UserId", "dbo.Users", "UserId");
        }
    }
}
