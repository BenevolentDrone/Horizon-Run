using System;

namespace HereticalSolutions.Modules.Core_DefaultECS
{
	public interface IComponentWrapper
	{
		Type Type { get; }

		object ObjectValue { get; }
	}
}