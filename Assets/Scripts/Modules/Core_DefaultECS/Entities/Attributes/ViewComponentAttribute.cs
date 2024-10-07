namespace HereticalSolutions.Modules.Core_DefaultECS
{
	[System.AttributeUsage(
		System.AttributeTargets.Class
		| System.AttributeTargets.Struct)]
	public class ViewComponentAttribute : System.Attribute
	{
		public ViewComponentAttribute()
		{
		}
	}
}