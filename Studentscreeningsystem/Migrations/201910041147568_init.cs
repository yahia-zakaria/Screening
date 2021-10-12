namespace Studentscreeningsystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class init : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DegreeGraduate",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        percentage = c.Int(nullable: false),
                        IdRequirement = c.Int(nullable: false),
                        User_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Requirement", t => t.IdRequirement, cascadeDelete: true)
                .ForeignKey("dbo.USERS", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.IdRequirement)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Requirement",
                c => new
                    {
                        IdRequirement = c.Int(nullable: false, identity: true),
                        NameRequirement = c.String(),
                        Categorie = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdRequirement);
            
            CreateTable(
                "dbo.USERS",
                c => new
                    {
                        User_Id = c.Int(nullable: false, identity: true),
                        Username = c.String(nullable: false, maxLength: 10),
                        LastModified = c.DateTime(),
                        Inactive = c.Boolean(),
                        Firstname = c.String(maxLength: 50),
                        Lastname = c.String(maxLength: 50),
                        Title = c.String(maxLength: 30),
                        Insortable = c.Boolean(),
                        Passeword = c.String(nullable: false),
                        Comfirmepasseword = c.String(),
                    })
                .PrimaryKey(t => t.User_Id);
            
            CreateTable(
                "dbo.ROLES",
                c => new
                    {
                        Role_Id = c.Int(nullable: false, identity: true),
                        RoleName = c.String(nullable: false),
                        RoleDescription = c.String(),
                        IsSysAdmin = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Role_Id);
            
            CreateTable(
                "dbo.PERMISSIONS",
                c => new
                    {
                        Permission_Id = c.Int(nullable: false, identity: true),
                        PermissionDescription = c.String(nullable: false, maxLength: 50),
                        PermissionName = c.String(),
                    })
                .PrimaryKey(t => t.Permission_Id);
            
            CreateTable(
                "dbo.GraduateWishes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        User_Id = c.Int(nullable: false),
                        IdSector = c.Int(nullable: false),
                        Rank = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Sector", t => t.IdSector, cascadeDelete: true)
                .ForeignKey("dbo.USERS", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.User_Id)
                .Index(t => t.IdSector);
            
            CreateTable(
                "dbo.Sector",
                c => new
                    {
                        IdSector = c.Int(nullable: false, identity: true),
                        NameSector = c.String(nullable: false),
                        NbGraduates = c.Int(nullable: false),
                        LogoPath = c.String(),
                    })
                .PrimaryKey(t => t.IdSector);
            
            CreateTable(
                "dbo.LeadershipGraduate",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsChecked = c.Boolean(nullable: false),
                        IdLeadership = c.Int(nullable: false),
                        User_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Leadershiprequirement", t => t.IdLeadership, cascadeDelete: true)
                .ForeignKey("dbo.USERS", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.IdLeadership)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.Leadershiprequirement",
                c => new
                    {
                        IdLeadership = c.Int(nullable: false, identity: true),
                        NameLeadership = c.String(),
                    })
                .PrimaryKey(t => t.IdLeadership);
            
            CreateTable(
                "dbo.LeadershipSector",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        IsChecked = c.Boolean(nullable: false),
                        IdLeadership = c.Int(nullable: false),
                        IdSector = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Leadershiprequirement", t => t.IdLeadership, cascadeDelete: true)
                .ForeignKey("dbo.Sector", t => t.IdSector, cascadeDelete: true)
                .Index(t => t.IdLeadership)
                .Index(t => t.IdSector);
            
            CreateTable(
                "dbo.MouvementUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        User_Id = c.Int(nullable: false),
                        Link = c.String(),
                        DateMouvement = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.USERS", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.RequirementSector",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        percentage = c.Int(nullable: false),
                        priority = c.Int(nullable: false),
                        IdRequirement = c.Int(nullable: false),
                        IdSector = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Requirement", t => t.IdRequirement, cascadeDelete: true)
                .ForeignKey("dbo.Sector", t => t.IdSector, cascadeDelete: true)
                .Index(t => t.IdRequirement)
                .Index(t => t.IdSector);
            
            CreateTable(
                "dbo.Specification",
                c => new
                    {
                        IdSpecification = c.Int(nullable: false, identity: true),
                        Maxlenght = c.Int(nullable: false),
                        Minlenght = c.Int(nullable: false),
                        Maxweight = c.Int(nullable: false),
                        Minweight = c.Int(nullable: false),
                        IdSector = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdSpecification)
                .ForeignKey("dbo.Sector", t => t.IdSector, cascadeDelete: true)
                .Index(t => t.IdSector);
            
            CreateTable(
                "dbo.SpecificationGraduate",
                c => new
                    {
                        IdSpecification = c.Int(nullable: false, identity: true),
                        Length = c.Int(nullable: false),
                        Weight = c.Int(nullable: false),
                        AverageGraduate = c.Int(nullable: false),
                        User_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.IdSpecification)
                .ForeignKey("dbo.USERS", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.LNK_ROLE_PERMISSION",
                c => new
                    {
                        Permission_Id = c.Int(nullable: false),
                        Role_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Permission_Id, t.Role_Id })
                .ForeignKey("dbo.PERMISSIONS", t => t.Permission_Id, cascadeDelete: true)
                .ForeignKey("dbo.ROLES", t => t.Role_Id, cascadeDelete: true)
                .Index(t => t.Permission_Id)
                .Index(t => t.Role_Id);
            
            CreateTable(
                "dbo.LNK_USER_ROLE",
                c => new
                    {
                        Role_Id = c.Int(nullable: false),
                        User_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Role_Id, t.User_Id })
                .ForeignKey("dbo.ROLES", t => t.Role_Id, cascadeDelete: true)
                .ForeignKey("dbo.USERS", t => t.User_Id, cascadeDelete: true)
                .Index(t => t.Role_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SpecificationGraduate", "User_Id", "dbo.USERS");
            DropForeignKey("dbo.Specification", "IdSector", "dbo.Sector");
            DropForeignKey("dbo.RequirementSector", "IdSector", "dbo.Sector");
            DropForeignKey("dbo.RequirementSector", "IdRequirement", "dbo.Requirement");
            DropForeignKey("dbo.MouvementUsers", "User_Id", "dbo.USERS");
            DropForeignKey("dbo.LeadershipSector", "IdSector", "dbo.Sector");
            DropForeignKey("dbo.LeadershipSector", "IdLeadership", "dbo.Leadershiprequirement");
            DropForeignKey("dbo.LeadershipGraduate", "User_Id", "dbo.USERS");
            DropForeignKey("dbo.LeadershipGraduate", "IdLeadership", "dbo.Leadershiprequirement");
            DropForeignKey("dbo.GraduateWishes", "User_Id", "dbo.USERS");
            DropForeignKey("dbo.GraduateWishes", "IdSector", "dbo.Sector");
            DropForeignKey("dbo.DegreeGraduate", "User_Id", "dbo.USERS");
            DropForeignKey("dbo.LNK_USER_ROLE", "User_Id", "dbo.USERS");
            DropForeignKey("dbo.LNK_USER_ROLE", "Role_Id", "dbo.ROLES");
            DropForeignKey("dbo.LNK_ROLE_PERMISSION", "Role_Id", "dbo.ROLES");
            DropForeignKey("dbo.LNK_ROLE_PERMISSION", "Permission_Id", "dbo.PERMISSIONS");
            DropForeignKey("dbo.DegreeGraduate", "IdRequirement", "dbo.Requirement");
            DropIndex("dbo.LNK_USER_ROLE", new[] { "User_Id" });
            DropIndex("dbo.LNK_USER_ROLE", new[] { "Role_Id" });
            DropIndex("dbo.LNK_ROLE_PERMISSION", new[] { "Role_Id" });
            DropIndex("dbo.LNK_ROLE_PERMISSION", new[] { "Permission_Id" });
            DropIndex("dbo.SpecificationGraduate", new[] { "User_Id" });
            DropIndex("dbo.Specification", new[] { "IdSector" });
            DropIndex("dbo.RequirementSector", new[] { "IdSector" });
            DropIndex("dbo.RequirementSector", new[] { "IdRequirement" });
            DropIndex("dbo.MouvementUsers", new[] { "User_Id" });
            DropIndex("dbo.LeadershipSector", new[] { "IdSector" });
            DropIndex("dbo.LeadershipSector", new[] { "IdLeadership" });
            DropIndex("dbo.LeadershipGraduate", new[] { "User_Id" });
            DropIndex("dbo.LeadershipGraduate", new[] { "IdLeadership" });
            DropIndex("dbo.GraduateWishes", new[] { "IdSector" });
            DropIndex("dbo.GraduateWishes", new[] { "User_Id" });
            DropIndex("dbo.DegreeGraduate", new[] { "User_Id" });
            DropIndex("dbo.DegreeGraduate", new[] { "IdRequirement" });
            DropTable("dbo.LNK_USER_ROLE");
            DropTable("dbo.LNK_ROLE_PERMISSION");
            DropTable("dbo.SpecificationGraduate");
            DropTable("dbo.Specification");
            DropTable("dbo.RequirementSector");
            DropTable("dbo.MouvementUsers");
            DropTable("dbo.LeadershipSector");
            DropTable("dbo.Leadershiprequirement");
            DropTable("dbo.LeadershipGraduate");
            DropTable("dbo.Sector");
            DropTable("dbo.GraduateWishes");
            DropTable("dbo.PERMISSIONS");
            DropTable("dbo.ROLES");
            DropTable("dbo.USERS");
            DropTable("dbo.Requirement");
            DropTable("dbo.DegreeGraduate");
        }
    }
}
