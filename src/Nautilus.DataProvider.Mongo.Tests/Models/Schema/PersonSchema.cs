/*
 * References
 *	https://stackoverflow.com/questions/6218966/creating-mongodb-unique-key-with-c-sharp
 *	https://dotnetcodr.com/2016/06/03/introduction-to-mongodb-with-net-part-39-working-with-indexes-in-the-mongodb-net-driver/
 * 
 */
namespace MongoClient.Tests.Models.Schema;

public class PersonSchema : MongoBaseSchema<Person>
{
    public PersonSchema()
    {

    }

    public PersonSchema(IMongoDatabase database) : base(database)
    {
        //OnCreateIndexes += async (o, e) =>
        //{
        //	Console.WriteLine("PersonSchema OnCreateIndexes called");

        //	await CreateIndexAsync(nameof(Person.FirstName));

        //	//
        //	// NOTE: this is how to get the index list for a particular collection
        //	var a = Collection.Indexes.List().ToList();
        //};
    }

    protected override async Task CreateModelIndexesAsync()
    {
        Console.WriteLine("PersonSchema OnCreateIndexes called");

        await CreateIndexAsync(nameof(Person.FirstName));

        //
        // NOTE: this is how to get the index list for a particular collection
        //var a = Collection.Indexes.List().ToList();
    }
}
