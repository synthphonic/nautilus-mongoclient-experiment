using System;
using System.IO;
using System.Threading.Tasks;
using MongoClient.Tests.Helpers;
using MongoClient.Tests.Models;
using MongoClient.Tests.Models.Schema;
using MongoDB.Driver;
using Nautilus.Experiment.DataProvider.Mongo;
using Newtonsoft.Json;

namespace MongoClient.Tests.Base
{
    public class BaseTest
    {
       
        private Type[] _schemaTypes;
        protected MongoService _mongoService;

        protected string DatabaseName { get; set; }

        protected BaseTest()
        {
            _schemaTypes = new Type[]
            {
                typeof(PersonSchema),
                typeof(UserSchema),
                typeof(CategorySchema),
                typeof(RawPayloadSchema)
            };
        }

        protected virtual async Task SetupMongoDb(bool useMongoAuthentication = false)
        {
            if (!useMongoAuthentication)
                await SetupMongo_NoAuth();
            else
                await SetupMongo_WithAuth();
        }

        protected virtual async Task TearDown()
        {
            await _mongoService.DropDatabaseAsync();
            await Task.Delay(2000);
        }

        private async Task SetupMongo_NoAuth()
        {
            _mongoService = MongoInitializer.Initialize(DatabaseName, _schemaTypes);
            await Task.Delay(2000);
        }

        private async Task SetupMongo_WithAuth()
        {
            _mongoService = MongoInitializer.Initialize(DatabaseName, _schemaTypes);
            await Task.Delay(2000);
        }

        #region Helpers
        protected async Task<TModel> ReadJsonData<TModel>(string fileName)
        {
            var filePath = Path.Combine(AppContext.BaseDirectory, "_data", fileName);
            var content = await File.ReadAllTextAsync(filePath);
            var model = JsonConvert.DeserializeObject<TModel>(content);

            return model;
        }

        protected Category CreateObject(string categoryName, string userId)
        {
            var category = new Category
            {
                CategoryName = categoryName,
                UserId = userId
            };

            return category;
        }

        protected int GetRandomAge(int[] ages)
        {
            var random = new Random();
            return ages[random.Next(ages.Length)];
        }

        protected int GetRandomAge()
        {
            var ages = new[] { 5, 14, 25, 39, 43, 63 };
            var random = new Random();
            return ages[random.Next(ages.Length)];
        }

        protected FilterDefinition<TModel> CreateEmptyFilter<TModel>()
        {
            var emptyFilter = FilterDefinition<TModel>.Empty;
            return emptyFilter;
        }

        protected async Task ExecutePostTestCleanupAsync<TModel>() where TModel : class, new()
        {
            var schema = _mongoService.GetSchema<TModel>();
            await schema.DeleteManyAsync(CreateEmptyFilter<TModel>());
        }
        #endregion
    }

    
}