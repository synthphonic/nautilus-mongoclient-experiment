using MongoDB.Bson;

namespace MongoClient.Tests.Models
{
    public class Category
    {
        public ObjectId Id { get; set; }
        public string CategoryName { get; set; }
        public string UserId { get; set; }
    }
}