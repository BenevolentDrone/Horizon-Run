using System.Threading.Tasks;

using System.Collections.Generic;

using HereticalSolutions.Asynchronous;

namespace HereticalSolutions.Allocations
{
	public class AsyncCompositeAllocationCallback<T>
		: IAsyncAllocationCallback<T>
	{
		private readonly List<IAsyncAllocationCallback<T>> callbacks;

		public AsyncCompositeAllocationCallback(
			List<IAsyncAllocationCallback<T>> callbacks)
		{
			this.callbacks = callbacks;
		}

		#region IAsyncAllocationCallback

		public async Task OnAllocated(
			T element,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			foreach (var callback in callbacks)
				await callback.OnAllocated(
					element,
					
					asyncContext);
		}

		#endregion

		public void AddCallback(
			IAsyncAllocationCallback<T> callback)
		{
			callbacks.Add(callback);
		}

		public void RemoveCallback(
			IAsyncAllocationCallback<T> callback)
		{
			callbacks.Remove(callback);
		}
	}
}