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
        public DbSet<Request> Requests { get; set; }
        public DbSet<RequestStatusHistory> RequestStatusHistories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // User configuration
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email).IsUnique();
                entity.Property(e => e.Role).HasConversion<int>();
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
