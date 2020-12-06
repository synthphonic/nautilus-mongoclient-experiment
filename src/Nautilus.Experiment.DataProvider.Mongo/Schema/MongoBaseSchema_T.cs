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
	}
}