namespace WebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddingOptionPickTypesToOptionPicks : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OptionPicks", "OptionPickTypeId", c => c.Int(nullable: false));
            AddForeignKey("dbo.OptionPicks", "OptionPickTypeId", "dbo.OptionPickTypes", "OptionPickTypeId", cascadeDelete: true);
            CreateIndex("dbo.OptionPicks", "OptionPickTypeId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.OptionPicks", new[] { "OptionPickTypeId" });
            DropForeignKey("dbo.OptionPicks", "OptionPickTypeId", "dbo.OptionPickTypes");
            DropColumn("dbo.OptionPicks", "OptionPickTypeId");
        }
    }
}
