using Friends_Date_API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Friends_Date_API.Data
{
    // DbContext Represent the session with database
    // since we we have used identity we will inherit IdentityDbContext
    // we are intrested to deal with role, get list of role, generate token, and claim user info so
    // we need to provide in IdentityDbContext

    public class DataContext : IdentityDbContext<User, Role, int,
            IdentityUserClaim<int>, UserRole, IdentityUserLogin<int>,
            IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //many to many relationship between user and role
            modelBuilder.Entity<User>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.User)
                .HasForeignKey(ur => ur.UserId)
                .IsRequired();

            modelBuilder.Entity<Role>()
                .HasMany(ur => ur.UserRoles)
                .WithOne(u => u.Role)
                .HasForeignKey(ur => ur.RoleId)
                .IsRequired();

            // many to many relationship between User and Like
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


            // Many to Many relationship for Message
            // one recipient will received many messae
            modelBuilder.Entity<Message>()
                .HasOne(u => u.Recipient)
                .WithMany(m => m.MessageReceived)
                .OnDelete(DeleteBehavior.Restrict);

            // one sender can send many message
            modelBuilder.Entity<Message>()
               .HasOne(u => u.Sender)
               .WithMany(m => m.MessageSent)
               .OnDelete(DeleteBehavior.Restrict);

        }

        // we don't need Users Entity to define because we have used microsoft identity 
        // public DbSet<User> Users { get; set; }
        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Connection> Connections { get; set; }

    }
}
