namespace Studentscreeningsystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeperiority : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.RequirementSector", "priority");
        }
        
        public override void Down()
        {
            AddColumn("dbo.RequirementSector", "priority", c => c.Int(nullable: false));
        }
    }
}
