namespace Collaboration.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addnew : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Countries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Code = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CountryProject",
                c => new
                    {
                        ProjectId = c.Int(nullable: false),
                        CountryId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ProjectId, t.CountryId })
                .ForeignKey("dbo.Projects", t => t.ProjectId, cascadeDelete: true)
                .ForeignKey("dbo.Countries", t => t.CountryId, cascadeDelete: true)
                .Index(t => t.ProjectId)
                .Index(t => t.CountryId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.CountryProject", "CountryId", "dbo.Countries");
            DropForeignKey("dbo.CountryProject", "ProjectId", "dbo.Projects");
            DropIndex("dbo.CountryProject", new[] { "CountryId" });
            DropIndex("dbo.CountryProject", new[] { "ProjectId" });
            DropTable("dbo.CountryProject");
            DropTable("dbo.Countries");
        }
    }
}
