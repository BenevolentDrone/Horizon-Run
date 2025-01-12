using System.Threading.Tasks;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.Pools
{
	//TODO: design this properly
	public interface IAsyncPool<T>
	{
		Task<T> Pop(

			//Async tail
			AsyncExecutionContext asyncContext);

		Task<T> Pop(
			IPoolPopArgument[] args,

			//Async tail
			AsyncExecutionContext asyncContext);

		Task Push(
			T instance,

			//Async tail
			AsyncExecutionContext asyncContext);
	}
}