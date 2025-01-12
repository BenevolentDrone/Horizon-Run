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
	public class AsyncStackPool<T>
		: IAsyncPool<T>,
		  IAsyncAllocationResizeable,
		  ICleanuppable,
		  IDisposable
	{
		private readonly Stack<T> pool;

		private readonly AsyncAllocationCommand<T> allocationCommand;

		private readonly object lockObject;

		private readonly ILogger logger;

		private int capacity;

		public AsyncStackPool(
			Stack<T> pool,
			AsyncAllocationCommand<T> allocationCommand,
			ILogger logger = null)
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
					result = pool.Pop();
				}
				else
				{
					capacity = await StackPoolFactory.ResizeAsyncStackPool(
						pool,
						capacity,
						allocationCommand,
						logger,
						
						asyncContext);

					result = pool.Pop();
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
				pool.Push(instance);
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
				capacity = await StackPoolFactory.ResizeAsyncStackPool(
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
					if (item is ICleanuppable cleanItem)
					{
						cleanItem.Cleanup();
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
					if (item is IDisposable disposableItem)
					{
						disposableItem.Dispose();
					}
				}

				pool.Clear();
			}
		}

		#endregion
	}
}