namespace Nautilus.Experiment.DataProvider.Mongo
{
	public interface IMongoServiceFactory
	{
		IMongoService GetService(string key);
	}

	public class MongoServiceFactory : IMongoServiceFactory
	{
		private Dictionary<string, IMongoService> _mongoServices;

		public MongoServiceFactory()
		{
			_mongoServices = new Dictionary<string, IMongoService>();
		}

		public void Add(string key, IMongoService mongoService)
		{
			_mongoServices[key] = mongoService;
		}

		public IMongoService GetService(string key)
		{
			return _mongoServices[key];
		}
	}
}