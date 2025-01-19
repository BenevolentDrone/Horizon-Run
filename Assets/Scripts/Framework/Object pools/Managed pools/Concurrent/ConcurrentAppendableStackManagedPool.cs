using System.Collections.Generic;

using HereticalSolutions.Allocations;

using HereticalSolutions.Pools.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools
{
	public class ConcurrentAppendableStackManagedPool<T>
		: ConcurrentStackManagedPool<T>
	{
		private readonly AllocationCommand<IPoolElementFacade<T>> appendFacadeAllocationCommand;

		private readonly AllocationCommand<T> nullValueAllocationCommand;

		public ConcurrentAppendableStackManagedPool(
			Stack<IPoolElementFacade<T>> pool,
			AllocationCommand<IPoolElementFacade<T>> facadeAllocationCommand,
			AllocationCommand<T> valueAllocationCommand,

			AllocationCommand<IPoolElementFacade<T>> appendFacadeAllocationCommand,
			AllocationCommand<T> nullValueAllocationCommand,

			ILogger logger)
			: base (
				pool,
				facadeAllocationCommand,
				valueAllocationCommand,
				logger)
		{
			this.appendFacadeAllocationCommand = appendFacadeAllocationCommand;

			this.nullValueAllocationCommand = nullValueAllocationCommand;
		}

		public override IPoolElementFacade<T> Pop(IPoolPopArgument[] args)
		{
			lock (lockObject)
			{
				#region Append from argument

				if (args.TryGetArgument<AppendToPoolArgument>(out var arg))
				{
					logger?.Log(
						GetType(),
						"APPEND ARGUMENT RECEIVED, APPENDING");

					// Resize the stack in a thread-safe manner

					capacity = StackPoolFactory.ResizeStackManagedPool(
						pool,
						capacity,
						appendFacadeAllocationCommand,
						nullValueAllocationCommand,
						logger);

					// Pop an element from the stack

					var result = pool.Pop();

					// Validate the pool

					if (result.Pool == null)
					{
						result.Pool = this;
					}

					// Update the facade's status

					result.Status = EPoolElementStatus.UNINITIALIZED;

					return result;
				}

				#endregion

				// Call the base Pop if no append argument is provided
				
				return base.Pop(args);
			}
		}
	}
}