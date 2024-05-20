using System;

namespace HereticalSolutions.StanleyScript
{
	public interface IStanleyVariable
	{
		string Name { get; }

		Type VariableType { get; }

		object GetValue();

		T GetValue<T>();
	}
}