using System.Threading.Tasks;
using MongoClient.Tests.Base;
using MongoClient.Tests.Models;
using NUnit.Framework;

namespace MongoClient.Tests
{
    public class MongoStartupConfigTest : BaseTest
    {
        [OneTimeSetUp]
        public async Task Setup()
        {
            DatabaseName = "configuration_test_db";

            await SetupMongoDb();
            //await TearDownOneTime();
        }

        [OneTimeTearDown]
        public async Task TearDownOneTime()
        {
            await base.TearDown();
        }

        [Test]
        public void Use_CollectionNameAttribute_Conventions()
        {
            #region Arrange
            //var schemaTypes = new List<Type>
            //    {
            //        typeof(UserSchema),
            //        typeof(CategorySchema),
            //        typeof(PersonSchema),
            //        typeof(CategoryDetailSchema)
            //    };

            //MongoService = MongoInitializer.CreateMongoService(schemaTypes, DatabaseName);
            //MongoService.Connect();
            #endregion

            #region Act
            var userSchema = MongoService.GetSchema<User>();
            var personSchema = MongoService.GetSchema<Person>();
            var categorySchema = MongoService.GetSchema<Category>();
            var categoryDetailSchema = MongoService.GetSchema<CategoryDetail>();
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
            //var schemaTypes = new List<Type>
            //    {
            //        typeof(CategorySchema),
            //        typeof(PersonSchema),
            //        typeof(UserSchema),
            //        typeof(CategoryDetailSchema)
            //    };

            //MongoService = MongoInitializer.CreateMongoService(schemaTypes, DatabaseName);
            //MongoService.UseCamelCase();
            //MongoService.Connect();
            #endregion

            #region Act
            var userSchema = MongoService.GetSchema<User>();
            var personSchema = MongoService.GetSchema<Person>();
            var categorySchema = MongoService.GetSchema<Category>();
            var categoryDetailSchema = MongoService.GetSchema<CategoryDetail>();
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
            //var schemaTypes = new List<Type>
            //    {
            //        typeof(CategorySchema),
            //        typeof(PersonSchema),
            //        typeof(UserSchema),
            //        typeof(CategoryDetailSchema),
            //        typeof(NoAttributeModelSchema),
            //    };

            //MongoService = MongoInitializer.CreateMongoService(schemaTypes, DatabaseName);
            //MongoService.Connect();
            #endregion

            #region Act
            var userSchema = MongoService.GetSchema<User>();
            var personSchema = MongoService.GetSchema<Person>();
            var categorySchema = MongoService.GetSchema<Category>();
            var categoryDetailSchema = MongoService.GetSchema<CategoryDetail>();
            var noAttributeSchema = MongoService.GetSchema<NoAttributeModel>();
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
