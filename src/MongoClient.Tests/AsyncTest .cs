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
			//
			// Arrange
			var schema = _mongoService.GetSchema<Person>();
			var newPerson = new Person { FirstName = "TonyAsync", LastName = "StarkAsync", Active = true };

			await schema.CreateAsync(newPerson);
			var newPersonId = newPerson.Id;

			var foundPerson = await schema.FindAsync(newPerson.Id);

			foundPerson.FirstName = "ToniAsync";
			foundPerson.LastName = "StorkeAsync";
			foundPerson.Active = false;

			//
			// Act
			await schema.UpsertAsync(x => x.Id == foundPerson.Id, foundPerson);

			//
			// Assert
			var assertPerson = await schema.FindAsync(foundPerson.Id);
			Assert.NotNull(assertPerson);
			Assert.True(newPersonId == assertPerson.Id);
			Assert.AreEqual("ToniAsync", assertPerson.FirstName);
			Assert.AreEqual("StorkeAsync", assertPerson.LastName);
			Assert.AreEqual(false, assertPerson.Active);
		}

		[Test]
		public async Task CreatePersonAsync_Success()
		{
			//
			// Arrange
			var schema = _mongoService.GetSchema<Person>();
			var p = new Person { FirstName = "BatAsync", LastName = "ManAsync", Active = true };

			//
			// Act
			await schema.CreateAsync(p);

			//
			// Assert
			var assertPerson = await schema.FindAsync(p.Id);

			Assert.NotNull(p);
			Assert.AreEqual("BatAsync", assertPerson.FirstName);
			Assert.AreEqual("ManAsync", assertPerson.LastName);
		}

		[Test]
		public async Task FindPersonAsync_NotFound_Null()
		{
			//
			// Arrange
			var personSchema = _mongoService.GetSchema<Person>();

			//
			// Act
			var foundPerson = await personSchema.FindAsync(MongoHelper.NotFoundId);

			//
			// Assert
			Assert.IsNull(foundPerson);
		}
	}
}