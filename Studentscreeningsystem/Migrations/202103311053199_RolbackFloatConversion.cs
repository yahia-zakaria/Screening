namespace Studentscreeningsystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RolbackFloatConversion : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.USERS", "AverageGraduate", c => c.Single(nullable: false));
            AlterColumn("dbo.alldegreeGraduateVM", "AverageGraduate", c => c.Single(nullable: false));
            AlterColumn("dbo.SpecificationGraduate", "AverageGraduate", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SpecificationGraduate", "AverageGraduate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.alldegreeGraduateVM", "AverageGraduate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.USERS", "AverageGraduate", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
