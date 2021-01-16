using System;
using MongoDB.Driver;
using Nautilus.Experiment.DataProvider.Mongo.Schema;

namespace MongoClient.Tests.Models
{
	public class CategoryDetailSchema : MongoBaseSchema<CategoryDetail>
	{
		public CategoryDetailSchema()
		{

		}

		public CategoryDetailSchema(IMongoDatabase database) : base(database)
		{
			OnCreateIndexes += async (o, e) =>
			{
				Console.WriteLine("UserSchema OnCreateIndexes called");
				await CreateIndexAsync(nameof(CategoryDetail.CategoryName), isUnique: true);
			};
		}
	}
}