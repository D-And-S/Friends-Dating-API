using Friends_Date_API.Entities;
using Microsoft.EntityFrameworkCore;

namespace Friends_Date_API.Data
{
    // Represent the session with database
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserLike>()
                .HasKey(k => new { k.SourceUserId, k.LikeduserId });

            modelBuilder.Entity<UserLike>()
                .HasOne(s => s.SourceUser) 
                .WithMany(l => l.LikedUser)
                .HasForeignKey(l => l.SourceUserId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserLike>()
               .HasOne(s => s.Likeduser)
               .WithMany(l => l.LikedByUsers)
               .HasForeignKey(l => l.LikeduserId)
               .OnDelete(DeleteBehavior.NoAction);

        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserLike> Likes { get; set; }
    }
}
