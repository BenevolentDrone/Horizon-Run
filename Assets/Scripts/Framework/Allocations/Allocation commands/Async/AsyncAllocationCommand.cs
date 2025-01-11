using System.Threading.Tasks;

namespace HereticalSolutions.Allocations
{
	public class AsyncAllocationCommand<T>
	{
		public AllocationCommandDescriptor Descriptor { get; set; }

		public Task<T> AllocationDelegate { get; set; }

		public IAsyncAllocationCallback<T> AllocationCallback { get; set; }
	}
}