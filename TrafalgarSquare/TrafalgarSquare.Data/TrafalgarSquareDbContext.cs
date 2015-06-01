namespace TrafalgarSquare.Data
{
    using System.Data.Entity;
    using System.Data.Entity.ModelConfiguration.Conventions;
    using Microsoft.AspNet.Identity.EntityFramework;
    using TrafalgarSquare.Data.Migrations;
    using TrafalgarSquare.Models;

    public class TrafalgarSquareDbContext : IdentityDbContext<User>, ITrafalgarSquareDbContext
    {
        public TrafalgarSquareDbContext()
            : base("TrafalgarSquare", throwIfV1Schema: false)
        {
        }

        public virtual IDbSet<Notification> Notifications { get; set; }

        public virtual IDbSet<Message> Messages { get; set; }

        public IDbSet<Category> Categories { get; set; }

        public IDbSet<Comment> Comments { get; set; }

        public virtual IDbSet<ReportedPosts> UsersReportedPosts { get; set; }

        public IDbSet<PostLikes> UsersLikes { get; set; }

        public IDbSet<UserFriends> UsersFriends { get; set; }

        public virtual IDbSet<Post> Posts { get; set; }


        public static TrafalgarSquareDbContext Create()
        {
            return new TrafalgarSquareDbContext();
        }

        public new IDbSet<T> Set<T>() where T : class
        {
            return base.Set<T>();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<TrafalgarSquareDbContext, Configuration>());
            modelBuilder.Conventions.Remove<OneToManyCascadeDeleteConvention>();

            // User's Friends
            modelBuilder.Entity<User>()
                .HasMany(x => x.Friends)
                .WithRequired(x => x.User)
                .HasForeignKey(x => x.UserId);

            // Messages
            modelBuilder.Entity<User>()
                .HasMany(x => x.Messages)
                .WithRequired(x => x.Sender)
                .HasForeignKey(x => x.SenderId);

            // Notifications
            modelBuilder.Entity<User>()
                .HasMany(x => x.Notifications)
                .WithRequired(x => x.Recepient)
                .HasForeignKey(x => x.RecepientId);

            // User's Posts
            modelBuilder.Entity<User>()
                .HasMany(x => x.Posts)
                .WithRequired(x => x.PostOwner)
                .HasForeignKey(x => x.PostOwnerId);

            // Reported Posts by user
            modelBuilder.Entity<User>()
                .HasMany(m => m.ReportedPosts)
                .WithRequired(x => x.Reporter)
                .HasForeignKey(x => x.ReportedUserId)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Post>()
                .HasMany(x => x.UsersReportedPosts)
                .WithRequired(x => x.Post)
                .HasForeignKey(x => x.PostId)
                .WillCascadeOnDelete(true);

            // Post likes
            modelBuilder.Entity<Post>()
                .HasMany(x => x.LikesPost)
                .WithRequired(x => x.Post)
                .HasForeignKey(x => x.PostId)
                .WillCascadeOnDelete(true);

            // Liked Posts by user
            modelBuilder.Entity<User>()
                .HasMany(m => m.LikedPosts)
                .WithRequired(x => x.User)
                .HasForeignKey(x => x.UserId)
                .WillCascadeOnDelete(false);

            // User's Comments
            modelBuilder.Entity<User>()
                .HasMany(x => x.Comments)
                .WithRequired(x => x.User)
                .HasForeignKey(x => x.UserId);

           /* // Liked Comments by User
            modelBuilder.Entity<User>()
                .HasMany(m => m.LikedComments)
                .WithRequired(x => x.User)
                .HasForeignKey(x => x.UserId)
                .WillCascadeOnDelete(false);*/

            modelBuilder.Entity<IdentityUserLogin>().HasKey<string>(l => l.UserId);
            modelBuilder.Entity<IdentityRole>().HasKey<string>(r => r.Id);
            modelBuilder.Entity<IdentityUserRole>().HasKey(r => new { r.RoleId, r.UserId });

            base.OnModelCreating(modelBuilder);
        }
    }
}
