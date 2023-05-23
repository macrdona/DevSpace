using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace backend.Tests.Helpers
{
    public interface IMongoCollectionWrapper<TDocument>
    {
        IFindFluentWrapper<TDocument> Find(Expression<Func<TDocument, bool>> filter, FindOptions<TDocument, TDocument> options = null);
        Task<long> CountDocumentsAsync(Expression<Func<TDocument, bool>> filter, CountOptions options = null, CancellationToken cancellationToken = default(CancellationToken));
    }

    public class MongoCollectionWrapper<TDocument> : IMongoCollectionWrapper<TDocument>
    {
        private readonly IMongoCollection<TDocument> _mongoCollection;

        public MongoCollectionWrapper(IMongoCollection<TDocument> mongoCollection)
        {
            _mongoCollection = mongoCollection;
        }

        public IFindFluentWrapper<TDocument> Find(Expression<Func<TDocument, bool>> filter, FindOptions<TDocument, TDocument> options = null)
        {
            var findFluent = _mongoCollection.Find(filter);
            return new FindFluentWrapper<TDocument>(findFluent);
        }

        public Task<long> CountDocumentsAsync(Expression<Func<TDocument, bool>> filter, CountOptions options = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            var count = _mongoCollection.CountDocumentsAsync(filter, options);
            return count;
        }
    }
}
