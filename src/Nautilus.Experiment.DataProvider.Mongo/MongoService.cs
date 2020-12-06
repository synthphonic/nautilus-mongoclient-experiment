using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Nautilus.Experiment.DataProvider.Mongo.Schema;

namespace Nautilus.Experiment.DataProvider.Mongo
{
	public class MongoService
    {
        private readonly string _connectionString;
        private readonly string _databaseName;

        private MongoClient _mongClient;
        private IMongoDatabase _database;
        private IList<MongoBaseSchema> _list;
        private Type[] _schemaTypes;

        public MongoService(string connectionString, string databaseName)
        {
            _connectionString = connectionString;
            _databaseName = databaseName;
        }

        public void UseCamelCase()
		{
            var conventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("camelCase", conventionPack, t => true);
		}
        
        public void Connect()
        {
            if (_mongClient == null)
            {
                _mongClient = new MongoClient(_connectionString);
                _database = _mongClient.GetDatabase(_databaseName);

                InitializeSchemas();
            }
        }

        public void InitializeSchemas(Type[] schemaTypes)
        {
            _schemaTypes = schemaTypes;
        }

        private void InitializeSchemas()
		{
            _list = new List<MongoBaseSchema>();


            foreach (var item in _schemaTypes)
            {
                var instance = (MongoBaseSchema)Activator.CreateInstance(item, _database);
                _list.Add(instance);
            }
        }

        public MongoBaseSchema<T> GetSchema<T>() where T : class,new()
		{
            Console.WriteLine($"Fetching schema for {new T().GetType().FullName}");

            var instance = new T();
            var schemaName = instance.GetType().Name.ToLower();
            var found = _list.FirstOrDefault(x => x.TableNameMongo.Equals(schemaName));

            return found as MongoBaseSchema<T>;
        }
    }
}