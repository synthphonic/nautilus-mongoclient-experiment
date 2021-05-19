using System;
using System.Collections.Generic;
using System.Linq;
using MongoClient.Tests.Models.Schema;
using MongoDB.Bson;
using Nautilus.Experiment.DataProvider.Mongo;

namespace MongoClient.Tests.Helpers
{
    internal static class MongoInitializer
    {
        const string ConnectionString = "mongodb://localhost:27017";

        public static ObjectId NotFoundId = new ObjectId("2fcd299f9e1d7d949562d108");

        internal static MongoService Initialize(string databaseName)
        {
            DatabaseName = databaseName;

            var mongoService = new MongoService(ConnectionString, DatabaseName);
            mongoService.RegisterSchemas(new Type[]
            {
                typeof(PersonSchema),
                typeof(UserSchema),
                typeof(CategorySchema)
            });

            mongoService.UseCamelCase();
            mongoService.Connect();

            return mongoService;
        }

        /// <summary>
        /// Creates a mongo service but not connecting to the db just yet
        /// </summary>
        /// <param name="schemaTypes"></param>
        /// <param name="databasename"></param>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        internal static MongoService CreateMongoService(IEnumerable<Type> schemaTypes, string databasename = "test_db", string connectionString = "mongodb://localhost:27017")
        {
            var mongoService = new MongoService(connectionString, databasename);
            mongoService.RegisterSchemas(schemaTypes.ToArray());

            return mongoService;
        }

        internal static string DatabaseName { get; private set; }
    }
}