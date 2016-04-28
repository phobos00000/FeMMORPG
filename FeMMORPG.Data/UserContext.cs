using System.Data.Entity;

namespace FeMMORPG.Data
{
    public class UserContext : DbContext
    {
        public UserContext() : base(nameof(UserContext))
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Character> Characters { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new UserMap());
            modelBuilder.Configurations.Add(new CharacterMap());
        }
    }
}
