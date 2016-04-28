namespace FeMMORPG.Data
{
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    using System.Linq;

    public class Repository<T> : IRepository<T> where T : class
    {
        private DbContext context;

        public Repository(DbContext context)
        {
            this.context = context;
        }

        private DbSet<T> GetDbSet()
        {
            return context.Set<T>();
        }

        public void Add(T item)
        {
            GetDbSet().Add(item);
        }

        public void Update(T item)
        {
            context.Entry(item).State = EntityState.Modified;
        }

        public void Remove(T item)
        {
            GetDbSet().Remove(item);
        }

        public IQueryable<T> Query()
        {
            return GetDbSet().AsQueryable();
        }

        public IQueryable<T> Query(string[] includes)
        {
            if (includes.Length == 0)
            {
                return Query();
            }

            DbQuery<T> query = null;
            foreach (var include in includes)
            {
                if (query == null)
                {
                    query = GetDbSet().Include(include);
                }
                else
                {
                    query = query.Include(include);
                }
            }

            return query.AsQueryable();
        }

        public T Find(params object[] keyValues)
        {
            return GetDbSet().Find(keyValues);
        }
    }
}