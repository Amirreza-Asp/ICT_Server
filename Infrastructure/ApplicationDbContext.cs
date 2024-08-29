using Domain.Entities.Account;
using Domain.Entities.Business;
using Domain.Entities.Static;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<BigGoal> BigGoals { get; set; }
        public DbSet<OperationalObjective> OperationalObjectives { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<HardwareEquipment> HardwareEquipment { get; set; }
        public DbSet<Domain.Entities.Business.System> Systems { get; set; }
        public DbSet<Transition> Transitions { get; set; }
        public DbSet<Indicator> Indicators { get; set; }
        public DbSet<OperationalObjectiveIndicator> OperationalObjectiveIndicators { get; set; }
        public DbSet<TransitionIndicator> TransitionIndicators { get; set; }
        public DbSet<BigGoalIndicator> BigGoalIndicators { get; set; }
        public DbSet<IndicatorProgress> IndicatorProgresses { get; set; }
        public DbSet<CompanyIndicator> CompanyIndicators { get; set; }
        public DbSet<Program> Program { get; set; }
        public DbSet<ProgramBigGoal> ProgramBigGoal { get; set; }
        public DbSet<Perspective> Perspective { get; set; }
        public DbSet<SWOT> SWOT { get; set; }
        public DbSet<Strategy> Strategy { get; set; }



        public DbSet<ProgramYear> ProgramYears { get; set; }
        public DbSet<IndicatorCategory> IndicatorCategories { get; set; }
        public DbSet<IndicatorType> IndicatorTypes { get; set; }


        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        private DbSet<PermissionContainer> PermissionContainers { get; set; }
        private DbSet<PermissionItem> permissionItems { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Company> Company { get; set; }
        public DbSet<Token> Tokens { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<UserJoinRequest> UsersJoinRequests { get; set; }
        public DbSet<Act> Act { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Person>()
                .OwnsMany(b => b.Expertises, b =>
                {
                    b.Property(b => b.Title).IsRequired().HasColumnName("title");
                });

            modelBuilder.Entity<Transition>()
                .OwnsMany(b => b.Financials, b =>
                {
                    b.Property(b => b.Title).IsRequired().HasColumnName("title");
                });


            modelBuilder.Entity<User>()
                .HasIndex(b => b.NationalId).IsUnique(true);

            modelBuilder.Entity<ProgramBigGoal>()
                .HasKey(b => new { b.BigGoalId, b.ProgramId });

            modelBuilder.Entity<Perspective>()
                .HasOne(b => b.Program)
                .WithOne(b => b.Perspective)
                .HasForeignKey<Perspective>(b => b.ProgramId);

            modelBuilder.Entity<Transition>()
                .HasOne(b => b.Leader)
                .WithMany(b => b.Transitions)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserJoinRequest>()
                .Property(b => b.CompanyId)
                .IsRequired(false);

            modelBuilder.Entity<OperationalObjectiveIndicator>()
                .HasKey(b => new { b.IndicatorId, b.OperationalObjectiveId });

            modelBuilder.Entity<BigGoalIndicator>()
                .HasKey(b => new { b.IndicatorId, b.BigGoalId });

            modelBuilder.Entity<CompanyIndicator>()
                .HasKey(b => new { b.IndicatorId, b.CompanyId });

            modelBuilder.Entity<Transition>()
                .HasOne(b => b.Parent)
                .WithMany(b => b.Childs)
                .HasForeignKey(b => b.ParentId);
        }
    }
}
