﻿global using Xunit;

namespace MongoClient.Tests;

[Collection(CollectionDefinitionKeys.MongoDb)]
public class AsyncTestBuilderPattern : BaseTestContext<MongoFixture>
{
    protected readonly IMongoService MongoService;

    public AsyncTestBuilderPattern(MongoFixture fixture)
         : base(fixture)
    {
        var mongoFactory = fixture.Services.GetService<IMongoServiceFactory>();
        MongoService = mongoFactory.GetService(TestConstants.MongoDBKey);
    }

    [Fact]
    public async Task CreatePersonAsync_Success()
    {
        //
        // Arrange
        var schema = MongoService.GetSchema<Person>();
        var p = new Person { FirstName = "BatAsync", LastName = "ManAsync", Active = true };

        //
        // Act
        await schema.InsertAsync(p);

        //
        // Assert
        var assertPerson = await schema.FindAsync(p.Id);

        Assert.NotNull(p);
        Assert.True(p.Id != ObjectId.Empty);
        Assert.Equal("BatAsync", assertPerson.FirstName);
        Assert.Equal("ManAsync", assertPerson.LastName);

        //
        // Post db cleanup
        //await ExecutePostTestCleanupAsync<Person>();
        //await Task.Delay(1500); // release a bit of pressure to the code
    }

    [Fact]
    public async Task UpsertPersonAsync_Success()
    {
        //
        // Arrange
        var schema = MongoService.GetSchema<Person>();
        var newPerson = new Person { FirstName = "TonyAsync", LastName = "StarkAsync", Active = true };

        await schema.InsertAsync(newPerson);
        var newPersonId = newPerson.Id;

        var foundPerson = await schema.FindAsync(newPerson.Id);

        foundPerson.FirstName = "ToniAsync";
        foundPerson.LastName = "StorkeAsync";
        foundPerson.Active = false;

        //
        // Act
        await schema.UpsertAsync(x => x.Id == foundPerson.Id, foundPerson);

        //
        // Assert
        var assertPerson = await schema.FindAsync(foundPerson.Id);
        Assert.NotNull(assertPerson);
        Assert.True(newPersonId == assertPerson.Id);
        Assert.Equal("ToniAsync", assertPerson.FirstName);
        Assert.Equal("StorkeAsync", assertPerson.LastName);
        Assert.False(assertPerson.Active);

        //
        // Post db cleanup
        //await ExecutePostTestCleanupAsync<Person>();
        //await Task.Delay(1500); // release a bit of pressure to the code
    }

    [Fact]
    public async Task FindPersonAsync_NotFound_Null()
    {
        //
        // Arrange
        var personSchema = MongoService.GetSchema<Person>();

        //
        // Act
        var foundPerson = await personSchema.FindAsync(MongoInitializer.NotFoundId);

        //
        // Assert
        Assert.Null(foundPerson);
    }

    [Fact]
    public async Task FindPersonAsyncWithFilterParam_Success()
    {
        //
        // Arrange
        var schema = MongoService.GetSchema<Person>();
        var p = new Person { FirstName = "Super", LastName = "Man", Active = true };
        await schema.InsertAsync(p);

        //
        // Act
        var filterDefinition = Builders<Person>.Filter.Where(p => p.FirstName.Equals("Super") && p.LastName.Equals("Man"));
        var foundPerson = await schema.FindAsync(filterDefinition);

        //
        // Assert
        Assert.NotNull(foundPerson);
        Assert.Equal("Super", foundPerson.FirstName);
        Assert.Equal("Man", foundPerson.LastName);

        //
        // Post db cleanup
        //await ExecutePostTestCleanupAsync<Person>();
        //await Task.Delay(1500); // release a bit of pressure to the code
    }

    [Fact]
    public async Task FindPersonAsyncWithFilterParam_NotFound()
    {
        //
        // Arrange
        var schema = MongoService.GetSchema<Person>();
        var p = new Person { FirstName = "Dead", LastName = "Pool", Active = true };
        await schema.InsertAsync(p);

        //
        // Act
        var filterDefinition = Builders<Person>.Filter.Where(p => p.FirstName.Equals("Dead") && p.LastName.Equals("Pooling"));
        var foundPerson = await schema.FindAsync(filterDefinition);

        //
        // Assert
        Assert.Null(foundPerson);

        //
        // Post db cleanup
        //await ExecutePostTestCleanupAsync<Person>();
        //await Task.Delay(1500); // release a bit of pressure to the code
    }

