/*
 * References
 *  https://stackoverflow.com/questions/32703051/properly-shutting-down-mongodb-database-connection-from-c-sharp-2-1-driver
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;
using Nautilus.Experiment.DataProvider.Mongo.Attributes;
using Nautilus.Experiment.DataProvider.Mongo.Schema;

namespace Nautilus.Experiment.DataProvider.Mongo
{
	public class MongoService
    {
        private readonly string _connectionString;
        private readonly string _databaseName;

        private MongoClient _mongoClient;
        private IMongoDatabase _database;
        private IList<MongoBaseSchema> _list;
        private Type[] _schemaTypes;

        public MongoService(string connectionString, string databaseName)
        {
            _connectionString = connectionString;
            _databaseName = databaseName;
        }

        /// <summary>
		/// Use camelCasing on document elements
		/// </summary>
        public void UseCamelCase()
		{
            var conventionPack = new ConventionPack { new CamelCaseElementNameConvention() };
            ConventionRegistry.Register("camelCase", conventionPack, t => true);
		}
    
        public void Connect()
        {
            if (_mongoClient == null)
            {
                _mongoClient = new MongoClient(_connectionString);
                _database = _mongoClient.GetDatabase(_databaseName);

                InitializeSchemas();
            }
        }

        public void InitializeSchemas(Type[] schemaTypes)
        {
            _schemaTypes = schemaTypes;
        }

        public void DropDatabase(string databaseName)
		{
            _mongoClient.DropDatabase(databaseName);
		}

        public async Task DropDatabaseAsync(string databaseName, CancellationToken token = default)
        {
            await _mongoClient.DropDatabaseAsync(databaseName, token);
        }

        private void InitializeSchemas()
		{
            _list = new List<MongoBaseSchema>();

            foreach (var schemaType in _schemaTypes)
            {
                var instance = (MongoBaseSchema)Activator.CreateInstance(schemaType, new object[] { _database });
                instance.CreateIndexes();

                _list.Add(instance);
            }
        }

        public MongoBaseSchema<TModel> GetSchema<TModel>() where TModel : class,new()
		{
            Console.WriteLine($"Fetching schema for {new TModel().GetType().FullName}");

            var instance = new TModel();
            var schemaName = instance.GetType().Name.ToLower();

            var foundAttributes = instance.GetType().GetCustomAttributes(typeof(CollectionName), false);
            if (foundAttributes != null && foundAttributes.Count() == 1)
            {
                var attrib = foundAttributes[0] as CollectionName;
                schemaName = attrib.Name;
            }

            var found = _list.FirstOrDefault(x => x.TableNameMongo.Equals(schemaName));

            return found as MongoBaseSchema<TModel>;
        }
	}
}