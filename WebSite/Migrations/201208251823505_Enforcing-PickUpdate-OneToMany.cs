namespace WebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EnforcingPickUpdateOneToMany : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PickUpdates", "Pick_PickId", "dbo.Picks");
            DropIndex("dbo.PickUpdates", new[] { "Pick_PickId" });
            AddColumn("dbo.PickUpdates", "PickId", c => c.Int(nullable: false));
            AddForeignKey("dbo.PickUpdates", "PickId", "dbo.Picks", "PickId", cascadeDelete: true);
            CreateIndex("dbo.PickUpdates", "PickId");
            DropColumn("dbo.PickUpdates", "Pick_PickId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.PickUpdates", "Pick_PickId", c => c.Int());
            DropIndex("dbo.PickUpdates", new[] { "PickId" });
            DropForeignKey("dbo.PickUpdates", "PickId", "dbo.Picks");
            DropColumn("dbo.PickUpdates", "PickId");
            CreateIndex("dbo.PickUpdates", "Pick_PickId");
            AddForeignKey("dbo.PickUpdates", "Pick_PickId", "dbo.Picks", "PickId");
        }
    }
}
