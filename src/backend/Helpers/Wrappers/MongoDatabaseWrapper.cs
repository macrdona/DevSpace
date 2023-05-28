using MongoDB.Driver;

namespace backend.Helpers.Wrappers
{
    public interface IMongoDatabaseWrapper
    {
        public IMongoCollectionWrapper<TDocument> GetCollection<TDocument>(string database, MongoCollectionSettings settings = null);
    }
    public class MongoDatabaseWrapper : IMongoDatabaseWrapper
    {
        private readonly IMongoDatabase _mongoDatabase;

        public MongoDatabaseWrapper(IMongoDatabase mongoDatabase)
        {
            _mongoDatabase = mongoDatabase;
        }

        public IMongoCollectionWrapper<TDocument> GetCollection<TDocument>(string database, MongoCollectionSettings settings = null)
        {
            var collection = _mongoDatabase.GetCollection<TDocument>(database, settings);
            return new MongoCollectionWrapper<TDocument>(collection);
        }
    }
}
