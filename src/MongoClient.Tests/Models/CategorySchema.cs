using MongoDB.Driver;
using Nautilus.Experiment.DataProvider.Mongo.Schema;

namespace MongoClient.Tests.Models
{
	public class CategorySchema : MongoBaseSchema<Category>
	{
		public CategorySchema(IMongoDatabase database) : base(database)
		{
			OnCreateIndexes += async (o, e) =>
			{
				await CreateIndexAsync(nameof(Category.CategoryName));
			};
		}
	}
}