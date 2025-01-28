namespace HereticalSolutions.Allocations
{
	public interface IIDAllocationController<TID>
	{
		bool ValidateID(
			TID id);

		bool IsAllocated(
			TID id);

		bool AllocateID(
			out TID id);

		bool FreeID(
			TID id);
	}
}