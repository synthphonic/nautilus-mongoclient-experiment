/*
 * References
 *  https://stackoverflow.com/questions/32703051/properly-shutting-down-mongodb-database-connection-from-c-sharp-2-1-driver
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
		private IList<MongoBaseSchema> _initializedSchemas;
		private IEnumerable<Type> _registeringSchemaTypes;

		public MongoService(string connectionString, string databaseName)
		{
			_connectionString = connectionString;
			_databaseName = databaseName;

			_initializedSchemas = new List<MongoBaseSchema>();
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
			try
			{
				if (_mongoClient == null)
				{
					_mongoClient = new MongoClient(_connectionString);

					_database = _mongoClient.GetDatabase(_databaseName);

					//InitializeSchemas();

					//
					// code below are debugging codes
					// reference: https://stackoverflow.com/questions/29459990/mongoserver-state-equivalent-in-the-2-0-driver
					//
					//var server = _mongoClient.Cluster.Description.Servers.Single();
					//if (_mongoClient.Cluster.Description.Servers.Single().State == MongoDB.Driver.Core.Servers.ServerState.Connected)
					//{
					//}
					//if (_mongoClient.Cluster.Description.State == MongoDB.Driver.Core.Clusters.ClusterState.Connected)
					//{
					//}
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		public void RegisterSchemas(IEnumerable<Type> schemaTypes)
		{
			_registeringSchemaTypes = schemaTypes;
		}

		public void DropDatabase(string databaseName)
		{
			_mongoClient.DropDatabase(databaseName);
		}

		public async Task DropDatabaseAsync(string databaseName, CancellationToken token = default)
		{
			await _mongoClient.DropDatabaseAsync(databaseName, token);
		}

		private void InitializeSchema(Type schemaType)
		{
			try
			{
				var instance = (MongoBaseSchema)Activator.CreateInstance(schemaType, new object[] { _database });
				instance.CreateIndexes();

				_initializedSchemas.Add(instance);



				//foreach (var schemaType in _registeringSchemaTypes)
				//{
				//	var instance = (MongoBaseSchema)Activator.CreateInstance(schemaType, new object[] { _database });
				//	instance.CreateIndexes();

				//	initializedSchemas.Add(instance);
				//}
			}
			catch (TimeoutException)
			{
				throw;
			}
			catch (Exception)
			{
				throw;
			}
		}

		public MongoBaseSchema<TModel> GetSchema<TModel>() where TModel : class, new()
		{
			var modelInstance = new TModel();

			Console.WriteLine($"Fetching schema for {new TModel().GetType().FullName}");

			var schemaName = ProcessSchemaName(modelInstance);

			var found = _initializedSchemas.FirstOrDefault(x => x.TableNameMongo.Equals(schemaName));
			if (found == null)
			{
				foreach (var schemaType in _registeringSchemaTypes)
				{
					var instance = Activator.CreateInstance(schemaType);
					var mongoModelInterface = instance as IMongoModel;
					if (modelInstance.GetType().Equals(mongoModelInterface.ModelType))
					{
						InitializeSchema(schemaType);
						var returningSchemaInstance = _initializedSchemas.FirstOrDefault(x => x.TableNameMongo.Equals(schemaName));
						return returningSchemaInstance as MongoBaseSchema<TModel>;
					}					
				}
			}

			return found as MongoBaseSchema<TModel>;
		}

		private string ProcessSchemaName(object modelInstance)
		{
			var schemaName = modelInstance.GetType().Name.ToLower();

			var foundAttributes = modelInstance.GetType().GetCustomAttributes(typeof(CollectionName), false);
			if (foundAttributes != null && foundAttributes.Count() == 1)
			{
				var attrib = foundAttributes[0] as CollectionName;
				schemaName = attrib.Name;
			}

			return schemaName;
		}

		private void InitializeSchemas()
		{
			_initializedSchemas = new List<MongoBaseSchema>();

			try
			{
				foreach (var schemaType in _registeringSchemaTypes)
				{
					var instance = (MongoBaseSchema)Activator.CreateInstance(schemaType, new object[] { _database });
					instance.CreateIndexes();

					_initializedSchemas.Add(instance);
				}
			}
			catch (TimeoutException)
			{
				throw;
			}
			catch (Exception)
			{
				throw;
			}
		}

		//public MongoBaseSchema<TModel> GetSchema2<TModel>() where TModel : class, new()
		//{
		//	Console.WriteLine($"Fetching schema for {new TModel().GetType().FullName}");

		//	var schemaName = ProcessSchemaName<TModel>();

		//	var found = initializedSchemas.FirstOrDefault(x => x.TableNameMongo.Equals(schemaName));

		//	return found as MongoBaseSchema<TModel>;
		//}
	}
}