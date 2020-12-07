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
			_mongoService = MongoHelper.InitializeMongo();
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
			await schema.CreateAsync(user);

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
				Email = "nautiblaze@jogimali.com",
				FirstName = "passblaze",
				LastName = "tail",
				Active = true
			};

			var user2 = new User
			{
				Email = "nautiblaze@jogimali.com",
				FirstName = "harry",
				LastName = "potter",
				Active = true
			}; // create another user with the same email

			//
			// Act

			// create the first user (new)
			await schema.CreateAsync(user);

			// create another user but with same email
			AsyncTestDelegate action = () => schema.CreateAsync(user2);
			Assert.That(action, Throws.TypeOf<MongoWriteException>());

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