namespace Studentscreeningsystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddIdSector : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.USERS", "IdSector", c => c.Int());
            CreateIndex("dbo.USERS", "IdSector");
            AddForeignKey("dbo.USERS", "IdSector", "dbo.Sector", "IdSector");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.USERS", "IdSector", "dbo.Sector");
            DropIndex("dbo.USERS", new[] { "IdSector" });
            DropColumn("dbo.USERS", "IdSector");
        }
    }
}
