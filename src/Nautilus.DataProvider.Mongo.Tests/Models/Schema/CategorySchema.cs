namespace MongoClient.Tests.Models.Schema;

public class CategorySchema : MongoBaseSchema<Category>
{
    public CategorySchema()
    {

    }

    public CategorySchema(IMongoDatabase database) : base(database)
    {
        //OnCreateIndexes += async (o, e) =>
        //{
        //	await CreateIndexAsync(nameof(Category.CategoryName));
        //};
    }

    protected override async Task CreateModelIndexesAsync()
    {
        await CreateIndexAsync(nameof(Category.CategoryName));
    }
}
