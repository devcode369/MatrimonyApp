using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : IdentityDbContext<AppUser, AppRole, int,
    IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,
    IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }


        //   public DbSet<AppUser> Users { get; set; }

        public DbSet<UserLike> Likes { get; set; }

        public DbSet<Message> Messages { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Connection> Connections { get; set; }

        public DbSet<Photo>Photos{get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<AppUser>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.User)
            .HasForeignKey(u => u.UserId)
            .IsRequired();

            modelBuilder.Entity<AppRole>()
            .HasMany(ur => ur.UserRoles)
            .WithOne(u => u.Role)
            .HasForeignKey(u => u.RoleId)
            .IsRequired();


            modelBuilder.Entity<UserLike>()
                        .HasKey(k => new { k.SourceUserId, k.TargetUserId });

            modelBuilder.Entity<UserLike>()
                         .HasOne(s => s.SourceUser)
                         .WithMany(l => l.LikedUsers)
                         .HasForeignKey(s => s.SourceUserId)
                         .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<UserLike>()
                         .HasOne(s => s.TargetUser)
                         .WithMany(l => l.LikedByUsers)
                         .HasForeignKey(s => s.TargetUserId)
                         .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Message>()
                      .HasOne(u => u.Recipient)
                      .WithMany(m => m.MessageReceived)
                      .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Message>()
                     .HasOne(u => u.Sender)
                     .WithMany(m => m.MessageSent)
                     .OnDelete(DeleteBehavior.NoAction);

             modelBuilder.Entity<Photo>().HasQueryFilter(p=>p.IsApproved);
           // modelBuilder.ApplyUtcDateTimeConverter();
        }

    }
}