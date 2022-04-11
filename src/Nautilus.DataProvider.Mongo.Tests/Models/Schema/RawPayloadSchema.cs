namespace MongoClient.Tests.Models.Schema;

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
