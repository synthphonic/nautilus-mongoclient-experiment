using MongoDB.Bson;
using Nautilus.Experiment.DataProvider.Mongo.Attributes;

namespace MongoClient.Tests.Models
{
    [CollectionName("Persons")]
    public class Person
    {
        public ObjectId Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public bool Active { get; set; }
        public int Age { get; set; } = 0;

        //TODO: Research on BsonDefaultValue
        //[BsonDefaultValue("ENTER")]

    }
}