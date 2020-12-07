using MongoDB.Bson;

namespace MongoClient.Tests.Models
{
	public class Person
	{
		public ObjectId Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public bool Active { get; set; }
	}
}