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
            var p = new Person { FirstName = "ShahZaa", LastName = "shafie", Active = true };
            
            p.PrintObject();
            Console.WriteLine("Inserting record...");

			personSchema.InsertRecord(p);
			Console.WriteLine("Inserting record done...");
            p.PrintObject();

            var resultRecord = personSchema.Find(new ObjectId("5fccb39b8a4aa9e6fd4c24b1"));
            Console.WriteLine($"returningRecord2 null? (should be null) : {resultRecord == null}");

            var resultRecordAsync = await personSchema.FindAsync(new ObjectId("5fccb39b8a4aa9e6fd4c24b2"));
            Console.WriteLine($"returningRecord2 null? (not null) : {resultRecordAsync == null}");

            //var returningRecord = personSchema.Find(p.Id);
            //Console.WriteLine("\nreturningRecord");
            //returningRecord.PrintObject();
        }
    }
}