using System;

namespace HereticalSolutions.Persistence
{
	[System.AttributeUsage(
		System.AttributeTargets.Class)]
	public class VisitorAttribute : System.Attribute
	{
		public Type TargetType { get; private set; }

		public Type DTOType { get; private set; }

		public VisitorAttribute(
			Type targetType,
			Type dtoType)
		{
			TargetType = targetType;

			DTOType = dtoType;
		}
	}
}