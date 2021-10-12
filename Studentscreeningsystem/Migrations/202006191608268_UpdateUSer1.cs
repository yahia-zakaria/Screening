namespace Studentscreeningsystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdateUSer1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.DetailsRequirementVM", "USER_User_Id", "dbo.USERS");
            DropIndex("dbo.DetailsRequirementVM", new[] { "USER_User_Id" });
            CreateTable(
                "dbo.alldegreeGraduateVM",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdGraduate = c.Int(nullable: false),
                        Firstname = c.String(),
                        Lastname = c.String(),
                        NumGraduate = c.Long(nullable: false),
                        IdentityGraduate = c.Long(nullable: false),
                        lenght = c.Int(nullable: false),
                        weight = c.Int(nullable: false),
                        AverageGraduate = c.Single(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.DetailsLeadershipVM",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IdLeadership = c.Int(nullable: false),
                        NameLeadership = c.String(),
                        IsChecked = c.Boolean(nullable: false),
                        alldegreeGraduateVM_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.alldegreeGraduateVM", t => t.alldegreeGraduateVM_Id)
                .Index(t => t.alldegreeGraduateVM_Id);
            
            AddColumn("dbo.USERS", "RapportGraduate_Id", c => c.Int());
            AddColumn("dbo.DetailsRequirementVM", "alldegreeGraduateVM_Id", c => c.Int());
            AddColumn("dbo.DetailsRequirementVM", "alldegreeGraduateVM_Id1", c => c.Int());
            AddColumn("dbo.DetailsRequirementVM", "alldegreeGraduateVM_Id2", c => c.Int());
            AddColumn("dbo.DetailsRequirementVM", "alldegreeGraduateVM_Id3", c => c.Int());
            CreateIndex("dbo.USERS", "RapportGraduate_Id");
            CreateIndex("dbo.DetailsRequirementVM", "alldegreeGraduateVM_Id");
            CreateIndex("dbo.DetailsRequirementVM", "alldegreeGraduateVM_Id1");
            CreateIndex("dbo.DetailsRequirementVM", "alldegreeGraduateVM_Id2");
            CreateIndex("dbo.DetailsRequirementVM", "alldegreeGraduateVM_Id3");
            AddForeignKey("dbo.DetailsRequirementVM", "alldegreeGraduateVM_Id", "dbo.alldegreeGraduateVM", "Id");
            AddForeignKey("dbo.DetailsRequirementVM", "alldegreeGraduateVM_Id1", "dbo.alldegreeGraduateVM", "Id");
            AddForeignKey("dbo.DetailsRequirementVM", "alldegreeGraduateVM_Id2", "dbo.alldegreeGraduateVM", "Id");
            AddForeignKey("dbo.DetailsRequirementVM", "alldegreeGraduateVM_Id3", "dbo.alldegreeGraduateVM", "Id");
            AddForeignKey("dbo.USERS", "RapportGraduate_Id", "dbo.alldegreeGraduateVM", "Id");
            DropColumn("dbo.DetailsRequirementVM", "USER_User_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.DetailsRequirementVM", "USER_User_Id", c => c.Int());
            DropForeignKey("dbo.USERS", "RapportGraduate_Id", "dbo.alldegreeGraduateVM");
            DropForeignKey("dbo.DetailsLeadershipVM", "alldegreeGraduateVM_Id", "dbo.alldegreeGraduateVM");
            DropForeignKey("dbo.DetailsRequirementVM", "alldegreeGraduateVM_Id3", "dbo.alldegreeGraduateVM");
            DropForeignKey("dbo.DetailsRequirementVM", "alldegreeGraduateVM_Id2", "dbo.alldegreeGraduateVM");
            DropForeignKey("dbo.DetailsRequirementVM", "alldegreeGraduateVM_Id1", "dbo.alldegreeGraduateVM");
            DropForeignKey("dbo.DetailsRequirementVM", "alldegreeGraduateVM_Id", "dbo.alldegreeGraduateVM");
            DropIndex("dbo.DetailsLeadershipVM", new[] { "alldegreeGraduateVM_Id" });
            DropIndex("dbo.DetailsRequirementVM", new[] { "alldegreeGraduateVM_Id3" });
            DropIndex("dbo.DetailsRequirementVM", new[] { "alldegreeGraduateVM_Id2" });
            DropIndex("dbo.DetailsRequirementVM", new[] { "alldegreeGraduateVM_Id1" });
            DropIndex("dbo.DetailsRequirementVM", new[] { "alldegreeGraduateVM_Id" });
            DropIndex("dbo.USERS", new[] { "RapportGraduate_Id" });
            DropColumn("dbo.DetailsRequirementVM", "alldegreeGraduateVM_Id3");
            DropColumn("dbo.DetailsRequirementVM", "alldegreeGraduateVM_Id2");
            DropColumn("dbo.DetailsRequirementVM", "alldegreeGraduateVM_Id1");
            DropColumn("dbo.DetailsRequirementVM", "alldegreeGraduateVM_Id");
            DropColumn("dbo.USERS", "RapportGraduate_Id");
            DropTable("dbo.DetailsLeadershipVM");
            DropTable("dbo.alldegreeGraduateVM");
            CreateIndex("dbo.DetailsRequirementVM", "USER_User_Id");
            AddForeignKey("dbo.DetailsRequirementVM", "USER_User_Id", "dbo.USERS", "User_Id");
        }
    }
}
