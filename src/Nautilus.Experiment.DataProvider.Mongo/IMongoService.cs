using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Nautilus.Experiment.DataProvider.Mongo.Schema;

namespace Nautilus.Experiment.DataProvider.Mongo
{
    /// <summary>
    /// Represents a Mongo database class for registration, connection and others.
    /// </summary>
    public interface IMongoService
    {
        /// <summary>
        /// The Database key given from the configuration file setting
        /// </summary>
        string Key { get; }

        /// <summary>
        /// Connect to mongo db
        /// </summary>
        void Connect();

        /// <summary>
        /// Drop database
        /// </summary>
        /// <param name="databaseName"></param>
        void DropDatabase(string databaseName);

        /// <summary>
        /// Drop database asynchronously
        /// </summary>
        /// <param name="databaseName"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task DropDatabaseAsync(string databaseName, CancellationToken token = default);

        /// <summary>
        /// Gets a mongo db collection schema
        /// </summary>
        /// <typeparam name="TModel"></typeparam>
        /// <returns></returns>
        MongoBaseSchema<TModel> GetSchema<TModel>() where TModel : class, new();

        /// <summary>
        /// Register a list of schemas into mongo service
        /// </summary>
        /// <param name="schemaTypes"></param>
        void RegisterSchemas(IEnumerable<Type> schemaTypes);

        /// <summary>
        /// Use mongo database camel casing or just stick to mongodb's default.
        /// </summary>
        void UseCamelCase();
    }
}
