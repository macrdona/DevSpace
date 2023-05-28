using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using backend.Models; 

namespace backend.Helpers.Wrappers
{
    public interface IMongoCollectionWrapper<TDocument>
    {
        Task<TDocument> Find(Expression<Func<TDocument, bool>> filter);
        Task<long> CountDocumentsAsync(Expression<Func<TDocument, bool>> filter);
        Task InsertOneAsync(TDocument document);
        Task UpdateOneAsync(Expression<Func<TDocument, bool>> filter, UpdateDefinition<TDocument> update);
        Task DeleteOneAsync(Expression<Func<TDocument, bool>> filter);
        UpdateDefinition<TDocument> UpdateSet<TField>(Expression<Func<TDocument, TField>> field, TField value);
    }

    public class MongoCollectionWrapper<TDocument> : IMongoCollectionWrapper<TDocument>
    {
        private readonly IMongoCollection<TDocument> _mongoCollection;

        public MongoCollectionWrapper(IMongoCollection<TDocument> mongoCollection)
        {
            _mongoCollection = mongoCollection;
        }

        public async Task<TDocument> Find(Expression<Func<TDocument, bool>> filter)
        {
            return await _mongoCollection.Find(filter).FirstOrDefaultAsync();
            
        }

        public async Task<long> CountDocumentsAsync(Expression<Func<TDocument, bool>> filter)
        {
            var count = await _mongoCollection.CountDocumentsAsync(filter);
            return count;
        }

        public async Task InsertOneAsync(TDocument document)
        {
            await _mongoCollection.InsertOneAsync(document);
        }

        public async Task UpdateOneAsync(Expression<Func<TDocument, bool>> filter, UpdateDefinition<TDocument> update)
        {
            await _mongoCollection.UpdateOneAsync(filter,update);
        }

        public async Task DeleteOneAsync(Expression<Func<TDocument, bool>> filter)
        {
            await _mongoCollection.DeleteOneAsync(filter);
        }

        public UpdateDefinition<TDocument> UpdateSet<TField>(Expression<Func<TDocument, TField>> field, TField value)
        {
            return Builders<TDocument>.Update.Set(field, value);
        }
    }
}
