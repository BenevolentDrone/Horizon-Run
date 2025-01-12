using System;

using System.Collections.Generic;

using HereticalSolutions.Allocations;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools
{
	public class QueuePool<T>
		: IPool<T>,
		  IAllocationResizeable,
		  ICleanuppable,
		  IDisposable
	{
		private readonly Queue<T> pool;

		private readonly AllocationCommand<T> allocationCommand;

		private readonly ILogger logger;

		private int capacity;

		public QueuePool(
			Queue<T> pool,
			AllocationCommand<T> allocationCommand,
			ILogger logger = null)
		{
			this.pool = pool;

			this.allocationCommand = allocationCommand;

			this.logger = logger;

			capacity = this.pool.Count;
		}

		#region IPool

		public T Pop()
		{
			T result = default(T);

			if (pool.Count != 0)
			{
				result = pool.Dequeue();
			}
			else
			{
				capacity = QueuePoolFactory.ResizeQueuePool(
					pool,
					capacity,
					allocationCommand,
					logger);

				result = pool.Dequeue();
			}

			return result;
		}

		public T Pop(
			IPoolPopArgument[] args)
		{
			return Pop();
		}

		public void Push(
			T instance)
		{
			pool.Enqueue(instance);
		}

		#endregion

		#region IAllocationResizeable

		public void Resize()
		{
			capacity = QueuePoolFactory.ResizeQueuePool(
				pool,
				capacity,
				allocationCommand,
				logger);
		}

		#endregion

		#region ICleanUppable

		public void Cleanup()
		{
			foreach (var item in pool)
				if (item is ICleanuppable)
					(item as ICleanuppable).Cleanup();

			pool.Clear();
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			foreach (var item in pool)
				if (item is IDisposable)
					(item as IDisposable).Dispose();

			pool.Clear();
		}

		#endregion
	}
}