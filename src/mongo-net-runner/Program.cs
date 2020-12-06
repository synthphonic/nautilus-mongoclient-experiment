using System;
using MongoService.RunnerConsole.Models;
using MongoService.RunnerConsole.Models.Schema;
using Nautilus.Experiment.DataProvider.Mongo.Extensions;

namespace MongoService.RunnerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("==== Welcome to Mongo Service for .NET ====");

            const string ConnectionString = "mongodb://localhost:27017";
			const string databaseName = "passKeepr";

			var service = new Nautilus.Experiment.DataProvider.Mongo.MongoService(ConnectionString, databaseName);
            service.InitializeSchemas(new Type[] { typeof(PersonSchema) });
            service.UseCamelCase();
            service.Connect();

            var personSchema = service.GetSchema<Person>();
            var p = new Person { FirstName = "ShahZ", LastName = "shafie", Active = true };
            
            p.PrintObject();
            Console.WriteLine("Inserting record...");

            personSchema.InsertRecord(p);
            Console.WriteLine("Inserting record done...");
            p.PrintObject();

            var returningRecord = personSchema.Find(p.Id);
            Console.WriteLine("\nreturningRecord");
            returningRecord.PrintObject();
        }
    }
}