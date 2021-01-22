using System;
using MongoDB.Driver;
using Nautilus.Experiment.DataProvider.Mongo.Exceptions;
using Nautilus.Experiment.DataProvider.Mongo.Schema;

namespace MongoClient.Tests.Models.Schema
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
				try
				{
					Console.WriteLine("UserSchema OnCreateIndexes called");
					await CreateIndexAsync(nameof(User.Email), isUnique: true);
					await CreateIndexAsync(nameof(User.FirstName));
				}
				catch (NautilusMongoDbException)
				{
					throw;
				}
			};
		}
	}
}