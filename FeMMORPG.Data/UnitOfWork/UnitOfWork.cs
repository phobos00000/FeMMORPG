using System.Data.Entity;

namespace FeMMORPG.Data
{
    public abstract class UnitOfWork : IUnitOfWork
    {
        protected DbContext Context { get; private set; }

        protected UnitOfWork(DbContext context)
        {
            Context = context;
        }

        public void SaveChanges()
        {
            Context.SaveChanges();
        }
    }
}
