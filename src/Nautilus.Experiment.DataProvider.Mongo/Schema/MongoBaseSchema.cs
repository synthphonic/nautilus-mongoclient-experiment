using System;

namespace Nautilus.Experiment.DataProvider.Mongo.Schema
{
	public class MongoBaseSchema
	{
		public event EventHandler OnCreateIndexes;
		protected MongoBaseSchema()
		{
		}

		public string TableNameCSharp { get; internal set; }
		public string TableNameFullCSharp { get; internal set; }
		public string TableNameMongo { get; internal set; }

		internal void CreateIndexes()
		{
			Console.WriteLine("MongoBaseSchema.CreateIndexes");

			OnCreateIndexes?.Invoke(null, EventArgs.Empty);
		}
	}
}
