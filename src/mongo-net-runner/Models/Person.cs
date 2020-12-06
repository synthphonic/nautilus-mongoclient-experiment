using MongoDB.Bson;

namespace MongoService.RunnerConsole.Models
{
	public class Person
	{
		public ObjectId Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }

		public bool Active { get; set; }
	}
}