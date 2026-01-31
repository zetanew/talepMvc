using Microsoft.EntityFrameworkCore;
using TalepYonetimi.Models;
using TalepYonetimi.Enums;

namespace TalepYonetimi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Permission> Permissions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<Request> Requests { get; set; }
        public DbSet<RequestStatusHistory> RequestStatusHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                
                entity.HasOne(u => u.Role)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.RoleId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // Role configuration
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // Permission configuration
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // RolePermission configuration (Many-to-Many)
            modelBuilder.Entity<RolePermission>(entity =>
            {
                entity.HasIndex(e => new { e.RoleId, e.PermissionId }).IsUnique();

                entity.HasOne(rp => rp.Role)
                    .WithMany(r => r.RolePermissions)
                    .HasForeignKey(rp => rp.RoleId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(rp => rp.Permission)
                    .WithMany(p => p.RolePermissions)
                    .HasForeignKey(rp => rp.PermissionId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // Request configuration
            modelBuilder.Entity<Request>(entity =>
            {
                entity.HasIndex(e => e.RequestNumber).IsUnique();
                entity.Property(e => e.RequestType).HasConversion<int>();
                entity.Property(e => e.Priority).HasConversion<int>();
                entity.Property(e => e.Status).HasConversion<int>();

                entity.HasOne(r => r.CreatedByUser)
                    .WithMany(u => u.Requests)
                    .HasForeignKey(r => r.CreatedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            // RequestStatusHistory configuration
            modelBuilder.Entity<RequestStatusHistory>(entity =>
            {
                entity.Property(e => e.OldStatus).HasConversion<int>();
                entity.Property(e => e.NewStatus).HasConversion<int>();

                entity.HasOne(h => h.Request)
                    .WithMany(r => r.StatusHistory)
                    .HasForeignKey(h => h.RequestId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(h => h.ChangedByUser)
                    .WithMany()
                    .HasForeignKey(h => h.ChangedByUserId)
                    .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }
}
