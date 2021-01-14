using System;
using MongoDB.Driver;
using Nautilus.Experiment.DataProvider.Mongo.Schema;

namespace MongoClient.Tests.Models
{
	public class NoAttributeModelSchema : MongoBaseSchema<NoAttributeModel>
	{
		public NoAttributeModelSchema(IMongoDatabase database) : base(database)
		{
			OnCreateIndexes += async (o, e) =>
			{
				Console.WriteLine("UserSchema OnCreateIndexes called");
				await CreateIndexAsync(nameof(NoAttributeModel.Email), isUnique: true);
			};
		}
	}
}