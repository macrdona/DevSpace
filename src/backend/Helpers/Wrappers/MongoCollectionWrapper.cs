using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace backend.Helpers.Wrappers
{
    public interface IMongoCollectionWrapper<TDocument>
    {
        Task<TDocument> Find(Expression<Func<TDocument, bool>> filter, FindOptions<TDocument, TDocument> options = null);
        Task<long> CountDocumentsAsync(Expression<Func<TDocument, bool>> filter, CountOptions options = null, CancellationToken cancellationToken = default(CancellationToken));
        Task InsertOneAsync(TDocument document, InsertOneOptions options = null, CancellationToken cancellationToken = default(CancellationToken));

    }

    public class MongoCollectionWrapper<TDocument> : IMongoCollectionWrapper<TDocument>
    {
        private readonly IMongoCollection<TDocument> _mongoCollection;

        public MongoCollectionWrapper(IMongoCollection<TDocument> mongoCollection)
        {
            _mongoCollection = mongoCollection;
        }

        public async Task<TDocument> Find(Expression<Func<TDocument, bool>> filter, FindOptions<TDocument, TDocument> options = null)
        {
            return await _mongoCollection.Find(filter).FirstOrDefaultAsync();
            
        }

        public async Task<long> CountDocumentsAsync(Expression<Func<TDocument, bool>> filter, CountOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var count = await _mongoCollection.CountDocumentsAsync(filter, options);
            return count;
        }

        public async Task InsertOneAsync(TDocument document, InsertOneOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            await _mongoCollection.InsertOneAsync(document);
        }
    }
}
