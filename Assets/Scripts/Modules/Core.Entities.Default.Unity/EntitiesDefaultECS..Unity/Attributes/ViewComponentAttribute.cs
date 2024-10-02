namespace HereticalSolutions.Entities
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