using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.Allocations
{
	public interface IAsyncAllocationResizeable
	{
		Task Resize(

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}