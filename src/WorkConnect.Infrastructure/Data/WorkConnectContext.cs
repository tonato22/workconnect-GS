using Microsoft.EntityFrameworkCore;
using WorkConnect.Domain.Entities;

namespace WorkConnect.Infrastructure.Data
{
    public class WorkConnectContext : DbContext
    {
        public WorkConnectContext(DbContextOptions<WorkConnectContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users => Set<User>();
        public DbSet<Tip> Tips => Set<Tip>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("Users");
                entity.HasKey(u => u.Id);

                entity.Property(u => u.Name)
                      .IsRequired()
                      .HasMaxLength(100);

                entity.Property(u => u.Email)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(u => u.Occupation)
                      .HasMaxLength(100);

                entity.Property(u => u.Country)
                      .HasMaxLength(80);

                entity.Property(u => u.ExperienceLevel)
                      .HasMaxLength(50);

                entity.HasIndex(u => u.Email)
                      .IsUnique();
            });

            modelBuilder.Entity<Tip>(entity =>
            {
                entity.ToTable("Tips");
                entity.HasKey(t => t.Id);

                entity.Property(t => t.Title)
                      .IsRequired()
                      .HasMaxLength(150);

                entity.Property(t => t.Content)
                      .IsRequired();

                entity.Property(t => t.Category)
                      .HasMaxLength(80);

                entity.HasOne(t => t.Author)
                      .WithMany(u => u.Tips)
                      .HasForeignKey(t => t.AuthorId)
                      .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
