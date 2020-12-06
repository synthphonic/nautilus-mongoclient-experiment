using MongoDB.Driver;

namespace Nautilus.Experiment.DataProvider.Mongo.Schema
{
	public static class IndexHelper
	{
		public static CreateIndexModel<TModel> CreateIndex<TModel>(string fieldName, bool isUnique = false)
		{
			var indexOptions = new CreateIndexOptions() { Unique = isUnique };
			var field = new StringFieldDefinition<TModel>(fieldName);
			var indexDef = new IndexKeysDefinitionBuilder<TModel>().Ascending(field);

			return new CreateIndexModel<TModel>(indexDef, indexOptions);
		}
	}
}