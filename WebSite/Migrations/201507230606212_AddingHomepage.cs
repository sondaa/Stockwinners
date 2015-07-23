namespace WebSite.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class help : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Homepages",
                c => new
                    {
                        HomepageId = c.Int(nullable: false, identity: true),
                        Contents = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.HomepageId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Homepages");
        }
    }
}
