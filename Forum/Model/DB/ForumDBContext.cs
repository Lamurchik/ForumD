using Microsoft.EntityFrameworkCore;

namespace Forum.Model.DB
{
    public class ForumDBContext : DbContext
    {
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
       
        public DbSet<UserInfo> UsersInfo { get; set; }

        public DbSet<PostPartial> PostPartials { get; set; }

        public DbSet<Grade> Grades { get; set; }

        public DbSet<Tags> Tags { get; set; }
        public DbSet<SocialNotification> SocialNotifications { get; set; }

        public DbSet<Blacklist> Blacklists { get; set; }

        //public DbSet<Subscriptions> Subscriptions { get; set; }


        public ForumDBContext(DbContextOptions<ForumDBContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<Grade>(entity =>
            {
                entity.ToTable("Grade", t => t.HasCheckConstraint(
         "CK_Grade_PostOrComment",
         "(\"PostId\" IS NOT NULL AND \"CommentId\" IS NULL) OR (\"PostId\" IS NULL AND \"CommentId\" IS NOT NULL)"
     ));
            });

            modelBuilder.Entity<Tags>()
                .HasIndex(t => t.Name)
                    .IsUnique();
        

            /*
            modelBuilder.Entity<Subscriptions>(entity =>
            {
                entity.ToTable(tb =>
                {
                    tb.HasCheckConstraint(
                        "CK_Subscriptions_NotSelf",
                        "subscriberid <> targetuserid");
                });

                entity.HasIndex(s => new { s.SubscriberId, s.TargetUserId })
                    .IsUnique();
            });



            modelBuilder.Entity<Subscriptions>()
       .HasOne(s => s.Subscriber)
       .WithMany(u => u.Subscriptions)
       .HasForeignKey(s => s.SubscriberId)
       .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Subscriptions>()
                .HasOne(s => s.TargetUser)
                .WithMany(u => u.Subscribers)
                .HasForeignKey(s => s.TargetUserId)
                .OnDelete(DeleteBehavior.Restrict);
            */


        }
    }
}
