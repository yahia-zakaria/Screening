namespace Studentscreeningsystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class floatavreage : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.SpecificationGraduate", "AverageGraduate", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SpecificationGraduate", "AverageGraduate", c => c.Int(nullable: false));
        }
    }
}
