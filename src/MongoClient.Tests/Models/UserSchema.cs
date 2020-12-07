using System;
using MongoDB.Driver;
using Nautilus.Experiment.DataProvider.Mongo.Schema;

namespace MongoClient.Tests.Models
{
	public class UserSchema : MongoBaseSchema<User>
	{
		protected UserSchema(IMongoDatabase database) : base(database)
		{
			OnCreateIndexes += async (o, e) =>
			{
				Console.WriteLine("UserSchema OnCreateIndexes called");

				await Collection.Indexes.CreateManyAsync(new[]
				{
					IndexHelper.CreateIndex<User>(nameof(User.Email)),
					IndexHelper.CreateIndex<User>(nameof(User.Active))
				});
			};
		}
	}
}