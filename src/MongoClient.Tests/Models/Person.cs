using MongoDB.Bson;

namespace MongoClient.Tests.Models
{
	public class Person
	{
		public ObjectId Id { get; set; }
		public string FirstName { get; set; } = string.Empty;
		public string LastName { get; set; } = string.Empty;
		public bool Active { get; set; }

		//TODO: Research on BsonDefaultValue
		//[BsonDefaultValue("ENTER")]

	}
}