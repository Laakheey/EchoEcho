using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocialMedia_Data.Entity;
using System.Reflection.Emit;

namespace SocialMedia_Data
{
    public class SocialMediaDataContext : IdentityDbContext<User>
    {
        public SocialMediaDataContext(DbContextOptions<SocialMediaDataContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.Entity<FriendsRequest>()
            //   .HasOne(f => f.Requester)
            //   .WithMany(u => u.SentRequests)
            //   .HasForeignKey(f => f.RequesterId)
            //   .OnDelete(DeleteBehavior.Restrict);

            //builder.Entity<FriendsRequest>()
            //    .HasOne(f => f.Recipient)
            //    .WithMany(u => u.ReceivedRequests)
            //    .HasForeignKey(f => f.RecipientId)
            //    .OnDelete(DeleteBehavior.Restrict);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Chat> Chats { get; set; }
        public DbSet<ChatHead> ChatHeads { get; set; }
        public DbSet<FriendsRequest> FriendsRequests { get; set; }
        public DbSet<Like> Likes { get; set; }
    }
}