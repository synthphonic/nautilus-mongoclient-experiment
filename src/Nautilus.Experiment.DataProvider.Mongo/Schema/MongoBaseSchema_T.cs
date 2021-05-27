/*
 * References
 *	https://stackoverflow.com/questions/6218966/creating-mongodb-unique-key-with-c-sharp
 *	https://dotnetcodr.com/2016/06/03/introduction-to-mongodb-with-net-part-39-working-with-indexes-in-the-mongodb-net-driver/
 * 
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using Nautilus.Experiment.DataProvider.Mongo.Attributes;
using Nautilus.Experiment.DataProvider.Mongo.Exceptions;

namespace Nautilus.Experiment.DataProvider.Mongo.Schema
{
    public class MongoBaseSchema<TModel> : MongoBaseSchema where TModel : class, new()
    {
        private IMongoDatabase _database;
        private IMongoCollection<TModel> _collection;

        public MongoBaseSchema()
        {
            var t = new TModel();
            ModelType = t.GetType();
        }

        public MongoBaseSchema(IMongoDatabase database)
        {
            _database = database;

            var t = new TModel();

            TableNameFullCSharp = t.GetType().FullName;
            TableNameCSharp = t.GetType().Name;
            TableNameMongo = t.GetType().Name.ToLower();
            ModelType = t.GetType();

            var foundAttributes = t.GetType().GetCustomAttributes(typeof(CollectionName), false);
            if (foundAttributes != null && foundAttributes.Count() == 1)
            {
                var attrib = foundAttributes[0] as CollectionName;
                TableNameMongo = attrib.Name;
            }

            _collection = _database.GetCollection<TModel>(TableNameMongo);
        }

        public async Task InsertAsync(TModel model, InsertOneOptions options = null, CancellationToken token = default)
        {
            try
            {
                await _collection.InsertOneAsync(model, options, token);
            }
            catch (MongoAuthenticationException mongoAuthEx)
            {
                throw new NautilusMongoDbException("Mongo security error", mongoAuthEx);
            }
            catch (MongoConnectionException mongoConnectEx)
            {
                throw new NautilusMongoDbException(mongoConnectEx.Message, mongoConnectEx);
            }
            catch (MongoWriteException mongoWriteEx)
            {
                throw new NautilusMongoDbException("Mongo write error", mongoWriteEx);
            }
            catch (MongoCommandException mongoCmdEx)
            {
                throw new NautilusMongoDbException("Mongo command error", mongoCmdEx);
            }
            catch (TimeoutException timeoutEx)
            {
                throw new NautilusMongoDbException("Mongo has timed out", timeoutEx);
            }
            catch (Exception ex)
            {
                throw new NautilusMongoDbException("Mongo throws a general exception", ex);
            }
        }

        public async Task UpsertAsync(Expression<Func<TModel, bool>> filter, TModel model, CancellationToken token = default)
        {
            try
            {
                await _collection.ReplaceOneAsync(filter, model, new ReplaceOptions()
                {
                    IsUpsert = true
                }, token);
            }
            catch (MongoAuthenticationException mongoAuthEx)
            {
                throw new NautilusMongoDbException("Mongo security error", mongoAuthEx);
            }
            catch (MongoConnectionException mongoConnectEx)
            {
                throw new NautilusMongoDbException(mongoConnectEx.Message, mongoConnectEx);
            }
            catch (MongoWriteException mongoWriteEx)
            {
                throw new NautilusMongoDbException("Mongo write error", mongoWriteEx);
            }
            catch (MongoCommandException mongoCmdEx)
            {
                throw new NautilusMongoDbException("Mongo command error", mongoCmdEx);
            }
            catch (TimeoutException timeoutEx)
            {
                throw new NautilusMongoDbException("Mongo has timed out", timeoutEx);
            }
            catch (Exception ex)
            {
                throw new NautilusMongoDbException("Mongo throws a general exception", ex);
            }
        }

        public async Task UpdateOneAsync(FilterDefinition<TModel> filter, UpdateDefinition<TModel> update, UpdateOptions options = null, CancellationToken token = default)
        {
            try
            {
                await _collection.UpdateOneAsync(filter, update, options, token);
            }
            catch (MongoAuthenticationException mongoAuthEx)
            {
                throw new NautilusMongoDbException("Mongo security error", mongoAuthEx);
            }
            catch (MongoConnectionException mongoConnectEx)
            {
                throw new NautilusMongoDbException(mongoConnectEx.Message, mongoConnectEx);
            }
            catch (MongoWriteException mongoWriteEx)
            {
                throw new NautilusMongoDbException("Mongo write error", mongoWriteEx);
            }
            catch (MongoCommandException mongoCmdEx)
            {
                throw new NautilusMongoDbException("Mongo command error", mongoCmdEx);
            }
            catch (TimeoutException timeoutEx)
            {
                throw new NautilusMongoDbException("Mongo has timed out", timeoutEx);
            }
            catch (Exception ex)
            {
                throw new NautilusMongoDbException("Mongo throws a general exception", ex);
            }
        }

        public async Task<IEnumerable<TModel>> FindManyAsync(FilterDefinition<TModel> filterDefinition, CancellationToken token = default)
        {
            try
            {
                var found = await _collection.Find(filterDefinition).ToListAsync(token);

                return found;
            }
            catch (MongoAuthenticationException mongoAuthEx)
            {
                throw new NautilusMongoDbException("Mongo security error", mongoAuthEx);
            }
            catch (MongoConnectionException mongoConnectEx)
            {
                throw new NautilusMongoDbException(mongoConnectEx.Message, mongoConnectEx);
            }
            catch (MongoWriteException mongoWriteEx)
            {
                throw new NautilusMongoDbException("Mongo write error", mongoWriteEx);
            }
            catch (MongoCommandException mongoCmdEx)
            {
                throw new NautilusMongoDbException("Mongo command error", mongoCmdEx);
            }
            catch (TimeoutException timeoutEx)
            {
                throw new NautilusMongoDbException("Mongo has timed out", timeoutEx);
            }
            catch (Exception ex)
            {
                throw new NautilusMongoDbException("Mongo throws a general exception", ex);
            }
        }

        public async Task<TModel> FindAsync<TField>(TField id)
        {
            try
            {
                var filter = Builders<TModel>.Filter.Eq<TField>("_id", id);
                var found = await _collection.FindAsync(filter);

                return await found.FirstOrDefaultAsync();
            }
            catch (MongoAuthenticationException mongoAuthEx)
            {
                throw new NautilusMongoDbException("Mongo security error", mongoAuthEx);
            }
            catch (MongoConnectionException mongoConnectEx)
            {
                throw new NautilusMongoDbException(mongoConnectEx.Message, mongoConnectEx);
            }
            catch (MongoWriteException mongoWriteEx)
            {
                throw new NautilusMongoDbException("Mongo write error", mongoWriteEx);
            }
            catch (MongoCommandException mongoCmdEx)
            {
                throw new NautilusMongoDbException("Mongo command error", mongoCmdEx);
            }
            catch (TimeoutException timeoutEx)
            {
                throw new NautilusMongoDbException("Mongo has timed out", timeoutEx);
            }
            catch (Exception ex)
            {
                throw new NautilusMongoDbException("Mongo throws a general exception", ex);
            }
        }

        public async Task<TModel> FindAsync(FilterDefinition<TModel> filterDefinition)
        {
            try
            {
                var found = await _collection.FindAsync(filterDefinition);

                return await found.FirstOrDefaultAsync();
            }
            catch (MongoAuthenticationException mongoAuthEx)
            {
                throw new NautilusMongoDbException("Mongo security error", mongoAuthEx);
            }
            catch (MongoConnectionException mongoConnectEx)
            {
                throw new NautilusMongoDbException(mongoConnectEx.Message, mongoConnectEx);
            }
            catch (MongoWriteException mongoWriteEx)
            {
                throw new NautilusMongoDbException("Mongo write error", mongoWriteEx);
            }
            catch (MongoCommandException mongoCmdEx)
            {
                throw new NautilusMongoDbException("Mongo command error", mongoCmdEx);
            }
            catch (TimeoutException timeoutEx)
            {
                throw new NautilusMongoDbException("Mongo has timed out", timeoutEx);
            }
            catch (Exception ex)
            {
                throw new NautilusMongoDbException("Mongo throws a general exception", ex);
            }
        }

        public async Task<DeleteResult> DeleteAsync<TField>(TField id)
        {
            try
            {
                var filter = Builders<TModel>.Filter.Eq<TField>("_id", id);
                return await _collection.DeleteOneAsync(filter);
            }
            catch (MongoAuthenticationException mongoAuthEx)
            {
                throw new NautilusMongoDbException("Mongo security error", mongoAuthEx);
            }
            catch (MongoConnectionException mongoConnectEx)
            {
                throw new NautilusMongoDbException(mongoConnectEx.Message, mongoConnectEx);
            }
            catch (MongoWriteException mongoWriteEx)
            {
                throw new NautilusMongoDbException("Mongo write error", mongoWriteEx);
            }
            catch (MongoCommandException mongoCmdEx)
            {
                throw new NautilusMongoDbException("Mongo command error", mongoCmdEx);
            }
            catch (TimeoutException timeoutEx)
            {
                throw new NautilusMongoDbException("Mongo has timed out", timeoutEx);
            }
            catch (Exception ex)
            {
                throw new NautilusMongoDbException("Mongo throws a general exception", ex);
            }
        }

        public async Task<DeleteResult> DeleteManyAsync(FilterDefinition<TModel> filter)
        {
            try
            {
                return await _collection.DeleteManyAsync(filter);
            }
            catch (MongoAuthenticationException mongoAuthEx)
            {
                throw new NautilusMongoDbException("Mongo security error", mongoAuthEx);
            }
            catch (MongoConnectionException mongoConnectEx)
            {
                throw new NautilusMongoDbException(mongoConnectEx.Message, mongoConnectEx);
            }
            catch (MongoWriteException mongoWriteEx)
            {
                throw new NautilusMongoDbException("Mongo write error", mongoWriteEx);
            }
            catch (MongoCommandException mongoCmdEx)
            {
                throw new NautilusMongoDbException("Mongo command error", mongoCmdEx);
            }
            catch (TimeoutException timeoutEx)
            {
                throw new NautilusMongoDbException("Mongo has timed out", timeoutEx);
            }
            catch (Exception ex)
            {
                throw new NautilusMongoDbException("Mongo throws a general exception", ex);
            }
        }

        #region Bulk operations
        public BulkWriteResult<TModel> BulkWrite(
            IEnumerable<WriteModel<TModel>> requests,
            BulkWriteOptions options = null,
            CancellationToken token = default)
        {
            try
            {
                return _collection.BulkWrite(requests, options, token);
            }
            catch (MongoAuthenticationException mongoAuthEx)
            {
                throw new NautilusMongoDbException("Mongo security error", mongoAuthEx);
            }
            catch (MongoConnectionException mongoConnectEx)
            {
                throw new NautilusMongoDbException(mongoConnectEx.Message, mongoConnectEx);
            }
            catch (MongoWriteException mongoWriteEx)
            {
                throw new NautilusMongoDbException("Mongo write error", mongoWriteEx);
            }
            catch (MongoCommandException mongoCmdEx)
            {
                throw new NautilusMongoDbException("Mongo command error", mongoCmdEx);
            }
            catch (TimeoutException timeoutEx)
            {
                throw new NautilusMongoDbException("Mongo has timed out", timeoutEx);
            }
            catch (Exception ex)
            {
                throw new NautilusMongoDbException("Mongo throws a general exception", ex);
            }
        }

        public async Task<BulkWriteResult<TModel>> BulkWriteAsync(
            IEnumerable<WriteModel<TModel>> requests,
            BulkWriteOptions options = null,
            CancellationToken token = default)
        {
            try
            {
                return await _collection.BulkWriteAsync(requests, options, token);
            }
            catch (MongoAuthenticationException mongoAuthEx)
            {
                throw new NautilusMongoDbException("Mongo security error", mongoAuthEx);
            }
            catch (MongoConnectionException mongoConnectEx)
            {
                throw new NautilusMongoDbException(mongoConnectEx.Message, mongoConnectEx);
            }
            catch (MongoWriteException mongoWriteEx)
            {
                throw new NautilusMongoDbException("Mongo write error", mongoWriteEx);
            }
            catch (MongoCommandException mongoCmdEx)
            {
                throw new NautilusMongoDbException("Mongo command error", mongoCmdEx);
            }
            catch (TimeoutException timeoutEx)
            {
                throw new NautilusMongoDbException("Mongo has timed out", timeoutEx);
            }
            catch (Exception ex)
            {
                throw new NautilusMongoDbException("Mongo throws a general exception", ex);
            }
        }

        public async Task<BulkWriteResult<TModel>> BulkWriteAsync(
            IClientSessionHandle session,
            IEnumerable<WriteModel<TModel>> requests,
            BulkWriteOptions options = null,
            CancellationToken token = default)
        {
            try
            {
                return await _collection.BulkWriteAsync(session, requests, options, token);
            }
            catch (MongoAuthenticationException mongoAuthEx)
            {
                throw new NautilusMongoDbException("Mongo security error", mongoAuthEx);
            }
            catch (MongoConnectionException mongoConnectEx)
            {
                throw new NautilusMongoDbException(mongoConnectEx.Message, mongoConnectEx);
            }
            catch (MongoWriteException mongoWriteEx)
            {
                throw new NautilusMongoDbException("Mongo write error", mongoWriteEx);
            }
            catch (MongoCommandException mongoCmdEx)
            {
                throw new NautilusMongoDbException("Mongo command error", mongoCmdEx);
            }
            catch (TimeoutException timeoutEx)
            {
                throw new NautilusMongoDbException("Mongo has timed out", timeoutEx);
            }
            catch (Exception ex)
            {
                throw new NautilusMongoDbException("Mongo throws a general exception", ex);
            }
        }

        public async Task BulkInsertAsync(IEnumerable<TModel> models, IClientSessionHandle session, InsertManyOptions options = null, CancellationToken token = default)
        {
            try
            {
                await _collection.InsertManyAsync(session, models, options, token);
            }
            catch (MongoAuthenticationException mongoAuthEx)
            {
                throw new NautilusMongoDbException("Mongo security error", mongoAuthEx);
            }
            catch (MongoConnectionException mongoConnectEx)
            {
                throw new NautilusMongoDbException(mongoConnectEx.Message, mongoConnectEx);
            }
            catch (MongoWriteException mongoWriteEx)
            {
                throw new NautilusMongoDbException("Mongo write error", mongoWriteEx);
            }
            catch (MongoCommandException mongoCmdEx)
            {
                throw new NautilusMongoDbException("Mongo command error", mongoCmdEx);
            }
            catch (TimeoutException timeoutEx)
            {
                throw new NautilusMongoDbException("Mongo has timed out", timeoutEx);
            }
            catch (Exception ex)
            {
                throw new NautilusMongoDbException("Mongo throws a general exception", ex);
            }
        }

        public async Task BulkInsertAsync(IEnumerable<TModel> models, InsertManyOptions options = null, CancellationToken token = default)
        {
            try
            {
                await _collection.InsertManyAsync(models, options, token);
            }
            catch (MongoAuthenticationException mongoAuthEx)
            {
                throw new NautilusMongoDbException("Mongo security error", mongoAuthEx);
            }
            catch (MongoConnectionException mongoConnectEx)
            {
                throw new NautilusMongoDbException(mongoConnectEx.Message, mongoConnectEx);
            }
            catch (MongoWriteException mongoWriteEx)
            {
                throw new NautilusMongoDbException("Mongo write error", mongoWriteEx);
            }
            catch (MongoCommandException mongoCmdEx)
            {
                throw new NautilusMongoDbException("Mongo command error", mongoCmdEx);
            }
            catch (TimeoutException timeoutEx)
            {
                throw new NautilusMongoDbException("Mongo has timed out", timeoutEx);
            }
            catch (Exception ex)
            {
                throw new NautilusMongoDbException("Mongo throws a general exception", ex);
            }
        }

        public async Task BulkUpdateAsync(FilterDefinition<TModel> filter, UpdateDefinition<TModel> update, UpdateOptions options = null, CancellationToken token = default)
        {
            try
            {
                await _collection.UpdateManyAsync(filter, update, options, token);
            }
            catch (MongoAuthenticationException mongoAuthEx)
            {
                throw new NautilusMongoDbException("Mongo security error", mongoAuthEx);
            }
            catch (MongoConnectionException mongoConnectEx)
            {
                throw new NautilusMongoDbException(mongoConnectEx.Message, mongoConnectEx);
            }
            catch (MongoWriteException mongoWriteEx)
            {
                throw new NautilusMongoDbException("Mongo write error", mongoWriteEx);
            }
            catch (MongoCommandException mongoCmdEx)
            {
                throw new NautilusMongoDbException("Mongo command error", mongoCmdEx);
            }
            catch (TimeoutException timeoutEx)
            {
                throw new NautilusMongoDbException("Mongo has timed out", timeoutEx);
            }
            catch (Exception ex)
            {
                throw new NautilusMongoDbException("Mongo throws a general exception", ex);
            }
        }

        public async Task BulkUpdateAsync(Expression<Func<TModel, bool>> filter, UpdateDefinition<TModel> update, UpdateOptions options = null, CancellationToken token = default)
        {
            try
            {
                await _collection.UpdateManyAsync(filter, update, options, token);
            }
            catch (MongoAuthenticationException mongoAuthEx)
            {
                throw new NautilusMongoDbException("Mongo security error", mongoAuthEx);
            }
            catch (MongoConnectionException mongoConnectEx)
            {
                throw new NautilusMongoDbException(mongoConnectEx.Message, mongoConnectEx);
            }
            catch (MongoWriteException mongoWriteEx)
            {
                throw new NautilusMongoDbException("Mongo write error", mongoWriteEx);
            }
            catch (MongoCommandException mongoCmdEx)
            {
                throw new NautilusMongoDbException("Mongo command error", mongoCmdEx);
            }
            catch (TimeoutException timeoutEx)
            {
                throw new NautilusMongoDbException("Mongo has timed out", timeoutEx);
            }
            catch (Exception ex)
            {
                throw new NautilusMongoDbException("Mongo throws a general exception", ex);
            }
        }

        public async Task<DeleteResult> BulkDeleteAsync(FilterDefinition<TModel> filter, CancellationToken token = default)
        {
            try
            {
                return await _collection.DeleteManyAsync(filter, token);
            }
            catch (MongoAuthenticationException mongoAuthEx)
            {
                throw new NautilusMongoDbException("Mongo security error", mongoAuthEx);
            }
            catch (MongoConnectionException mongoConnectEx)
            {
                throw new NautilusMongoDbException(mongoConnectEx.Message, mongoConnectEx);
            }
            catch (MongoWriteException mongoWriteEx)
            {
                throw new NautilusMongoDbException("Mongo write error", mongoWriteEx);
            }
            catch (MongoCommandException mongoCmdEx)
            {
                throw new NautilusMongoDbException("Mongo command error", mongoCmdEx);
            }
            catch (TimeoutException timeoutEx)
            {
                throw new NautilusMongoDbException("Mongo has timed out", timeoutEx);
            }
            catch (Exception ex)
            {
                throw new NautilusMongoDbException("Mongo throws a general exception", ex);
            }
        }

        public async Task<DeleteResult> BulkDeleteAsync(FilterDefinition<TModel> filter, DeleteOptions options, CancellationToken token = default)
        {
            try
            {
                return await _collection.DeleteManyAsync(filter, options, token);
            }
            catch (MongoAuthenticationException mongoAuthEx)
            {
                throw new NautilusMongoDbException("Mongo security error", mongoAuthEx);
            }
            catch (MongoConnectionException mongoConnectEx)
            {
                throw new NautilusMongoDbException(mongoConnectEx.Message, mongoConnectEx);
            }
            catch (MongoWriteException mongoWriteEx)
            {
                throw new NautilusMongoDbException("Mongo write error", mongoWriteEx);
            }
            catch (MongoCommandException mongoCmdEx)
            {
                throw new NautilusMongoDbException("Mongo command error", mongoCmdEx);
            }
            catch (TimeoutException timeoutEx)
            {
                throw new NautilusMongoDbException("Mongo has timed out", timeoutEx);
            }
            catch (Exception ex)
            {
                throw new NautilusMongoDbException("Mongo throws a general exception", ex);
            }
        }

        public async Task<DeleteResult> BulkDeleteAsync(IClientSessionHandle session, FilterDefinition<TModel> filter, DeleteOptions options, CancellationToken token = default)
        {
            try
            {
                return await _collection.DeleteManyAsync(session, filter, options, token);
            }
            catch (MongoAuthenticationException mongoAuthEx)
            {
                throw new NautilusMongoDbException("Mongo security error", mongoAuthEx);
            }
            catch (MongoConnectionException mongoConnectEx)
            {
                throw new NautilusMongoDbException(mongoConnectEx.Message, mongoConnectEx);
            }
            catch (MongoWriteException mongoWriteEx)
            {
                throw new NautilusMongoDbException("Mongo write error", mongoWriteEx);
            }
            catch (MongoCommandException mongoCmdEx)
            {
                throw new NautilusMongoDbException("Mongo command error", mongoCmdEx);
            }
            catch (TimeoutException timeoutEx)
            {
                throw new NautilusMongoDbException("Mongo has timed out", timeoutEx);
            }
            catch (Exception ex)
            {
                throw new NautilusMongoDbException("Mongo throws a general exception", ex);
            }
        }

        public async Task<DeleteResult> BulkDeleteAsync(Expression<Func<TModel, bool>> filter, CancellationToken token = default)
        {
            try
            {
                return await _collection.DeleteManyAsync(filter, token);
            }
            catch (MongoAuthenticationException mongoAuthEx)
            {
                throw new NautilusMongoDbException("Mongo security error", mongoAuthEx);
            }
            catch (MongoConnectionException mongoConnectEx)
            {
                throw new NautilusMongoDbException(mongoConnectEx.Message, mongoConnectEx);
            }
            catch (MongoWriteException mongoWriteEx)
            {
                throw new NautilusMongoDbException("Mongo write error", mongoWriteEx);
            }
            catch (MongoCommandException mongoCmdEx)
            {
                throw new NautilusMongoDbException("Mongo command error", mongoCmdEx);
            }
            catch (TimeoutException timeoutEx)
            {
                throw new NautilusMongoDbException("Mongo has timed out", timeoutEx);
            }
            catch (Exception ex)
            {
                throw new NautilusMongoDbException("Mongo throws a general exception", ex);
            }
        }

        public async Task<DeleteResult> BulkDeleteAsync(Expression<Func<TModel, bool>> filter, DeleteOptions options, CancellationToken token = default)
        {
            try
            {
                return await _collection.DeleteManyAsync(filter, options, token);
            }
            catch (MongoAuthenticationException mongoAuthEx)
            {
                throw new NautilusMongoDbException("Mongo security error", mongoAuthEx);
            }
            catch (MongoConnectionException mongoConnectEx)
            {
                throw new NautilusMongoDbException(mongoConnectEx.Message, mongoConnectEx);
            }
            catch (MongoWriteException mongoWriteEx)
            {
                throw new NautilusMongoDbException("Mongo write error", mongoWriteEx);
            }
            catch (MongoCommandException mongoCmdEx)
            {
                throw new NautilusMongoDbException("Mongo command error", mongoCmdEx);
            }
            catch (TimeoutException timeoutEx)
            {
                throw new NautilusMongoDbException("Mongo has timed out", timeoutEx);
            }
            catch (Exception ex)
            {
                throw new NautilusMongoDbException("Mongo throws a general exception", ex);
            }
        }

        public async Task<DeleteResult> BulkDeleteAsync(IClientSessionHandle session, Expression<Func<TModel, bool>> filter, DeleteOptions options, CancellationToken token = default)
        {
            try
            {
                return await _collection.DeleteManyAsync(session, filter, options, token);
            }
            catch (MongoAuthenticationException mongoAuthEx)
            {
                throw new NautilusMongoDbException("Mongo security error", mongoAuthEx);
            }
            catch (MongoConnectionException mongoConnectEx)
            {
                throw new NautilusMongoDbException(mongoConnectEx.Message, mongoConnectEx);
            }
            catch (MongoWriteException mongoWriteEx)
            {
                throw new NautilusMongoDbException("Mongo write error", mongoWriteEx);
            }
            catch (MongoCommandException mongoCmdEx)
            {
                throw new NautilusMongoDbException("Mongo command error", mongoCmdEx);
            }
            catch (TimeoutException timeoutEx)
            {
                throw new NautilusMongoDbException("Mongo has timed out", timeoutEx);
            }
            catch (Exception ex)
            {
                throw new NautilusMongoDbException("Mongo throws a general exception", ex);
            }
        }
        #endregion

        #region [Protected] Create Index methods
        protected async Task CreateIndexAsync(string fieldName, bool isUnique = false)
        {
            try
            {
                var options = new CreateIndexOptions() { Unique = isUnique };
                var field = new StringFieldDefinition<TModel>(fieldName);
                var indexDef = new IndexKeysDefinitionBuilder<TModel>().Ascending(field);

                var indexModel = new CreateIndexModel<TModel>(indexDef, options);

                await _collection.Indexes.CreateOneAsync(indexModel);
            }
            catch (MongoAuthenticationException mongoAuthEx)
            {
                throw new NautilusMongoDbException("Mongo security error", mongoAuthEx);
            }
            catch (MongoConnectionException mongoConnectEx)
            {
                throw new NautilusMongoDbException(mongoConnectEx.Message, mongoConnectEx);
            }
            catch (MongoWriteException mongoWriteEx)
            {
                throw new NautilusMongoDbException("Mongo write error", mongoWriteEx);
            }
            catch (MongoCommandException mongoCmdEx)
            {
                throw new NautilusMongoDbException("Mongo command error", mongoCmdEx);
            }
            catch (TimeoutException timeoutEx)
            {
                throw new NautilusMongoDbException("Mongo has timed out", timeoutEx);
            }
            catch (Exception ex)
            {
                throw new NautilusMongoDbException("Mongo throws a general exception", ex);
            }
        }

        protected async Task CreateIndexAsync(string fieldName, CreateIndexOptions options)
        {
            try
            {
                var field = new StringFieldDefinition<TModel>(fieldName);
                var indexDef = new IndexKeysDefinitionBuilder<TModel>().Ascending(field);

                var indexModel = new CreateIndexModel<TModel>(indexDef, options);

                await _collection.Indexes.CreateOneAsync(indexModel);
            }
            catch (MongoAuthenticationException mongoAuthEx)
            {
                throw new NautilusMongoDbException("Mongo security error", mongoAuthEx);
            }
            catch (MongoConnectionException mongoConnectEx)
            {
                throw new NautilusMongoDbException(mongoConnectEx.Message, mongoConnectEx);
            }
            catch (MongoWriteException mongoWriteEx)
            {
                throw new NautilusMongoDbException("Mongo write error", mongoWriteEx);
            }
            catch (MongoCommandException mongoCmdEx)
            {
                throw new NautilusMongoDbException("Mongo command error", mongoCmdEx);
            }
            catch (TimeoutException timeoutEx)
            {
                throw new NautilusMongoDbException("Mongo has timed out", timeoutEx);
            }
            catch (Exception ex)
            {
                throw new NautilusMongoDbException("Mongo throws a general exception", ex);
            }
        }
        #endregion

        #region Properties
        public TModel Model { get; private set; }

        /// <summary>
        /// For advance collection funcionalities, we use this property instead for now.
        /// </summary>
        public IMongoCollection<TModel> Collection
        {
            get
            {
                return _collection;
            }
        }
        #endregion

        #region Obsoleted methods
        [Obsolete("Use the asynchronous method instead", true)]
        public void Insert(TModel model)
        {
            try
            {
                _collection.InsertOne(model);
            }
            catch (MongoAuthenticationException mongoAuthEx)
            {
                throw new NautilusMongoDbException("Mongo security error", mongoAuthEx);
            }
            catch (MongoConnectionException mongoConnectEx)
            {
                throw new NautilusMongoDbException(mongoConnectEx.Message, mongoConnectEx);
            }
            catch (MongoWriteException mongoWriteEx)
            {
                throw new NautilusMongoDbException("Mongo write error", mongoWriteEx);
            }
            catch (MongoCommandException mongoCmdEx)
            {
                throw new NautilusMongoDbException("Mongo command error", mongoCmdEx);
            }
            catch (TimeoutException timeoutEx)
            {
                throw new NautilusMongoDbException("Mongo has timed out", timeoutEx);
            }
            catch (Exception ex)
            {
                throw new NautilusMongoDbException("Mongo throws a general exception", ex);
            }
        }

        [Obsolete("Use the asynchronous method instead", true)]
        public void Upsert(Expression<Func<TModel, bool>> filter, TModel model)
        {
            try
            {
                _collection.ReplaceOne(filter, model, new ReplaceOptions()
                {
                    IsUpsert = true
                });
            }
            catch (MongoConnectionException mongoConnectEx)
            {
                throw new NautilusMongoDbException(mongoConnectEx.Message, mongoConnectEx);
            }
            catch (MongoWriteException mongoWriteEx)
            {
                throw new NautilusMongoDbException("Mongo write error", mongoWriteEx);
            }
            catch (MongoCommandException mongoCmdEx)
            {
                throw new NautilusMongoDbException("Mongo command error", mongoCmdEx);
            }
            catch (TimeoutException timeoutEx)
            {
                throw new NautilusMongoDbException("Mongo has timed out", timeoutEx);
            }
            catch (Exception ex)
            {
                throw new NautilusMongoDbException("Mongo throws a general exception", ex);
            }
        }

        [Obsolete("Use the asynchronous method instead", true)]
        public DeleteResult Delete<TField>(TField id)
        {
            try
            {
                var filter = Builders<TModel>.Filter.Eq<TField>("_id", id);
                return _collection.DeleteOne(filter);
            }
            catch (MongoConnectionException mongoConnectEx)
            {
                throw new NautilusMongoDbException(mongoConnectEx.Message, mongoConnectEx);
            }
            catch (MongoWriteException mongoWriteEx)
            {
                throw new NautilusMongoDbException("Mongo write error", mongoWriteEx);
            }
            catch (MongoCommandException mongoCmdEx)
            {
                throw new NautilusMongoDbException("Mongo command error", mongoCmdEx);
            }
            catch (TimeoutException timeoutEx)
            {
                throw new NautilusMongoDbException("Mongo has timed out", timeoutEx);
            }
            catch (Exception ex)
            {
                throw new NautilusMongoDbException("Mongo throws a general exception", ex);
            }
        }

        [Obsolete("Use the asynchronous method instead", true)]
        protected void CreateIndex(string fieldName, bool isUnique = false)
        {
            var options = new CreateIndexOptions() { Unique = isUnique };
            var field = new StringFieldDefinition<TModel>(fieldName);
            var indexDef = new IndexKeysDefinitionBuilder<TModel>().Ascending(field);

            var indexModel = new CreateIndexModel<TModel>(indexDef, options);

            _collection.Indexes.CreateOne(indexModel);
        }

        [Obsolete("Use the asynchronous method instead", true)]
        protected void CreateIndex(string fieldName, CreateIndexOptions options)
        {
            var field = new StringFieldDefinition<TModel>(fieldName);
            var indexDef = new IndexKeysDefinitionBuilder<TModel>().Ascending(field);

            var indexModel = new CreateIndexModel<TModel>(indexDef, options);

            _collection.Indexes.CreateOne(indexModel);
        }

        [Obsolete("Use the asynchronous method instead", true)]
        public TModel Find<TField>(TField id)
        {
            try
            {
                var filter = Builders<TModel>.Filter.Eq<TField>("_id", id);
                var found = _collection.Find(filter).FirstOrDefault();

                return found;
            }
            catch (MongoConnectionException mongoConnectEx)
            {
                throw new NautilusMongoDbException(mongoConnectEx.Message, mongoConnectEx);
            }
            catch (MongoWriteException mongoWriteEx)
            {
                throw new NautilusMongoDbException("Mongo write error", mongoWriteEx);
            }
            catch (MongoCommandException mongoCmdEx)
            {
                throw new NautilusMongoDbException("Mongo command error", mongoCmdEx);
            }
            catch (TimeoutException timeoutEx)
            {
                throw new NautilusMongoDbException("Mongo has timed out", timeoutEx);
            }
            catch (Exception ex)
            {
                throw new NautilusMongoDbException("Mongo throws a general exception", ex);
            }
        }

        [Obsolete("Use the asynchronous method instead", true)]
        public TModel Find(FilterDefinition<TModel> filterDefinition)
        {
            try
            {
                var found = _collection.Find(filterDefinition);

                return found.FirstOrDefault();
            }
            catch (MongoConnectionException mongoConnectEx)
            {
                throw new NautilusMongoDbException(mongoConnectEx.Message, mongoConnectEx);
            }
            catch (MongoWriteException mongoWriteEx)
            {
                throw new NautilusMongoDbException("Mongo write error", mongoWriteEx);
            }
            catch (MongoCommandException mongoCmdEx)
            {
                throw new NautilusMongoDbException("Mongo command error", mongoCmdEx);
            }
            catch (TimeoutException timeoutEx)
            {
                throw new NautilusMongoDbException("Mongo has timed out", timeoutEx);
            }
            catch (Exception ex)
            {
                throw new NautilusMongoDbException("Mongo throws a general exception", ex);
            }
        }

        [Obsolete("Use the asynchronous method instead", true)]
        public IEnumerable<TModel> FindMany(FilterDefinition<TModel> filterDefinition)
        {
            try
            {
                var found = _collection.Find(filterDefinition).ToList();

                return found;
            }
            catch (MongoAuthenticationException mongoAuthEx)
            {
                throw new NautilusMongoDbException("Mongo security error", mongoAuthEx);
            }
            catch (MongoConnectionException mongoConnectEx)
            {
                throw new NautilusMongoDbException(mongoConnectEx.Message, mongoConnectEx);
            }
            catch (MongoWriteException mongoWriteEx)
            {
                throw new NautilusMongoDbException("Mongo write error", mongoWriteEx);
            }
            catch (MongoCommandException mongoCmdEx)
            {
                throw new NautilusMongoDbException("Mongo command error", mongoCmdEx);
            }
            catch (TimeoutException timeoutEx)
            {
                throw new NautilusMongoDbException("Mongo has timed out", timeoutEx);
            }
            catch (Exception ex)
            {
                throw new NautilusMongoDbException("Mongo throws a general exception", ex);
            }
        }
        #endregion

    }
}