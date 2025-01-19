using System;
using System.Collections.Generic;

using HereticalSolutions.Allocations;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools
{
	public class ConcurrentQueueManagedPool<T>
		: IManagedPool<T>,
		  IAllocationResizeable,
		  ICleanuppable,
		  IDisposable
	{
		protected readonly Queue<IPoolElementFacade<T>> pool;

		private readonly AllocationCommand<IPoolElementFacade<T>> facadeAllocationCommand;

		private readonly AllocationCommand<T> valueAllocationCommand;

		protected readonly object lockObject;

		protected readonly ILogger logger;

		protected int capacity;

		public ConcurrentQueueManagedPool(
			Queue<IPoolElementFacade<T>> pool,
			AllocationCommand<IPoolElementFacade<T>> facadeAllocationCommand,
			AllocationCommand<T> valueAllocationCommand,
			ILogger logger)
		{
			this.pool = pool;

			this.facadeAllocationCommand = facadeAllocationCommand;

			this.valueAllocationCommand = valueAllocationCommand;

			this.logger = logger;


			lockObject = new object();

			 
			capacity = this.pool.Count;
		}

		#region IManagedPool

		public IPoolElementFacade<T> Pop()
		{
			lock (lockObject)
			{
				IPoolElementFacade<T> result = null;

				if (pool.Count != 0)
				{
					result = pool.Dequeue();
				}
				else
				{
					capacity = QueuePoolFactory.ResizeQueueManagedPool(
						pool,
						capacity,
						facadeAllocationCommand,
						valueAllocationCommand,
						logger);

					result = pool.Dequeue();
				}

				// Validate values

				if (result.Status == EPoolElementStatus.UNINITIALIZED)
				{
					var newElement = valueAllocationCommand.AllocationDelegate();
					valueAllocationCommand.AllocationCallback?.OnAllocated(newElement);
					result.Value = newElement;
				}

				// Validate pool

				if (result.Pool == null)
				{
					result.Pool = this;
				}

				// Update facade

				result.Status = EPoolElementStatus.POPPED;

				return result;
			}
		}

		public virtual IPoolElementFacade<T> Pop(
			IPoolPopArgument[] args)
		{
			return Pop();
		}

		public void Push(
			IPoolElementFacade<T> instance)
		{
			lock (lockObject)
			{
				// Validate values

				if (instance.Status != EPoolElementStatus.POPPED)
				{
					return;
				}

				// Update facade

				instance.Status = EPoolElementStatus.PUSHED;

				pool.Enqueue(instance);
			}
		}

		#endregion

		#region IAllocationResizeable

		public void Resize()
		{
			lock (lockObject)
			{
				capacity = QueuePoolFactory.ResizeQueueManagedPool(
					pool,
					capacity,
					facadeAllocationCommand,
					valueAllocationCommand,
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