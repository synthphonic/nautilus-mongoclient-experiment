namespace Nautilus.Experiment.DataProvider.Mongo.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class CollectionName : Attribute
{
	public CollectionName(string name)
	{
		Name = name;
	}

	public string Name { get; private set; }
}
