using System;
using MongoClient.Tests.Helpers;
using MongoClient.Tests.Models;
using Nautilus.Experiment.DataProvider.Mongo;
using NUnit.Framework;

namespace MongoClient.Tests
{
	public class SyncTest
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
		public void UpsertPerson_Success()
		{
			var schema = _mongoService.GetSchema<Person>();
			var newPerson = new Person { FirstName = "Tony", LastName = "Stark", Active = true };

			schema.Create(newPerson);

			var foundPerson = schema.Find(newPerson.Id);

			foundPerson.FirstName = "Toni";
			foundPerson.Active = false;

			schema.Upsert(x => x.Id == foundPerson.Id, foundPerson);
		}

		[Test]
		public void CreatePerson_Success()
		{
			var personSchema = _mongoService.GetSchema<Person>();
			var p = new Person { FirstName = "Bat", LastName = "Man", Active = true };

			personSchema.Create(p);

			Assert.NotNull(p.Id);
		}

		[Test]
		public void FindPerson_NotFound_Null()
		{
			var personSchema = _mongoService.GetSchema<Person>();
			var foundPerson = personSchema.Find(MongoHelper.NotFoundId);

			Assert.IsNull(foundPerson);
		}
	}
}