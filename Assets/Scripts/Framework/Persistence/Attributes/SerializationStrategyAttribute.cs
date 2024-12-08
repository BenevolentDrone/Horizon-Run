namespace HereticalSolutions.Persistence
{
	[System.AttributeUsage(
		System.AttributeTargets.Class)]
	public class SerializationStrategyAttribute : System.Attribute
	{
		public SerializationStrategyAttribute()
		{
		}
	}
}