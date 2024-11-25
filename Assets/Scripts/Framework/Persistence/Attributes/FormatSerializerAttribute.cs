using System;

namespace HereticalSolutions.Persistence
{
	[System.AttributeUsage(
		System.AttributeTargets.Class)]
	public class FormatSerializerAttribute : System.Attribute
	{
		public FormatSerializerAttribute()
		{
		}
	}
}