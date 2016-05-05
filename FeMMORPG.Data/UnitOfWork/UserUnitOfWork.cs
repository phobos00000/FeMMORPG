using System.Data.Entity;

namespace FeMMORPG.Data
{
    public class UserUnitOfWork : UnitOfWork, IUserUnitOfWork
    {
        private IRepository<User> users;
        private IRepository<LoginToken> loginTokens;
        private IRepository<Server> servers;

        public UserUnitOfWork(DbContext context) : base(context)
        {
        }

        public IRepository<User> Users
            => users ?? (users = new Repository<User>(Context));

        public IRepository<LoginToken> LoginTokens
            => loginTokens ?? (loginTokens = new Repository<LoginToken>(Context));

        public IRepository<Server> Servers
            => servers ?? (servers = new Repository<Server>(Context));
    }
}
