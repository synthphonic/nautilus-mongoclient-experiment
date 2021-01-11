using System.Linq;
using System.Threading.Tasks;
using MongoClient.Tests.Helpers;
using MongoClient.Tests.Models;
using MongoDB.Driver;
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
			_mongoService = MongoInitializer.Initialize();
		}

		[OneTimeTearDown]
		public async Task TearDownOneTime()
		{
			await _mongoService.DropDatabaseAsync(MongoInitializer.DatabaseName);
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
			await schema.InsertAsync(p);

			//
			// Assert
			var assertPerson = await schema.FindAsync(p.Id);

			Assert.NotNull(p);
			Assert.AreEqual("BatAsync", assertPerson.FirstName);
			Assert.AreEqual("ManAsync", assertPerson.LastName);
		}

		[Test]
		public async Task UpsertPersonAsync_Success()
		{
			//
			// Arrange
			var schema = _mongoService.GetSchema<Person>();
			var newPerson = new Person { FirstName = "TonyAsync", LastName = "StarkAsync", Active = true };

			await schema.InsertAsync(newPerson);
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
		public async Task FindPersonAsync_NotFound_Null()
		{
			//
			// Arrange
			var personSchema = _mongoService.GetSchema<Person>();

			//
			// Act
			var foundPerson = await personSchema.FindAsync(MongoInitializer.NotFoundId);

			//
			// Assert
			Assert.IsNull(foundPerson);
		}

		[Test]
		public async Task FindPersonAsyncWithFilterParam_Success()
		{
			//
			// Arrange
			var schema = _mongoService.GetSchema<Person>();
			var p = new Person { FirstName = "Super", LastName = "Man", Active = true };
			await schema.InsertAsync(p);

			//
			// Act
			var filterDefinition = Builders<Person>.Filter.Where(p => p.FirstName.Equals("Super") && p.LastName.Equals("Man"));
			var foundPerson = await schema.FindAsync(filterDefinition);

			//
			// Assert
			Assert.NotNull(foundPerson);
			Assert.AreEqual("Super", foundPerson.FirstName);
			Assert.AreEqual("Man", foundPerson.LastName);
		}

		[Test]
		public async Task FindPersonAsyncWithFilterParam_NotFound()
		{
			//
			// Arrange
			var schema = _mongoService.GetSchema<Person>();
			var p = new Person { FirstName = "Dead", LastName = "Pool", Active = true };
			await schema.InsertAsync(p);

			//
			// Act
			var filterDefinition = Builders<Person>.Filter.Where(p => p.FirstName.Equals("Dead") && p.LastName.Equals("Pooling"));
			var foundPerson = await schema.FindAsync(filterDefinition);

			//
			// Assert
			Assert.Null(foundPerson);
		}

		[Test]
		public async Task CreateUserAsync_Success()
		{
			//
			// Arrange
			var schema = _mongoService.GetSchema<User>();
			var user = new User
			{
				Email = "smiggle@gmali.com",
				FirstName = "Smiggle",
				LastName = "Golum",
				Active = true
			};

			//
			// Act
			await schema.InsertAsync(user);

			//
			// Assert
			var assertPerson = schema.Find(user.Id);

			Assert.NotNull(user);
			Assert.AreEqual("Smiggle", assertPerson.FirstName);
			Assert.AreEqual("Golum", assertPerson.LastName);
		}

		[Test]
		public async Task CreateUserWithSameEmailAsync_ThrowException()
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
			await schema.InsertAsync(user);

			// create another user but with same email
			var exceptionThrown = Assert.ThrowsAsync<MongoWriteException>(() => schema.InsertAsync(user2));

			//
			// Assert
			Assert.AreEqual(exceptionThrown.GetType(), typeof(MongoWriteException));
		}

		[Test]
		public async Task DeleteUserAsync_Success()
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
			await schema.InsertAsync(user);

			//
			// Act
			await schema.DeleteAsync(user.Id);

			//
			// Assert
			var assertPerson = await schema.FindAsync(user.Id);
			Assert.Null(assertPerson);
		}

		[Test]
		public async Task GetManyCategoriesAsync_Success()
		{
			//
			// Arrange
			var schema = _mongoService.GetSchema<Category>();

			var cat = CategoryFactoryHelper.CreateObject("cat1", "shawn");
			schema.InsertAsync(cat);

			cat = CategoryFactoryHelper.CreateObject("cat2", "totot");
			schema.Insert(cat);

			cat = CategoryFactoryHelper.CreateObject("cat3", "totot");
			schema.Insert(cat);

			cat = CategoryFactoryHelper.CreateObject("cat4", "shawn");
			schema.Insert(cat);

			cat = CategoryFactoryHelper.CreateObject("cat5", "shawn");
			schema.Insert(cat);

			//
			// Act
			var filterDefinition = Builders<Category>.Filter.Where(p => p.UserId.Equals("shawn"));
			var searchShawnResults = await schema.FindManyAsync(filterDefinition);

			filterDefinition = Builders<Category>.Filter.Where(p => p.UserId.Equals("totot"));
			var searchTototResults = await schema.FindManyAsync(filterDefinition);

			//
			// Assert			
			Assert.NotNull(searchShawnResults);
			Assert.AreEqual(3, searchShawnResults.Count());

			Assert.NotNull(searchShawnResults);
			Assert.AreEqual(2, searchTototResults.Count());
		}
	}
}