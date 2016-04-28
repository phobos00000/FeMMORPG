using System.Linq;

namespace FeMMORPG.Data
{
    public interface IRepository<T> where T : class
    {
        void Add(T item);
        T Find(params object[] keyValues);
        IQueryable<T> Query();
        IQueryable<T> Query(string[] includes);
        void Remove(T item);
        void Update(T item);
    }
}
