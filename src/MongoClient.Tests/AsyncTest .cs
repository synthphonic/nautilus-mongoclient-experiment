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
		public async Task UpsertPersonAsync_Success()
		{
			var schema = _mongoService.GetSchema<Person>();
			var newPerson = new Person { FirstName = "Tony", LastName = "Stark", Active = true };

			await schema.CreateAsync(newPerson);

			var foundPerson = await schema.FindAsync(newPerson.Id);

			foundPerson.FirstName = "Toni";
			foundPerson.Active = false;

			await schema.UpsertAsync(x => x.Id == foundPerson.Id, foundPerson);
		}

		[Test]
		public async Task CreatePersonAsync_Success()
		{
			var personSchema = _mongoService.GetSchema<Person>();
			var p = new Person { FirstName = "Bat", LastName = "Man", Active = true };

			await personSchema.CreateAsync(p);

			Assert.NotNull(p.Id);
		}

		[Test]
		public async Task FindPersonAsync_NotFound_Null()
		{
			var personSchema = _mongoService.GetSchema<Person>();
			var foundPerson = await personSchema.FindAsync(MongoHelper.NotFoundId);

			Assert.IsNull(foundPerson);
		}
	}
}