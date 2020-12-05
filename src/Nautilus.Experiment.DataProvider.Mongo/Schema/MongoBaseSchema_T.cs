using System;
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
			Console.WriteLine("Inserting record");
			_collection.InsertOne(record);
			Console.WriteLine("Insert done");
		}
	}
}