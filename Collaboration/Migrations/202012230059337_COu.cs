namespace Collaboration.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class COu : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CountriesViewModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Code = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.CountriesViewModels");
        }
    }
}
