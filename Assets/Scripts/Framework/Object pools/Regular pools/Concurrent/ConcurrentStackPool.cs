using System;
using System.Collections.Generic;

using HereticalSolutions.Allocations;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools
{
	public class ConcurrentStackPool<T>
		: IPool<T>,
		  IAllocationResizeable,
		  ICleanuppable,
		  IDisposable
	{
		private readonly Stack<T> pool;

		private readonly AllocationCommand<T> allocationCommand;

		private readonly object lockObject;

		private readonly ILogger logger;

		private int capacity;

		public ConcurrentStackPool(
			Stack<T> pool,
			AllocationCommand<T> allocationCommand,
			ILogger logger = null)
		{
			this.pool = pool;

			this.allocationCommand = allocationCommand;

			this.logger = logger;


			lockObject = new object();

			
			capacity = this.pool.Count;
		}

		#region IPool

		public T Pop()
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
					capacity = StackPoolFactory.ResizeStackPool(
						pool,
						capacity,
						allocationCommand,
						logger);

					result = pool.Pop();
				}

				return result;
			}
		}

		public T Pop(IPoolPopArgument[] args)
		{
			return Pop();
		}

		public void Push(T instance)
		{
			lock (lockObject)
			{
				pool.Push(instance);
			}
		}

		#endregion

		#region IResizeable

		public void Resize()
		{
			lock (lockObject)
			{
				capacity = StackPoolFactory.ResizeStackPool(
					pool,
					capacity,
					allocationCommand,
					logger);
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