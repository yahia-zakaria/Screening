namespace Studentscreeningsystem.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Data.Entity.ModelConfiguration.Conventions;

    public partial class RBAC_Model : DbContext
    {
        public RBAC_Model()
            : base("name=RBAC_Model")
        {
        }
        public DbSet<Sector> Sector { get; set; }

        public DbSet<Requirement> Requirement { get; set; }
        public DbSet<RequirementSector> RequirementSector { get; set; }
        public DbSet<Leadershiprequirement> Leadershiprequirement { get; set; }
        public DbSet<LeadershipSector> LeadershipSector { get; set; }
        public DbSet<Specification> Specification { get; set; }
      
        public DbSet<DegreeGraduate> DegreeGraduate { get; set; }
        public DbSet<LeadershipGraduate> LeadershipGraduate { get; set; }
        public DbSet<SpecificationGraduate> SpecificationGraduate { get; set; }
        public DbSet<GraduateWishes> GraduateWishes { get; set; }
        public virtual DbSet<PERMISSION> PERMISSIONS { get; set; }
        public virtual DbSet<ROLE> ROLES { get; set; }
        public virtual DbSet<USER> USERS { get; set; }
        public DbSet<MouvementUsers> MouvementUsers { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PERMISSION>()
                .HasMany(e => e.ROLES)
                .WithMany(e => e.PERMISSIONS)
                .Map(m => m.ToTable("LNK_ROLE_PERMISSION").MapLeftKey("Permission_Id").MapRightKey("Role_Id"));

            modelBuilder.Entity<ROLE>()
                .HasMany(e => e.USERS)
                .WithMany(e => e.ROLES)
                .Map(m => m.ToTable("LNK_USER_ROLE").MapLeftKey("Role_Id").MapRightKey("User_Id"));
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }
    }
}
