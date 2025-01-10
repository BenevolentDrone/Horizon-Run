using System;

using HereticalSolutions.Allocations;

using HereticalSolutions.Collections;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools
{
	public class ConcurrentLinkedListManagedPool<T>
		: IManagedPool<T>,
		  IAllocationResizeable,
		  ICleanuppable,
		  IDisposable
	{
		private readonly AllocationCommand<IPoolElementFacade<T>> facadeAllocationCommand;

		private readonly AllocationCommand<T> valueAllocationCommand;

		protected readonly object lockObject;

		protected readonly ILogger logger;

		protected ILinkedListLink<T> firstElement;

		protected int capacity;

		public ConcurrentLinkedListManagedPool(
			AllocationCommand<IPoolElementFacade<T>> facadeAllocationCommand,
			AllocationCommand<T> valueAllocationCommand,
			ILinkedListLink<T> firstElement,
			int capacity,
			ILogger logger = null)
		{
			this.facadeAllocationCommand = facadeAllocationCommand;

			this.valueAllocationCommand = valueAllocationCommand;

			this.logger = logger;


			lockObject = new object();


			this.firstElement = firstElement;

			this.capacity = capacity;
		}

		#region IManagedPool

		public IPoolElementFacade<T> Pop()
		{
			lock (lockObject)
			{
				IPoolElementFacade<T> result = null;

				if (firstElement == null)
				{
					LinkedListPoolFactory.ResizeLinkedListManagedPool(
						ref firstElement,
						ref capacity,
						facadeAllocationCommand,
						valueAllocationCommand,
						logger);
				}

				var poppedElement = firstElement;

				firstElement = poppedElement.Next;

				poppedElement.Previous = null;

				poppedElement.Next = null;

				if (firstElement != null)
					firstElement.Previous = null;

				result = poppedElement as IPoolElementFacade<T>;

				if (result == null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							"LINKED LIST MANAGED POOL ELEMENT IS NOT A POOL ELEMENT FACADE"));
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

		public void Push(IPoolElementFacade<T> instance)
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

				var instanceAsLinkedListLink = instance as ILinkedListLink<T>;

				if (instanceAsLinkedListLink == null)
				{
					throw new Exception(
						logger.TryFormatException(
							GetType(),
							"LINKED LIST MANAGED POOL ELEMENT IS NOT A LINKED LIST LINK"));
				}

				var previousFirstElement = firstElement;

				firstElement = instanceAsLinkedListLink;

				if (previousFirstElement != null)
					previousFirstElement.Previous = firstElement;

				instanceAsLinkedListLink.Previous = null;

				instanceAsLinkedListLink.Next = previousFirstElement;
			}
		}

		#endregion

		#region IResizeable

		public void Resize()
		{
			lock (lockObject)
			{
				LinkedListPoolFactory.ResizeLinkedListManagedPool(
					ref firstElement,
					ref capacity,
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
				var currentLink = firstElement;

				while (currentLink != null)
				{
					var currentElement = currentLink as IPoolElementFacade<T>;

					if (currentElement == null)
					{
						throw new Exception(
							logger.TryFormatException(
								GetType(),
								"LINKED LIST MANAGED POOL ELEMENT IS NOT A POOL ELEMENT FACADE"));
					}

					if (currentElement is ICleanuppable)
						(currentElement as ICleanuppable).Cleanup();

					currentLink = currentLink.Next;
				}
			}
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			lock (lockObject)
			{
				var currentLink = firstElement;

				while (currentLink != null)
				{
					var currentElement = currentLink as IPoolElementFacade<T>;

					if (currentElement == null)
					{
						throw new Exception(
							logger.TryFormatException(
								GetType(),
								"LINKED LIST MANAGED POOL ELEMENT IS NOT A POOL ELEMENT FACADE"));
					}

					if (currentElement is IDisposable)
						(currentElement as IDisposable).Dispose();

					currentLink = currentLink.Next;
				}
			}
		}

		#endregion
	}
}