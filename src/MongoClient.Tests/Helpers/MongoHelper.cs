using System;
using MongoClient.Tests.Models;
using MongoDB.Bson;
using Nautilus.Experiment.DataProvider.Mongo;

namespace MongoClient.Tests.Helpers
{
	internal static class MongoHelper
	{
		const string ConnectionString = "mongodb://localhost:27017";
		const string databaseName = "passKeepr";
		public static ObjectId NotFoundId = new ObjectId("2fcd299f9e1d7d949562d108");

		internal static MongoService InitializeMongo()
		{
			var mongoService = new MongoService(ConnectionString, databaseName);
			mongoService.InitializeSchemas(new Type[]
			{
				typeof(PersonSchema),
				typeof(UserSchema)
			});

			mongoService.UseCamelCase();
			mongoService.Connect();

			return mongoService;
		}
	}
}