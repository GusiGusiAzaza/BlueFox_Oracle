using System.Data.Entity;

namespace BlueFoxTests_Oracle.Models
{
    public partial class BlueFoxContext : DbContext
    {
        public BlueFoxContext()
            : base("name=BlueFoxContext")
        {
        }

        public virtual DbSet<Admin> Admins { get; set; }
        public virtual DbSet<Answers_For_Tests> Answers_For_Tests { get; set; }
        public virtual DbSet<HomeWebPage> HomeWebPages { get; set; }
        public virtual DbSet<Questions_For_Tests> Questions_For_Tests { get; set; }
        public virtual DbSet<Test_Progress> Test_Progress { get; set; }
        public virtual DbSet<Test> Tests { get; set; }
        public virtual DbSet<Themes_For_Tests> Themes_For_Tests { get; set; }
        public virtual DbSet<User_Info> User_Info { get; set; }
        public virtual DbSet<User_Stats> User_Stats { get; set; }
        public virtual DbSet<User> Users { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany(e => e.Admins)
                .WithRequired(e => e.User)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<User>()
                .HasOptional(e => e.User_Info)
                .WithRequired(e => e.User);

            modelBuilder.Entity<User>()
                .HasOptional(e => e.User_Stats)
                .WithRequired(e => e.User);
        }
    }
}
