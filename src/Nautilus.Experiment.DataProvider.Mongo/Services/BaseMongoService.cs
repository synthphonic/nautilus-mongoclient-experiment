namespace Nautilus.Experiment.DataProvider.Mongo.Services;

public class BaseMongoService<TModel, TIdField, TUserIdField> where TModel : MongoBaseModel<TIdField, TUserIdField>, new()
{
	private IMongoService _mongoService;

	public async Task DeleteAsync(TIdField id)
	{
		ConsoleOutput.Write(GetType());

		try
		{
			Connect();

			var schema = _mongoService.GetSchema<TModel>();
			var foundTag = await schema.DeleteAsync(id);
		}
		catch (Exception ex)
		{
			ConsoleOutput.Write(GetType(), ConsoleMessage.Create($"{ex.Message}"));

			//TODO: BaseMongoService class will be relocated to Nautilus experimental mongo project. need to refactor this as well
			throw new NautilusMongoDbException(ex.Message, ex);
		}
	}

	public async Task<TModel> GetAsync(TIdField id)
	{
		ConsoleOutput.Write(GetType());

		try
		{
			Connect();

			var schema = _mongoService.GetSchema<TModel>();
			var filter = Builders<TModel>.Filter.Where(t => t.Id.Equals(id));
			var foundRecord = await schema.FindAsync(filter);

			return foundRecord;
		}
		catch (Exception ex)
		{
			ConsoleOutput.Write(GetType(), ConsoleMessage.Create($"{ex.Message}"));
			throw new NautilusMongoDbException(ex.Message, ex);
		}
	}

	public async Task<IEnumerable<TModel>> GetListAsync(TUserIdField userId)
	{
		ConsoleOutput.Write(GetType());

		IEnumerable<TModel> models = null;

		try
		{
			Connect();

			var schema = _mongoService.GetSchema<TModel>();

			var filter = Builders<TModel>.Filter.Where(t => t.UserId.Equals(userId));
			models = await schema.FindManyAsync(filter);

		}
		catch (Exception ex)
		{
			ConsoleOutput.Write(GetType(), ConsoleMessage.Create($"{ex.Message}"));
			throw new NautilusMongoDbException(ex.Message, ex);
		}

		return models;
	}

	public async Task SaveAsync(TModel model)
	{
		ConsoleOutput.Write(GetType());

		try
		{
			Connect();

			var schema = _mongoService.GetSchema<TModel>();

			Nautilus.ConsoleOutput.Write(GetType(), message: $"schema is not be null? [{schema != null}]");

			await schema.InsertAsync(model);
		}
		catch (Exception ex)
		{
			ConsoleOutput.Write(GetType(), ConsoleMessage.Create($"{ex.Message}"));
			throw new NautilusMongoDbException(ex.Message, ex);
		}
	}

	public async Task UpsertAsync(TModel model)
	{
		ConsoleOutput.Write(GetType());

		try
		{
			Connect();

			var schema = _mongoService.GetSchema<TModel>();
			//await schema.UpsertAsync(t => t.Id == model.Id, model);
			await schema.UpsertAsync(t => t.Id.Equals(model.Id), model);
		}
		catch (Exception ex)
		{
			ConsoleOutput.Write(GetType(), ConsoleMessage.Create($"{ex.Message}"));
			throw new NautilusMongoDbException(ex.Message, ex);
		}
	}

	protected void Connect()
	{
		ConsoleOutput.Write(GetType(), ConsoleMessage.Create(""));
		_mongoService.Connect();
	}

	protected MongoBaseSchema<TModel> GetSchema()
	{
		ConsoleOutput.Write(GetType(), ConsoleMessage.Create(""));
		return _mongoService.GetSchema<TModel>();
	}

	protected MongoBaseSchema<TMongoModel> GetSchema<TMongoModel>() where TMongoModel : class, new()
	{
		ConsoleOutput.Write(GetType(), ConsoleMessage.Create(""));
		return _mongoService.GetSchema<TMongoModel>();
	}

	protected virtual void SetMongoService(IMongoService service)
	{
		ConsoleOutput.Write(GetType(), ConsoleMessage.Create(""));
		_mongoService = service;
	}
}
