using System.Data.Entity;

namespace FeMMORPG.Data
{
    public class UserDbContext : DbContext
    {
        public UserDbContext() : base(nameof(UserDbContext))
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Server> Servers { get; set; }
        public DbSet<Character> Characters { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new ServerMap());
            modelBuilder.Configurations.Add(new CharacterMap());
        }
    }
}
