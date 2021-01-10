namespace Collaboration.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_works : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Works", name: "User_Id", newName: "UserId");
            RenameIndex(table: "dbo.Works", name: "IX_User_Id", newName: "IX_UserId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Works", name: "IX_UserId", newName: "IX_User_Id");
            RenameColumn(table: "dbo.Works", name: "UserId", newName: "User_Id");
        }
    }
}
