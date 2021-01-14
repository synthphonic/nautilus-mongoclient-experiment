using System;

namespace Nautilus.Experiment.DataProvider.Mongo.Attributes
{
	public class CollectionName : Attribute
	{
		public CollectionName(string name)
		{
			Name = name;
		}

		public string Name { get; private set; }
	}
}