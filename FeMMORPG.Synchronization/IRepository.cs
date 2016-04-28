using System.Linq;

namespace FeMMORPG.Synchronization
{
    public interface IRepository<T> where T : class
    {
        T Find(string id);
        IQueryable<T> Query();
        void Add(T item);
        void Remove(string id);
        void Update(T item);
    }
}
