using System;
using System.Linq;
using System.Threading.Tasks;
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
			_mongoService = MongoInitializer.Initialize();
		}

		[OneTimeTearDown]
		public async Task TearDownOneTime()
		{
			await _mongoService.DropDatabaseAsync(MongoInitializer.DatabaseName);
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
			var assertPerson = schema.Find(foundPerson.Id);
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
			var foundPerson = personSchema.Find(MongoInitializer.NotFoundId);

			//
			// Assert
			Assert.IsNull(foundPerson);
		}

		[Test]
		public void FindPersonWithFilterParam_Success()
		{
			//
			// Arrange
			var schema = _mongoService.GetSchema<Person>();
			var p = new Person { FirstName = "Wonder", LastName = "Woman", Active = true };
			schema.Create(p);

			//
			// Act
			var filterDefinition = Builders<Person>.Filter.Where(p => p.FirstName.Equals("Wonder") && p.LastName.Equals("Woman"));
			var foundPerson = schema.Find(filterDefinition);

			//
			// Assert
			Assert.NotNull(foundPerson);
			Assert.AreEqual("Wonder", foundPerson.FirstName);
			Assert.AreEqual("Woman", foundPerson.LastName);
		}

		[Test]
		public void FindPersonWithFilterParam_NotFound()
		{
			//
			// Arrange
			var schema = _mongoService.GetSchema<Person>();
			var p = new Person { FirstName = "Spider", LastName = "Man", Active = true };
			schema.Create(p);

			//
			// Act
			var filterDefinition = Builders<Person>.Filter.Where(p => p.FirstName.Equals("Spider") && p.LastName.Equals("Woman"));
			var foundPerson = schema.Find(filterDefinition);

			//
			// Assert
			Assert.Null(foundPerson);
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
			var exceptionThrown = Assert.Throws<MongoWriteException>(() => schema.Create(user2));

			//
			// Assert
			Assert.AreEqual(exceptionThrown.GetType(), typeof(MongoWriteException));
		}

		[Test]
		public void DeleteUser_Success()
		{
			//
			// Arrange
			var schema = _mongoService.GetSchema<User>();
			var user = new User
			{
				Email = "jembalang@gmali.com",
				FirstName = "jembs",
				LastName = "alang",
				Active = true
			};
			schema.Create(user);

			//
			// Act
			schema.Delete(user.Id);

			//
			// Assert
			var assertPerson = schema.Find(user.Id);
			Assert.Null(assertPerson);
		}

		[Test]
		public void GetManyCategories_Success()
		{
			//
			// Arrange
			var schema = _mongoService.GetSchema<Category>();

			var cat = CategoryFactoryHelper.CreateObject("cat1", "shawn");
			schema.Create(cat);

			cat = CategoryFactoryHelper.CreateObject("cat2", "totot");
			schema.Create(cat);

			cat = CategoryFactoryHelper.CreateObject("cat3", "totot");
			schema.Create(cat);

			cat = CategoryFactoryHelper.CreateObject("cat4", "shawn");
			schema.Create(cat);

			cat = CategoryFactoryHelper.CreateObject("cat5", "shawn");
			schema.Create(cat);

			//
			// Act
			var filterDefinition = Builders<Category>.Filter.Where(p => p.UserId.Equals("shawn"));
			var searchShawnResults = schema.FindMany(filterDefinition);

			filterDefinition = Builders<Category>.Filter.Where(p => p.UserId.Equals("totot"));
			var searchTototResults = schema.FindMany(filterDefinition);

			//
			// Assert			
			Assert.NotNull(searchShawnResults);
			Assert.AreEqual(3, searchShawnResults.Count());

			Assert.NotNull(searchShawnResults);
			Assert.AreEqual(2, searchTototResults.Count());
		}
	}
}