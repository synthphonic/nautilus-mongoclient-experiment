using System;
using MongoDB.Driver;
using Nautilus.Experiment.DataProvider.Mongo.Schema;

namespace MongoClient.Tests.Models
{
	public class UserSchema : MongoBaseSchema<User>
	{
		public UserSchema() : base()
		{

		}

		public UserSchema(IMongoDatabase database) : base(database)
		{
			OnCreateIndexes += async (o, e) =>
			{
				Console.WriteLine("UserSchema OnCreateIndexes called");
				await CreateIndexAsync(nameof(User.Email), isUnique: true);
				await CreateIndexAsync(nameof(User.FirstName));
			};
		}
	}
}