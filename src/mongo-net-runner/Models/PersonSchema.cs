using Nautilus.Experiment.DataProvider.Mongo.Schema;

namespace MongoService.RunnerConsole.Models
{
	public class PersonSchema : MongoBaseSchema<Person>
	{
		public PersonSchema(MongoDB.Driver.IMongoDatabase database): base(database)
		{
		}
	}
}