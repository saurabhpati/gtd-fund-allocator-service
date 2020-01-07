using Microsoft.EntityFrameworkCore;

namespace GTDFundAllocatorService.Repository.Shared
{
    public partial class FundAllocatorDbContext : DbContext
    {
        public FundAllocatorDbContext()
        {
        }

        public FundAllocatorDbContext(DbContextOptions<FundAllocatorDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Fund> Fund { get; set; }

        public virtual DbSet<Role> Role { get; set; }

        public virtual DbSet<Status> Status { get; set; }

        public virtual DbSet<User> User { get; set; }

        public virtual DbSet<UserRole> UserRole { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<Fund>(entity =>
            {
                entity.Property(e => e.ApprovedOn).HasColumnType("datetime");

                entity.Property(e => e.CreatedOn).HasColumnType("datetime");

                entity.Property(e => e.StatusId).HasDefaultValueSql("((1))");

                entity.Property(e => e.UpdatedOn).HasColumnType("datetime");

                entity.HasOne(d => d.Approver)
                    .WithMany(p => p.ApprovedFund)
                    .HasForeignKey(d => d.ApprovedBy)
                    .HasConstraintName("FK_Fund_User_Approver");

                entity.HasOne(d => d.Creator)
                    .WithMany(p => p.CreatedFund)
                    .HasForeignKey(d => d.CreatedBy)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Fund_User_Creator");

                entity.HasOne(d => d.Status)
                    .WithMany(p => p.Fund)
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Fund_Status");

                entity.HasOne(d => d.Updater)
                    .WithMany(p => p.UpdatedFund)
                    .HasForeignKey(d => d.UpdatedBy)
                    .HasConstraintName("FK_Fund_User_Updator");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(32);
            });

            modelBuilder.Entity<Status>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(32);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(64);

                entity.Property(e => e.LastName).HasMaxLength(64);
            });

            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.UserRole)
                    .HasForeignKey(d => d.RoleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRole_Role");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UserRole)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_UserRole_User");
            });
        }
    }
}
