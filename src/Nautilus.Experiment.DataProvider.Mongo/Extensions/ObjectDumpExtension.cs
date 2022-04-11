/*
 * References
 *  https://stackoverflow.com/questions/852181/c-printing-all-properties-of-an-object
 *  https://stackoverflow.com/questions/4023462/how-do-i-automatically-display-all-properties-of-a-class-and-their-values-in-a-s/4023521
 * 
 * 
 */
namespace Nautilus.Experiment.DataProvider.Mongo.Extensions;

public static class ObjectDumpExtension
{
	//public static string PrintObject2(this object value)
	//{
	//    if (value == null)
	//        return null;

	//    var propertiesInfo = value.GetType().GetProperties();

	//    var sb = new StringBuilder();

	//    foreach (var info in propertiesInfo)
	//    {
	//        var objValue = info.GetValue(value, null) ?? "(null)";
	//        sb.AppendLine(info.Name + ": " + objValue.ToString());
	//    }

	//    return sb.ToString();
	//}

	public static void PrintObject(this object value)
	{
		foreach (PropertyDescriptor descriptor in TypeDescriptor.GetProperties(value))
		{
			var name = descriptor.Name;
			var objectValue = descriptor.GetValue(value);

			Console.WriteLine($"{name} = {objectValue}");
		}
	}
}
