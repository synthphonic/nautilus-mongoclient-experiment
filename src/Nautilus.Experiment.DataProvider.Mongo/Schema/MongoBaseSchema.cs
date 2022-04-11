namespace Nautilus.Experiment.DataProvider.Mongo.Schema;

public class MongoBaseSchema : IMongoModel
{
	//public event EventHandler OnCreateIndexes;
	protected MongoBaseSchema()
	{
	}

	public string TableNameCSharp { get; internal set; }
	public string TableNameFullCSharp { get; internal set; }
	public string TableNameMongo { get; internal set; }
	public Type ModelType { get; internal set; }

	internal void CreateIndexes()
	{
		Console.WriteLine("MongoBaseSchema.CreateIndexes");

		CreateModelIndexesAsync();
		//OnCreateIndexes?.Invoke(null, EventArgs.Empty);
	}

	protected virtual Task CreateModelIndexesAsync()
        {
		ConsoleOutput.Write(GetType(), message: $"[{ModelType.Name}]");

		return Task.CompletedTask;
        }
}

public interface IMongoModel
{
	Type ModelType { get; }
}
