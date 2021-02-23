using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MongoDB.Driver;
using Nautilus.Experiment.DataProvider.Mongo.Exceptions;
using Nautilus.Experiment.DataProvider.Mongo.Schema;

namespace Nautilus.Experiment.DataProvider.Mongo.Services
{
    public class BaseMongoService<TModel, TIdField, TUserIdField> where TModel : MongoBaseModel<TIdField, TUserIdField>, new()
	{
		private readonly MongoService _mongoService;

		public BaseMongoService(MongoService mongoService)
		{
			ConsoleOutput.Write(GetType());
			_mongoService = mongoService;
		}

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
				Nautilus.ConsoleOutput.Write(GetType(), $"Exception: {ex.Message}");

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
                ConsoleOutput.Write(GetType(), $"Exception: {ex.Message}");
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
				Nautilus.ConsoleOutput.Write(GetType(), $"Exception: {ex.Message}");
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
				Nautilus.ConsoleOutput.Write(GetType(), $"Exception: {ex.Message}");
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
				Nautilus.ConsoleOutput.Write(GetType(), $"Exception: {ex.Message}");
				throw new NautilusMongoDbException(ex.Message, ex);
			}
		}

		protected void Connect()
		{
            ConsoleOutput.Write(GetType());
			_mongoService.Connect();
		}

		protected MongoBaseSchema<TModel> GetSchema()
		{
            ConsoleOutput.Write(GetType());
			return _mongoService.GetSchema<TModel>();
		}

		protected MongoBaseSchema<TMongoModel> GetSchema<TMongoModel>() where TMongoModel : class, new()
		{
            ConsoleOutput.Write(GetType());
			return _mongoService.GetSchema<TMongoModel>();
		}
	}
}