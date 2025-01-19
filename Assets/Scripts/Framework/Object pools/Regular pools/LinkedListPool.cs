using System;

using System.Collections.Generic;

using HereticalSolutions.Allocations;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools
{
	public class LinkedListPool<T>
		: IPool<T>,
		  IAllocationResizeable,
		  ICleanuppable,
		  IDisposable
	{
		private readonly LinkedList<T> pool;

		private readonly AllocationCommand<T> allocationCommand;

		private readonly ILogger logger;

		private int capacity;

		public LinkedListPool(
			LinkedList<T> pool,
			AllocationCommand<T> allocationCommand,
			ILogger logger)
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
				result = pool.First.Value;

				pool.RemoveFirst();
			}
			else
			{
				capacity = LinkedListPoolFactory.ResizeLinkedListPool(
					pool,
					capacity,
					allocationCommand,
					logger);

				result = pool.First.Value;

				pool.RemoveFirst();
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
			pool.AddFirst(instance);
		}

		#endregion

		#region IAllocationResizeable

		public void Resize()
		{
			capacity = LinkedListPoolFactory.ResizeLinkedListPool(
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