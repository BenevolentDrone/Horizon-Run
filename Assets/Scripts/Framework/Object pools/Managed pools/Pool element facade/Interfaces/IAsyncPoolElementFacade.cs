using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.Pools
{
	//TODO: design this properly
	public interface IAsyncPoolElementFacade<T>
	{
		T Value { get; set; }

		EPoolElementStatus Status { get; set; }

		IAsyncManagedPool<T> Pool { get; set; }

		Task Push(

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}