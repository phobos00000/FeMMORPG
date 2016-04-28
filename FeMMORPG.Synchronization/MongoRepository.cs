namespace FeMMORPG.Synchronization
{
    using System.Linq;
    using MongoDB.Driver;

    public class MongoRepository<T> : IRepository<T> where T : class, IEntity
    {
        private IMongoDatabase context;

        public MongoRepository(string connectionString)
        {
            var url = new MongoUrl(connectionString);
            var client = new MongoClient(url);
            this.context = client.GetDatabase(url.DatabaseName);
        }

        private IMongoCollection<T> GetCollection()
        {
            return context.GetCollection<T>(typeof(T).Name);
        }

        public void Add(T item)
        {
            GetCollection().InsertOne(item);
        }

        public T Find(string id)
        {
            return GetCollection().Find(x => x.Id == id).SingleOrDefault();
        }

        public IQueryable<T> Query()
        {
            return GetCollection().AsQueryable();
        }

        public void Remove(string id)
        {
            GetCollection().DeleteOne(x => x.Id == id);
        }

        public void Update(T item)
        {
            GetCollection().ReplaceOne(x => x.Id == item.Id, item);
        }
    }
}
