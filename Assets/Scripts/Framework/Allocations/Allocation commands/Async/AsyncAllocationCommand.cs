using System;
using System.Threading.Tasks;

namespace HereticalSolutions.Allocations
{
	public class AsyncAllocationCommand<T>
	{
		public AllocationCommandDescriptor Descriptor { get; set; }

		public Func<Task<T>> AllocationDelegate { get; set; }

		public IAsyncAllocationCallback<T> AllocationCallback { get; set; }
	}
}