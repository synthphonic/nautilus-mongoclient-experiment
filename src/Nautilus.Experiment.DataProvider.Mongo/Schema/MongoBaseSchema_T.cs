/*
 * References
 *	https://stackoverflow.com/questions/6218966/creating-mongodb-unique-key-with-c-sharp
 *	https://dotnetcodr.com/2016/06/03/introduction-to-mongodb-with-net-part-39-working-with-indexes-in-the-mongodb-net-driver/
 * 
 */

using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Nautilus.Experiment.DataProvider.Mongo.Schema
{
	public class MongoBaseSchema<T> : MongoBaseSchema where T : class, new()
	{
		private IMongoDatabase _database;
		private IMongoCollection<T> _collection;
		
		protected MongoBaseSchema(IMongoDatabase database)
		{
			_database = database;

			var t = new T();

			TableNameFullCSharp = t.GetType().FullName;
			TableNameCSharp = t.GetType().Name;
			TableNameMongo = t.GetType().Name.ToLower();

			_collection = _database.GetCollection<T>(TableNameMongo);
		}

		public void InsertRecord(T record)
		{
			_collection.InsertOne(record);
		}

		public async Task InsertRecordAsync(T record, InsertOneOptions options = null, CancellationToken token = default)
		{
			await _collection.InsertOneAsync(record, options, token);
		}

		public T Find(ObjectId id)
		{
			var filter = Builders<T>.Filter.Eq("_id", id);
			var found = _collection.Find(filter).FirstOrDefault();

			return found;
		}

		public async Task<T> FindAsync(ObjectId id)
		{
			var filter = Builders<T>.Filter.Eq("_id", id);
			var found = await _collection.FindAsync(filter);

			return await found.FirstOrDefaultAsync();
		}

		/// <summary>
		/// For advance collection funcionalities, we use this property instead for now.
		/// </summary>
		public IMongoCollection<T> Collection
		{
			get
			{
				return _collection;
			}
		}
	}
}