using System;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoService.RunnerConsole.Models;
using Nautilus.Experiment.DataProvider.Mongo.Extensions;

namespace MongoService.RunnerConsole
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("==== Welcome to Mongo Service for .NET ====");

            const string ConnectionString = "mongodb://localhost:27017";
			const string databaseName = "passKeepr";

			var service = new Nautilus.Experiment.DataProvider.Mongo.MongoService(ConnectionString, databaseName);
            service.InitializeSchemas(new Type[] { typeof(PersonSchema) });
            service.UseCamelCase();
            service.Connect();

            var personSchema = service.GetSchema<Person>();

            #region Sync calls
            Console.WriteLine("=== SYNC CALLS");
			var personId = CreatePerson(new Person { FirstName = "ShahZaa", LastName = "shafie", Active = true }, personSchema);

            Console.WriteLine("FindPerson");
            var foundPerson = FindPerson(personId, personSchema);
            foundPerson.PrintObject();
            Console.WriteLine("===\n");
            #endregion

            #region Async calls
            Console.WriteLine("=== ASYNC CALLS");
            var personId2 = await CreatePersonAsync(new Person { FirstName = "Joe", LastName = "Jambul", Active = true }, personSchema);

            Console.WriteLine("FindPersonAsync");
            foundPerson = await FindPersonAsync(personId2, personSchema);
            foundPerson.PrintObject();
            Console.WriteLine("===\n");
            #endregion
        }

        private static async Task<Person> FindPersonAsync(ObjectId objId, Nautilus.Experiment.DataProvider.Mongo.Schema.MongoBaseSchema<Person> personSchema)
        {
            var resultRecord = await personSchema.FindAsync(objId);
            return resultRecord;
        }

        private static Person FindPerson(ObjectId objId, Nautilus.Experiment.DataProvider.Mongo.Schema.MongoBaseSchema<Person> personSchema)
		{
            var resultRecord = personSchema.Find(objId);
            return resultRecord;
        }

        private static ObjectId CreatePerson(Person p, Nautilus.Experiment.DataProvider.Mongo.Schema.MongoBaseSchema<Person> personSchema)
		{
            p.PrintObject();
            Console.WriteLine("Inserting record...");

            personSchema.InsertRecord(p);
            Console.WriteLine("Inserting record done...");
            Console.WriteLine();
            p.PrintObject();

            return p.Id;
        }

        private static async Task<ObjectId> CreatePersonAsync(Person p, Nautilus.Experiment.DataProvider.Mongo.Schema.MongoBaseSchema<Person> personSchema)
        {
            p.PrintObject();
            Console.WriteLine("Inserting record...");

            await personSchema.InsertRecordAsync(p);
            Console.WriteLine("Inserting record done...");
            Console.WriteLine();
            p.PrintObject();

            return p.Id;
        }
    }
}