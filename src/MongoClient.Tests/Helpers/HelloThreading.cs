using Nautilus.Experiment.DataProvider.Mongo;
using Nautilus.Experiment.DataProvider.Mongo.Schema;

namespace MongoClient.Tests.Helpers
{
    public class HelloThreading<TModel> where TModel : class, new()
    {
        private MongoService _mongoService;
        private MongoBaseSchema<TModel> _schema;

        public HelloThreading(MongoService mongoService)
        {
            _mongoService = mongoService;
        }

        public void Add(MongoBaseSchema<TModel> schema)
        {
            //_mongoService.GetSchema<TModel>()
        }        
    }
}