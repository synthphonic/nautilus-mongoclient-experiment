namespace Nautilus.Experiment.DataProvider.Mongo.Schema
{
	public class MongoBaseSchema
	{
		protected MongoBaseSchema()
		{
		}

		public string TableNameCSharp { get; internal set; }
		public string TableNameFullCSharp { get; internal set; }
		public string TableNameMongo { get; internal set; }

	}
}
