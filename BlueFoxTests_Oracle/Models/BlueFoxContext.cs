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
        public virtual DbSet<Test_Result> Test_Progress { get; set; }
        public virtual DbSet<Test> Tests { get; set; }
        public virtual DbSet<Theme> Themes_For_Tests { get; set; }
        public virtual DbSet<User_Info> User_Info { get; set; }
        public virtual DbSet<User_Stats> User_Stats { get; set; }
        public virtual DbSet<User> Users { get; set; }
    }
}
