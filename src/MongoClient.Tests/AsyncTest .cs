using System;
using System.Threading.Tasks;
using MongoClient.Tests.Helpers;
using MongoClient.Tests.Models;
using Nautilus.Experiment.DataProvider.Mongo;
using NUnit.Framework;

namespace MongoClient.Tests
{
	public class AsyncTest
	{
		private MongoService _mongoService;

		[OneTimeSetUp]
		public void Setup()
		{
			const string ConnectionString = "mongodb://localhost:27017";
			const string databaseName = "passKeepr";

			_mongoService = new MongoService(ConnectionString, databaseName);
			_mongoService.InitializeSchemas(new Type[] { typeof(PersonSchema) });
			_mongoService.UseCamelCase();
			_mongoService.Connect();
		}

		[Test]
		public async Task CreatePersonAsync_Success()
		{
			var personSchema = _mongoService.GetSchema<Person>();
			var p = new Person { FirstName = "Bat", LastName = "Man", Active = true };

			await MongoHelper.CreatePersonAsync(p, personSchema);

			Assert.NotNull(p.Id);
		}

		[Test]
		public async Task FindPersonAsync_NotFound_Null()
		{
			var personSchema = _mongoService.GetSchema<Person>();
			var foundPerson = await MongoHelper.FindPersonAsync(MongoHelper.NotFoundId, personSchema);

			Assert.IsNull(foundPerson);
		}
	}
}