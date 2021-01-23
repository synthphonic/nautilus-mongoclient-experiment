namespace Nautilus.Experiment.DataProvider.Mongo
{
	public class BaseModel<TIdField, TUserIdField>
	{
		public TIdField Id { get; set; }
		public TUserIdField UserId { get; set; }
	}
}