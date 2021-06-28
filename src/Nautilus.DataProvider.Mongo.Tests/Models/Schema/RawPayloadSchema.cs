using System.Threading.Tasks;
using MongoDB.Driver;
using Nautilus.Experiment.DataProvider.Mongo.Schema;

namespace MongoClient.Tests.Models.Schema
{
    public class RawPayloadSchema : MongoBaseSchema<RawPayloadModel>
    {
        public RawPayloadSchema()
        {

        }

        public RawPayloadSchema(IMongoDatabase database) : base(database)
        {
        }

        protected override async Task CreateModelIndexesAsync()
        {
            await CreateIndexAsync(nameof(RawPayloadModel.Id), isUnique: true);
        }
    }
}