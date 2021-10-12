namespace Studentscreeningsystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateUSer : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DetailsRequirementVM",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdRequirement = c.Int(nullable: false),
                        NameRequirement = c.String(),
                        percentage = c.Int(nullable: false),
                        USER_User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.USERS", t => t.USER_User_Id)
                .Index(t => t.USER_User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.DetailsRequirementVM", "USER_User_Id", "dbo.USERS");
            DropIndex("dbo.DetailsRequirementVM", new[] { "USER_User_Id" });
            DropTable("dbo.DetailsRequirementVM");
        }
    }
}
