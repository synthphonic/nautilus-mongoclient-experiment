namespace MongoClient.Tests.Models;

[CollectionName("CategoryDetails")]
public class CategoryDetail
{
    public ObjectId Id { get; set; }
    public string CategoryName { get; set; }
    public string Description { get; set; }
    public string Comments { get; set; }
}
