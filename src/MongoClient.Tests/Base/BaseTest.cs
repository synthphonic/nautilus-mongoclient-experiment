using System;
using System.Threading.Tasks;
using MongoClient.Tests.Helpers;
using MongoClient.Tests.Models;
using MongoDB.Driver;
using Nautilus.Experiment.DataProvider.Mongo;
using Nautilus.Experiment.DataProvider.Mongo.Schema;
using Nautilus.Tests.Core.Configuration;

namespace MongoClient.Tests.Base
{
    public class BaseTest
    {
        private Config _config;
        protected MongoService _mongoService;

        protected string DatabaseName { get; set; }

        protected virtual async Task SetupMongoDb(bool useMongoAuthentication = false)
        {
            if (!useMongoAuthentication)
                await SetupMongo_NoAuth();
            else
                await SetupMongo_WithAuth();
        }

        protected virtual async Task TearDown()
        {
            await _mongoService.DropDatabaseAsync(MongoInitializer.DatabaseName);
            await Task.Delay(2000);
        }

        private async Task SetupMongo_NoAuth()
        {
            _mongoService = MongoInitializer.Initialize(DatabaseName);
            await Task.Delay(2000);
        }

        private async Task SetupMongo_WithAuth()
        {
            _config = new Config();
            _config.Initialize();

            //var appSettingSection = Configuration.GetSection("AppSettings");
            //_passKeeprAppSettings = appSettingSection.Get<AppSettings>();
            //services.AddSingleton(_passKeeprAppSettings);

            _mongoService = MongoInitializer.Initialize(DatabaseName);
            await Task.Delay(2000);
        }

        #region Helpers
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