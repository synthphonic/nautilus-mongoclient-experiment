using System;
using System.Threading.Tasks;
using MongoDB.Driver;
using Nautilus.Experiment.DataProvider.Mongo.Schema;

namespace MongoClient.Tests.Models.Schema
{
	public class NoAttributeModelSchema : MongoBaseSchema<NoAttributeModel>
	{
		public NoAttributeModelSchema()
		{

		}

		public NoAttributeModelSchema(IMongoDatabase database) : base(database)
		{
			//OnCreateIndexes += async (o, e) =>
			//{
			//	Console.WriteLine("UserSchema OnCreateIndexes called");
			//	await CreateIndexAsync(nameof(NoAttributeModel.Email), isUnique: true);
			//};
		}

        protected override async Task CreateModelIndexesAsync()
        {
			Console.WriteLine("UserSchema OnCreateIndexes called");
			await CreateIndexAsync(nameof(NoAttributeModel.Email), isUnique: true);

		}
	}
}