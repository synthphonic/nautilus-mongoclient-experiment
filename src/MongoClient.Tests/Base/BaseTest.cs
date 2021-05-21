using System.Threading.Tasks;
using MongoClient.Tests.Helpers;
using Nautilus.Experiment.DataProvider.Mongo;
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
    }
}