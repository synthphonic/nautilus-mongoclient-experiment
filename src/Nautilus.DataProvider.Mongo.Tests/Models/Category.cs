namespace MongoClient.Tests.Models;

[CollectionName("Categories")]
public class Category
{
    public ObjectId Id { get; set; }
    public string CategoryName { get; set; }
    public string UserId { get; set; }
}
