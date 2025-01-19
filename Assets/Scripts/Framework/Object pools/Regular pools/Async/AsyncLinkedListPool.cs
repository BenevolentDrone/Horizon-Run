using System;
using System.Threading.Tasks;

using System.Collections.Generic;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Allocations;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools
{
	public class AsyncLinkedListPool<T>
		: IAsyncPool<T>,
		  IAsyncAllocationResizeable,
		  ICleanuppable,
		  IDisposable
	{
		private readonly LinkedList<T> pool;

		private readonly AsyncAllocationCommand<T> allocationCommand;

		private readonly object lockObject;

		private readonly ILogger logger;

		private int capacity;

		public AsyncLinkedListPool(
			LinkedList<T> pool,
			AsyncAllocationCommand<T> allocationCommand,
			ILogger logger)
		{
			this.pool = pool;

			this.allocationCommand = allocationCommand;

			this.logger = logger;


			lockObject = new object();


			capacity = this.pool.Count;
		}

		#region IAsyncPool

		public async Task<T> Pop(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			lock (lockObject)
			{
				T result = default(T);

				if (pool.Count != 0)
				{
					result = pool.First.Value;

					pool.RemoveFirst();
				}
				else
				{
					capacity = await LinkedListPoolFactory.ResizeAsyncLinkedListPool(
						pool,
						capacity,
						allocationCommand,
						logger,
						
						asyncContext);

					result = pool.First.Value;

					pool.RemoveFirst();
				}

				return result;
			}
		}

		public async Task<T> Pop(
			IPoolPopArgument[] args,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			return await Pop(
				asyncContext);
		}

		public async Task Push(
			T instance,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			lock (lockObject)
			{
				pool.AddFirst(instance);
			}
		}

		#endregion

		#region IAsyncAllocationResizeable

		public async Task Resize(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			lock (lockObject)
			{
				capacity = await LinkedListPoolFactory.ResizeAsyncLinkedListPool(
					pool,
					capacity,
					allocationCommand,
					logger,
					
					asyncContext);
			}
		}

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			lock (lockObject)
			{
				foreach (var item in pool)
				{
					if (item is ICleanuppable)
					{
						(item as ICleanuppable).Cleanup();
					}
				}

				pool.Clear();
			}
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			lock (lockObject)
			{
				foreach (var item in pool)
				{
					if (item is IDisposable)
					{
						(item as IDisposable).Dispose();
					}
				}

				pool.Clear();
			}
		}

		#endregion
	}
}