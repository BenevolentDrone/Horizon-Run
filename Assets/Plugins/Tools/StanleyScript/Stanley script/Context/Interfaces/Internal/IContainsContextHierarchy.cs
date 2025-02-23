using System.Collections.Generic;

using HereticalSolutions.Allocations;

namespace HereticalSolutions.StanleyScript
{
	public interface IContainsContextHierarchy
	{
		IStanleyContextInternal ParentContext { get; }

		bool TryGetChildContext(
			StanleyHandle handle,
			out IStanleyContextInternal context);

		bool CreateChildContext(
			out StanleyHandle handle);

		bool DestroyChildContext(
			StanleyHandle handle);

		IEnumerable<IStanleyContextInternal> ChildContexts { get; }
	}
}