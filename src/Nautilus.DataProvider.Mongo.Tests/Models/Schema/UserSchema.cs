namespace MongoClient.Tests.Models.Schema;

public class UserSchema : MongoBaseSchema<User>
{
    public UserSchema() : base()
    {

    }

    public UserSchema(IMongoDatabase database) : base(database)
    {
        //OnCreateIndexes += async (o, e) =>
        //{
        //	try
        //	{
        //		Console.WriteLine("UserSchema OnCreateIndexes called");
        //		await CreateIndexAsync(nameof(User.Email), isUnique: true);
        //		await CreateIndexAsync(nameof(User.FirstName));
        //	}
        //	catch (MongoCommandException mongoCmdEx)
        //             {
        //		throw new NautilusMongoDbException("Mongo command error", mongoCmdEx);

        //	}
        //	catch (NautilusMongoDbException)
        //	{
        //		throw;
        //	}
        //};
    }

    protected override async Task CreateModelIndexesAsync()
    {
        try
        {
            Console.WriteLine("UserSchema OnCreateIndexes called");
            await CreateIndexAsync(nameof(User.Email), isUnique: true);
            await CreateIndexAsync(nameof(User.FirstName));
        }
        catch (MongoCommandException mongoCmdEx)
        {
            throw new NautilusMongoDbException("Mongo command error", mongoCmdEx);
        }
        catch (NautilusMongoDbException)
        {
            throw;
        }
    }
}
