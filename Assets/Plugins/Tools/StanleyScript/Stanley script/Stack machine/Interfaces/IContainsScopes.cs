using HereticalSolutions.Allocations;

namespace HereticalSolutions.StanleyScript
{
	public interface IContainsScopes
	{
		IIDAllocationController<byte> ScopeIDAllocationController { get; }

		StanleyScope GlobalScope { get; }

		StanleyScope CurrentScope { get; }

		bool TryGetScope(
			StanleyHandle handle,
			out StanleyScope scope);

		bool PushScope(
			out StanleyHandle handle);

		bool PopScope();

		bool SwitchScope(
			StanleyHandle handle);

		bool RemoveScope(
			StanleyHandle handle);
	}
}