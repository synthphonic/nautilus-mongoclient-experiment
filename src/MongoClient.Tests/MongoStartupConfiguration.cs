using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoClient.Tests.Helpers;
using MongoClient.Tests.Models;
using MongoClient.Tests.Models.Schema;
using Nautilus.Experiment.DataProvider.Mongo;
using NUnit.Framework;

namespace MongoClient.Tests
{
	public class MongoStartupConfiguration
	{
		private const string DatabaseName = "configuration_test_db";
		private MongoService _mongoService;

		[SetUp]
		public void Setup()
		{

		}

		[TearDown]
		public async Task TearDownOneTime()
		{
			await _mongoService.DropDatabaseAsync(DatabaseName);
			await Task.Delay(2000);
		}

		[Test]
		public void Use_CollectionNameAttribute_Conventions()
		{
			#region Arrange
			var schemaTypes = new List<Type>
				{
					typeof(UserSchema),
					typeof(CategorySchema),
					typeof(PersonSchema),
					typeof(CategoryDetailSchema)
				};

			_mongoService = MongoInitializer.CreateMongoService(schemaTypes, DatabaseName);
			_mongoService.Connect();
			#endregion

			#region Act
			var userSchema = _mongoService.GetSchema<User>();
			var personSchema = _mongoService.GetSchema<Person>();
			var categorySchema = _mongoService.GetSchema<Category>();
			var categoryDetailSchema = _mongoService.GetSchema<CategoryDetail>();
			#endregion

			#region Assert
			Assert.NotNull(userSchema);
			Assert.NotNull(personSchema);
			Assert.NotNull(categorySchema);
			Assert.NotNull(categoryDetailSchema);
			#endregion
		}

		[Test]
		public void Use_CamelCasing_Property_Conventions()
		{
			#region Arrange
			var schemaTypes = new List<Type>
				{
					typeof(CategorySchema),
					typeof(PersonSchema),
					typeof(UserSchema),
					typeof(CategoryDetailSchema)
				};

			_mongoService = MongoInitializer.CreateMongoService(schemaTypes, DatabaseName);
			_mongoService.UseCamelCase();
			_mongoService.Connect();

			#endregion

			#region Act
			var userSchema = _mongoService.GetSchema<User>();
			var personSchema = _mongoService.GetSchema<Person>();
			var categorySchema = _mongoService.GetSchema<Category>();
			var categoryDetailSchema = _mongoService.GetSchema<CategoryDetail>();
			#endregion

			#region Assert
			Assert.NotNull(userSchema);
			Assert.NotNull(personSchema);
			Assert.NotNull(categorySchema);
			Assert.NotNull(categoryDetailSchema);
			#endregion
		}

		[Test]
		public void Use_CollectionNameAttribute_And_NoAttribute_Conventions()
		{
			#region Arrange
			var schemaTypes = new List<Type>
				{
					typeof(CategorySchema),
					typeof(PersonSchema),
					typeof(UserSchema),
					typeof(CategoryDetailSchema),
					typeof(NoAttributeModelSchema),
				};

			_mongoService = MongoInitializer.CreateMongoService(schemaTypes, DatabaseName);
			_mongoService.Connect();
			#endregion

			#region Act
			var userSchema = _mongoService.GetSchema<User>();
			var personSchema = _mongoService.GetSchema<Person>();
			var categorySchema = _mongoService.GetSchema<Category>();
			var categoryDetailSchema = _mongoService.GetSchema<CategoryDetail>();
			var noAttributeSchema = _mongoService.GetSchema<NoAttributeModel>();
			#endregion

			#region Assert
			Assert.NotNull(userSchema);
			Assert.NotNull(personSchema);
			Assert.NotNull(categorySchema);
			Assert.NotNull(categoryDetailSchema);
			Assert.NotNull(noAttributeSchema);
			#endregion
		}
	}
}
