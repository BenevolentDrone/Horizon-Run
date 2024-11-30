using System;

namespace HereticalSolutions.Persistence
{
	public interface IStrategyWithFilter
	{
		bool AllowsType<TValue>();

		bool AllowsType(
			Type valueType);
	}
}