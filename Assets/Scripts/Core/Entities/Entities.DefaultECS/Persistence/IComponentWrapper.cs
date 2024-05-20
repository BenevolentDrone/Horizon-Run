using System;

namespace HereticalSolutions.Entities
{
	public interface IComponentWrapper
	{
		Type Type { get; }

		object ObjectValue { get; }
	}
}