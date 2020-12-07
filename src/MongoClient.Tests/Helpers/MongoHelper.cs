using System.Threading.Tasks;
using MongoClient.Tests.Models;
using MongoDB.Bson;

namespace MongoClient.Tests.Helpers
{
    public static class MongoHelper
    {
        public static ObjectId NotFoundId = new ObjectId("2fcd299f9e1d7d949562d108");

        public static async Task<Person> FindPersonAsync(ObjectId objId, Nautilus.Experiment.DataProvider.Mongo.Schema.MongoBaseSchema<Person> personSchema)
        {
            var resultRecord = await personSchema.FindAsync(objId);
            return resultRecord;
        }

        public static Person FindPerson(ObjectId objId, Nautilus.Experiment.DataProvider.Mongo.Schema.MongoBaseSchema<Person> personSchema)
        {
            var resultRecord = personSchema.Find(objId);
            return resultRecord;
        }

        public static void CreatePerson(Person p, Nautilus.Experiment.DataProvider.Mongo.Schema.MongoBaseSchema<Person> personSchema)
        {
            personSchema.InsertRecord(p);
        }

        public static async Task CreatePersonAsync(Person p, Nautilus.Experiment.DataProvider.Mongo.Schema.MongoBaseSchema<Person> personSchema)
        {
            await personSchema.InsertRecordAsync(p);
        }
    }
}