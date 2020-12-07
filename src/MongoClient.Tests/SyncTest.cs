using MongoClient.Tests.Helpers;
using MongoClient.Tests.Models;
using MongoDB.Driver;
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
			_mongoService = MongoHelper.InitializeMongo();
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

		[Test]
		public void CreateUser_Success()
		{
			//
			// Arrange
			var schema = _mongoService.GetSchema<User>();
			var user = new User
			{
				Email = "helloworld@gmali.com",
				FirstName = "BatSync",
				LastName = "ManSync",
				Active = true
			};

			//
			// Act
			schema.Create(user);

			//
			// Assert
			var assertPerson = schema.Find(user.Id);

			Assert.NotNull(user);
			Assert.AreEqual("BatSync", assertPerson.FirstName);
			Assert.AreEqual("ManSync", assertPerson.LastName);
		}

		[Test]
		public void CreateUserWithSameEmail_ThrowException()
		{
			//
			// NOTE: I have set email as unique index for the mongo schema.
			// The test should throw exception - cannot create same email twice
			//

			//
			// Arrange
			var schema = _mongoService.GetSchema<User>();
			var user = new User
			{
				Email = "tailwind@jogimali.com",
				FirstName = "heads",
				LastName = "tail",
				Active = true
			};

			var user2 = new User
			{
				Email = "tailwind@jogimali.com",
				FirstName = "harry",
				LastName = "potter",
				Active = true
			}; // create another user with the same email

			//
			// Act

			// create the first user (new)
			schema.Create(user); 

			// create another user but with same email
			var exceptionThrown = Assert.Throws<MongoWriteException>(()=>schema.Create(user2));
			 
			//
			// Assert
			//Assert.Throws()
			//var assertPerson = schema.Find(user.Id);

			//Assert.NotNull(user);
			//Assert.AreEqual("BatSync", assertPerson.FirstName);
			//Assert.AreEqual("ManSync", assertPerson.LastName);
		}
	}
}