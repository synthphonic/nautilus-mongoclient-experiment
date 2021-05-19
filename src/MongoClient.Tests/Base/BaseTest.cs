using System.Threading.Tasks;
using MongoClient.Tests.Helpers;
using Nautilus.Experiment.DataProvider.Mongo;

namespace MongoClient.Tests.Base
{
    public class BaseTest
    {
        protected MongoService _mongoService;

        protected string DatabaseName { get; set; }

        protected virtual async Task OneTimeSetup()
        {
            _mongoService = MongoInitializer.Initialize(DatabaseName);
            await Task.Delay(2000);
        }

        protected virtual async Task OneTimeTearDown()
        {
            await _mongoService.DropDatabaseAsync(MongoInitializer.DatabaseName);
            await Task.Delay(2000);
        }
    }
}