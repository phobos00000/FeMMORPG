namespace FeMMORPG.Data
{
    public interface IUserUnitOfWork : IUnitOfWork
    {
        IRepository<User> Users { get; }
        IRepository<LoginToken> LoginTokens { get; }
        IRepository<Server> Servers { get; }
    }
}
