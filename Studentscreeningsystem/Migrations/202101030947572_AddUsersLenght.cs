namespace Studentscreeningsystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUsersLenght : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.USERS", "Length", c => c.Int(nullable: false));
            AddColumn("dbo.USERS", "Weight", c => c.Int(nullable: false));
            AddColumn("dbo.USERS", "AverageGraduate", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.USERS", "AverageGraduate");
            DropColumn("dbo.USERS", "Weight");
            DropColumn("dbo.USERS", "Length");
        }
    }
}
