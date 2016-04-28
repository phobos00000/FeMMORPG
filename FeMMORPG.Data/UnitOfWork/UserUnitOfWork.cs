using System.Data.Entity;

namespace FeMMORPG.Data
{
    public class UserUnitOfWork : UnitOfWork
    {
        private IRepository<User> users;
        private IRepository<Character> characters;

        public UserUnitOfWork(DbContext context) : base(context)
        {
        }

        public IRepository<User> Users
            => users ?? (users = new Repository<User>(Context));

        public IRepository<Character> Characters
            => characters ?? (characters = new Repository<Character>(Context));
    }
}
