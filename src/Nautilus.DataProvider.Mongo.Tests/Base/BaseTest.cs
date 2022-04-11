namespace MongoClient.Tests.Base;

public class BaseTest
{
    private const int Delay = 3000;
    private Type[] _schemaTypes;
    protected MongoService MongoService;

    protected string DatabaseName { get; set; }

    protected BaseTest()
    {
        _schemaTypes = new Type[]
        {
            typeof(PersonSchema),
            typeof(UserSchema),
            typeof(CategorySchema),
            typeof(CategoryDetailSchema),
            typeof(RawPayloadSchema),
            typeof(NoAttributeModelSchema)
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
        await MongoService.DropDatabaseAsync();
        await Task.Delay(Delay);
    }

    private async Task SetupMongo_NoAuth()
    {
        MongoService = MongoInitializer.Initialize(DatabaseName, _schemaTypes);
        await Task.Delay(Delay);
    }

    private async Task SetupMongo_WithAuth()
    {
        MongoService = MongoInitializer.Initialize(DatabaseName, _schemaTypes);
        await Task.Delay(Delay);
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
        var schema = MongoService.GetSchema<TModel>();
        await schema.DeleteManyAsync(CreateEmptyFilter<TModel>());
    }
    #endregion
}
