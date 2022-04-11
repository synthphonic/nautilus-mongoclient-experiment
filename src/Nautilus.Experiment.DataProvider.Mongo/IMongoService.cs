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
        /// The database name attached to the mongo service provided by a configuration
        /// </summary>
        string DatabaseName { get; }

        /// <summary>
        /// Connect to mongo db
        /// </summary>
        void Connect();

        /// <summary>
        /// Drop database
        /// </summary>
        void DropDatabase();

        /// <summary>
        /// Drop database asynchronously
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task DropDatabaseAsync(CancellationToken token = default);

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
