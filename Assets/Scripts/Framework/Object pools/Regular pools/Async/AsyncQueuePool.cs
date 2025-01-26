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
	public class AsyncQueuePool<T>
		: IAsyncPool<T>,
		  IAsyncAllocationResizeable,
		  ICleanuppable,
		  IDisposable
	{
		private readonly Queue<T> pool;

		private readonly AsyncAllocationCommand<T> allocationCommand;

		private readonly object lockObject;

		private readonly ILogger logger;

		private int capacity;

		private bool isResizing;

		public AsyncQueuePool(
			Queue<T> pool,
			AsyncAllocationCommand<T> allocationCommand,
			ILogger logger)
		{
			this.pool = pool;

			this.allocationCommand = allocationCommand;

			this.logger = logger;


			lockObject = new object();


			capacity = this.pool.Count;

			isResizing = false;
		}

		#region IAsyncPool

		public async Task<T> Pop(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			T result = default(T);

			#region Wait for resize

			bool isResizingClosure = false;

			lock (lockObject)
			{
				isResizingClosure = isResizing;
			}

			while (isResizingClosure)
			{
				await Task.Yield();

				lock (lockObject)
				{
					isResizingClosure = isResizing;
				}
			}

			#endregion

			lock (lockObject)
			{
				if (pool.Count != 0)
				{
					result = pool.Dequeue();

					return result;
				}

				isResizing = true;
			}

			capacity = await QueuePoolFactory.ResizeAsyncQueuePool(
				pool,
				capacity,
				allocationCommand,
				logger,

				asyncContext);

			lock (lockObject)
			{
				result = pool.Dequeue();

				isResizing = false;
			}

			return result;
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
			#region Wait for resize

			bool isResizingClosure = false;

			lock (lockObject)
			{
				isResizingClosure = isResizing;
			}

			while (isResizingClosure)
			{
				await Task.Yield();

				lock (lockObject)
				{
					isResizingClosure = isResizing;
				}
			}

			#endregion

			lock (lockObject)
			{
				pool.Enqueue(instance);
			}
		}

		#endregion

		#region IAsyncAllocationResizeable

		public async Task Resize(

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			#region Wait for resize

			bool isResizingClosure = false;

			lock (lockObject)
			{
				isResizingClosure = isResizing;
			}

			while (isResizingClosure)
			{
				await Task.Yield();

				lock (lockObject)
				{
					isResizingClosure = isResizing;
				}
			}

			#endregion

			lock (lockObject)
			{
				isResizing = true;
			}

			capacity = await QueuePoolFactory.ResizeAsyncQueuePool(
				pool,
				capacity,
				allocationCommand,
				logger,

				asyncContext);

			lock (lockObject)
			{
				isResizing = false;
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