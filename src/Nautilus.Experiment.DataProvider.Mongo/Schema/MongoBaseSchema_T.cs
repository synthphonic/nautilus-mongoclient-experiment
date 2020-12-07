/*
 * References
 *	https://stackoverflow.com/questions/6218966/creating-mongodb-unique-key-with-c-sharp
 *	https://dotnetcodr.com/2016/06/03/introduction-to-mongodb-with-net-part-39-working-with-indexes-in-the-mongodb-net-driver/
 * 
 */

using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Nautilus.Experiment.DataProvider.Mongo.Schema
{
	public class MongoBaseSchema<TModel> : MongoBaseSchema where TModel : class, new()
	{
		private IMongoDatabase _database;
		private IMongoCollection<TModel> _collection;

		protected MongoBaseSchema(IMongoDatabase database)
		{
			_database = database;

			var t = new TModel();

			TableNameFullCSharp = t.GetType().FullName;
			TableNameCSharp = t.GetType().Name;
			TableNameMongo = t.GetType().Name.ToLower();

			_collection = _database.GetCollection<TModel>(TableNameMongo);
		}

		public void Create(TModel model)
		{
			_collection.InsertOne(model);
		}

		public async Task CreateAsync(TModel model)
		{
			await _collection.InsertOneAsync(model);
		}

		public void Upsert(Expression<Func<TModel, bool>> filter, TModel model)
		{
			_collection.ReplaceOne(filter, model, new ReplaceOptions()
			{
				IsUpsert = true
			});
		}

		public async Task UpsertAsync(Expression<Func<TModel, bool>> filter, TModel model)
		{
			await _collection.ReplaceOneAsync(filter, model, new ReplaceOptions()
			{
				IsUpsert = true
			});
		}

		public async Task InsertRecordAsync(TModel record, InsertOneOptions options = null, CancellationToken token = default)
		{
			await _collection.InsertOneAsync(record, options, token);
		}

		public TModel Find(ObjectId id)
		{
			var filter = Builders<TModel>.Filter.Eq("_id", id);
			var found = _collection.Find(filter).FirstOrDefault();

			return found;
		}

		public async Task<TModel> FindAsync(ObjectId id)
		{
			var filter = Builders<TModel>.Filter.Eq("_id", id);
			var found = await _collection.FindAsync(filter);

			return await found.FirstOrDefaultAsync();
		}

		#region [Protected] Create Index methods
		protected async Task CreateIndexAsync(string fieldName, bool isUnique = false)
		{
			var options = new CreateIndexOptions() { Unique = isUnique };
			var field = new StringFieldDefinition<TModel>(fieldName);
			var indexDef = new IndexKeysDefinitionBuilder<TModel>().Ascending(field);

			var indexModel = new CreateIndexModel<TModel>(indexDef, options);

			await _collection.Indexes.CreateOneAsync(indexModel);
		}

		protected async Task CreateIndexAsync(string fieldName, CreateIndexOptions options)
		{
			var field = new StringFieldDefinition<TModel>(fieldName);
			var indexDef = new IndexKeysDefinitionBuilder<TModel>().Ascending(field);

			var indexModel = new CreateIndexModel<TModel>(indexDef, options);

			await _collection.Indexes.CreateOneAsync(indexModel);
		}

		protected void CreateIndex(string fieldName, bool isUnique = false)
		{
			var options = new CreateIndexOptions() { Unique = isUnique };
			var field = new StringFieldDefinition<TModel>(fieldName);
			var indexDef = new IndexKeysDefinitionBuilder<TModel>().Ascending(field);

			var indexModel = new CreateIndexModel<TModel>(indexDef, options);

			_collection.Indexes.CreateOne(indexModel);
		}

		protected void CreateIndex(string fieldName, CreateIndexOptions options)
		{
			var field = new StringFieldDefinition<TModel>(fieldName);
			var indexDef = new IndexKeysDefinitionBuilder<TModel>().Ascending(field);

			var indexModel = new CreateIndexModel<TModel>(indexDef, options);

			_collection.Indexes.CreateOne(indexModel);
		}
		#endregion

		#region Properties
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

	}
}