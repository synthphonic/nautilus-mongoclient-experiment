global using Xunit;

namespace MongoClient.Tests;

[Collection(CollectionDefinitionKeys.MongoDb)]
public class MongoStartupConfigTestBuilderPattern : BaseTestContext<MongoFixture>
{
    protected readonly IMongoService MongoService;

    public MongoStartupConfigTestBuilderPattern(MongoFixture fixture)
         : base(fixture)
    {
        var mongoFactory = fixture.Services.GetService<IMongoServiceFactory>();
        MongoService = mongoFactory.GetService(TestConstants.MongoDBKey);
    }

    [Fact]
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

    [Fact]
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

    [Fact]
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
