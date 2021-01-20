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

		public void Insert(TModel model)
		{
			_collection.InsertOne(model);
		}

		public async Task InsertAsync(TModel model, InsertOneOptions options = null, CancellationToken token = default)
		{
			await _collection.InsertOneAsync(model, options, token);
		}

		public void Upsert(Expression<Func<TModel, bool>> filter, TModel model)
		{
			_collection.ReplaceOne(filter, model, new ReplaceOptions()
			{
				IsUpsert = true
			});
		}

		public async Task UpsertAsync(Expression<Func<TModel, bool>> filter, TModel model, CancellationToken token = default)
		{
			await _collection.ReplaceOneAsync(filter, model, new ReplaceOptions()
			{
				IsUpsert = true
			}, token);
		}

		public TModel Find<TField>(TField id)
		{
			var filter = Builders<TModel>.Filter.Eq<TField>("_id", id);
			var found = _collection.Find(filter).FirstOrDefault();

			return found;
		}

		public TModel Find(FilterDefinition<TModel> filterDefinition)
		{
			var found = _collection.Find(filterDefinition);

			return found.FirstOrDefault();
		}

		public IEnumerable<TModel> FindMany(FilterDefinition<TModel> filterDefinition)
		{
			var found = _collection.Find(filterDefinition).ToList();

			return found;
		}

		public async Task<IEnumerable<TModel>> FindManyAsync(FilterDefinition<TModel> filterDefinition, CancellationToken token = default)
		{
			var found = await _collection.Find(filterDefinition).ToListAsync(token);

			return found;
		}

		public async Task<TModel> FindAsync<TField>(TField id)
		{
			var filter = Builders<TModel>.Filter.Eq<TField>("_id", id);
			var found = await _collection.FindAsync(filter);

			return await found.FirstOrDefaultAsync();
		}

		public async Task<TModel> FindAsync(FilterDefinition<TModel> filterDefinition)
		{
			var found = await _collection.FindAsync(filterDefinition);

			return await found.FirstOrDefaultAsync();
		}

		public DeleteResult Delete<TField>(TField id)
		{
			//var filter = Builders<TModel>.Filter.Eq("_id", id);
			var filter = Builders<TModel>.Filter.Eq<TField>("_id", id);
			return _collection.DeleteOne(filter);
		}

		public async Task<DeleteResult> DeleteAsync<TField>(TField id)
		{
			//var filter = Builders<TModel>.Filter.Eq("_id", id);
			var filter = Builders<TModel>.Filter.Eq<TField>("_id", id);
			return await _collection.DeleteOneAsync(filter);
		}

		#region [Protected] Create Index methods
		protected async Task CreateIndexAsync(string fieldName, bool isUnique = false)
		{
			var options = new CreateIndexOptions() { Unique = isUnique };
			var field = new StringFieldDefinition<TModel>(fieldName);
			var indexDef = new IndexKeysDefinitionBuilder<TModel>().Ascending(field);

			var indexModel = new CreateIndexModel<TModel>(indexDef, options);

			try
			{
				await _collection.Indexes.CreateOneAsync(indexModel);
			}
			catch (Exception)
			{
				throw;
			}
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
	}
}