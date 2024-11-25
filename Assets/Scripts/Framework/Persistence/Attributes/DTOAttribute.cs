using System;

namespace HereticalSolutions.Persistence
{
	[System.AttributeUsage(
		System.AttributeTargets.Class
		| System.AttributeTargets.Struct)]
	public class DTOAttribute : System.Attribute
	{
		public Type TargetType { get; private set; }

		public DTOAttribute(Type targetType)
		{
			TargetType = targetType;
		}
	}
}