    [Fact]
    public async Task CreateUserAsync_Success()
    {
        //
        // Arrange
        var schema = MongoService.GetSchema<User>();
        var user = new User
        {
            Email = "smiggle@gmali.com",
            FirstName = "Smiggle",
            LastName = "Golum",
            Active = true
        };

        //
        // Act
        await schema.InsertAsync(user);

        //
        // Assert
        var assertPerson = await schema.FindAsync(user.Id);

        Assert.NotNull(user);
        Assert.Equal("Smiggle", assertPerson.FirstName);
        Assert.Equal("Golum", assertPerson.LastName);

        //
        // Post db cleanup
        //await ExecutePostTestCleanupAsync<Person>();
        //await Task.Delay(1500); // release a bit of pressure to the code
    }

    [Fact]
    public async Task CreateUserWithSameEmailAsync_ThrowException()
    {
        //
        // NOTE: I have set email as unique index for the mongo schema.
        // The test should throw exception - cannot create same email twice
        //

        //var schemaTypes = new List<Type>
        //    {
        //        typeof(CategorySchema),
        //        typeof(PersonSchema),
        //        typeof(UserSchema),
        //        typeof(CategoryDetailSchema)
        //    };
        //_mongoService = MongoInitializer.CreateMongoService(schemaTypes, DatabaseName);
        //_mongoService.UseCamelCase();
        //_mongoService.Connect();

        //
        // Arrange
        var schema = MongoService.GetSchema<User>();
        var user = new User
        {
            Email = "tailwind@jogimali.com",
            FirstName = "heads",
            LastName = "tail",
            Active = true
        };

        var user2 = new User
        {
            Email = "tailwind@jogimali.com",
            FirstName = "harry",
            LastName = "potter",
            Active = true
        }; // create another user with the same email

        //
        // Act
        // create the first user (new)
        await schema.InsertAsync(user);

        // create another user but with same email
        var exceptionThrown = Assert.ThrowsAsync<NautilusMongoDbException>(() => schema.InsertAsync(user2));

        //
        // Assert
        //Assert.Equal(exceptionThrown.GetType(), typeof(NautilusMongoDbException));
    }

    [Fact]
    public async Task DeleteUserAsync_Success()
    {
        //
        // Arrange
        var schema = MongoService.GetSchema<User>();
        var user = new User
        {
            Email = "jembalang@gmali.com",
            FirstName = "jembs",
            LastName = "alang",
            Active = true
        };
        await schema.InsertAsync(user);

        //
        // Act
        await schema.DeleteAsync(user.Id);

        //
        // Assert
        var assertPerson = await schema.FindAsync(user.Id);
        Assert.Null(assertPerson);

        //
        // Post db cleanup
        //await ExecutePostTestCleanupAsync<User>();
        //await Task.Delay(1500); // release a bit of pressure to the code
    }

    [Fact]
    public async Task GetManyCategoriesAsync_Success()
    {
        //
        // Arrange
        var schema = MongoService.GetSchema<Category>();

        var cat = Fixture.CreateObject("cat1", "shawn");
        await schema.InsertAsync(cat);

        cat = Fixture.CreateObject("cat2", "totot");
        await schema.InsertAsync(cat);

        cat = Fixture.CreateObject("cat3", "totot");
        await schema.InsertAsync(cat);

        cat = Fixture.CreateObject("cat4", "shawn");
        await schema.InsertAsync(cat);

        cat = Fixture.CreateObject("cat5", "shawn");
        await schema.InsertAsync(cat);

        //
        // Act
        var filterDefinition = Builders<Category>.Filter.Where(p => p.UserId.Equals("shawn"));
        var searchShawnResults = await schema.FindManyAsync(filterDefinition);

        filterDefinition = Builders<Category>.Filter.Where(p => p.UserId.Equals("totot"));
        var searchTototResults = await schema.FindManyAsync(filterDefinition);

        //
        // Assert			
        Assert.NotNull(searchShawnResults);
        Assert.Equal(3, searchShawnResults.Count());

        Assert.NotNull(searchShawnResults);
        Assert.Equal(2, searchTototResults.Count());

        //
        // Post db cleanup
        //await ExecutePostTestCleanupAsync<Category>();
        //await Task.Delay(1500); // release a bit of pressure to the code
    }
}
