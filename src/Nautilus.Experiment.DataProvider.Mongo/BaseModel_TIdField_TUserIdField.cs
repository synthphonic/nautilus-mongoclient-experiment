using MongoDB.Bson.Serialization.Attributes;

namespace Nautilus.Experiment.DataProvider.Mongo
{
	public class MongoBaseModel<TIdField, TUserIdField>
	{
		[BsonId]
		public TIdField Id { get; set; }

		public TUserIdField UserId { get; set; }
	}
}