namespace Studentscreeningsystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangePercentageFromintToDecimal : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.DegreeGraduate", "percentage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.DetailsRequirementVM", "percentage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.RequirementSector", "percentage", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.RequirementSector", "percentage", c => c.Int(nullable: false));
            AlterColumn("dbo.DetailsRequirementVM", "percentage", c => c.Int(nullable: false));
            AlterColumn("dbo.DegreeGraduate", "percentage", c => c.Int(nullable: false));
        }
    }
}
