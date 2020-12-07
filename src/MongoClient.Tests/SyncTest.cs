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
			//
			// Arrange
			var schema = _mongoService.GetSchema<Person>();
			var newPerson = new Person { FirstName = "TonySync", LastName = "StarkSync", Active = true };

			schema.Create(newPerson);
			var newPersonId = newPerson.Id;

			var foundPerson = schema.Find(newPerson.Id);

			foundPerson.FirstName = "ToniSync";
			foundPerson.LastName = "StorkeSync";
			foundPerson.Active = false;

			//
			// Act
			schema.Upsert(x => x.Id == foundPerson.Id, foundPerson);

			//
			// Assert
			var assertPerson =  schema.Find(foundPerson.Id);
			Assert.NotNull(assertPerson);
			Assert.True(newPersonId == assertPerson.Id);
			Assert.AreEqual("ToniSync", assertPerson.FirstName);
			Assert.AreEqual("StorkeSync", assertPerson.LastName);
			Assert.AreEqual(false, assertPerson.Active);
		}

		[Test]
		public void CreatePerson_Success()
		{
			//
			// Arrange
			var schema = _mongoService.GetSchema<Person>();
			var p = new Person { FirstName = "BatSync", LastName = "ManSync", Active = true };

			//
			// Act
			schema.Create(p);

			//
			// Assert
			var assertPerson = schema.Find(p.Id);

			Assert.NotNull(p);
			Assert.AreEqual("BatSync", assertPerson.FirstName);
			Assert.AreEqual("ManSync", assertPerson.LastName);
		}

		[Test]
		public void FindPerson_NotFound_Null()
		{
			//
			// Arrange
			var personSchema = _mongoService.GetSchema<Person>();

			//
			// Act
			var foundPerson = personSchema.Find(MongoHelper.NotFoundId);

			//
			// Assert
			Assert.IsNull(foundPerson);
		}
	}
}