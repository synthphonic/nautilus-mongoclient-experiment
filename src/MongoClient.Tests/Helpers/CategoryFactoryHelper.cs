using MongoClient.Tests.Models;

namespace MongoClient.Tests.Helpers
{
	public static class CategoryFactoryHelper
	{
		internal static Category CreateObject(string categoryName, string userId)
		{
			var category = new Category
			{
				CategoryName = categoryName,
				UserId = userId
			};

			return category;
		}
	}
}