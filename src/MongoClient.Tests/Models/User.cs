using MongoDB.Bson;

namespace MongoClient.Tests.Models
{
	public class User
	{
		public ObjectId Id { get; set; }
		public string Email { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string HashedPassword { get; set; }
		public bool Active { get; set; }
	}
}