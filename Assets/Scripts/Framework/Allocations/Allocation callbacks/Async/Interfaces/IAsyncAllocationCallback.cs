using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.Allocations
{
	public interface IAsyncAllocationCallback<T>
	{
		Task OnAllocated(
			T instance,

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}