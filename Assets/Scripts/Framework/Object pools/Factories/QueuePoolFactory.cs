using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using HereticalSolutions.Asynchronous;

using HereticalSolutions.Allocations;
using HereticalSolutions.Allocations.Factories;

using HereticalSolutions.Metadata.Allocations;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Factories
{
	public static class QueuePoolFactory
	{
		#region Build

		#region Queue pool

		public static QueuePool<T> BuildQueuePool<T>(
			AllocationCommand<T> initialAllocationCommand,
			AllocationCommand<T> additionalAllocationCommand,
			ILoggerResolver loggerResolver)
		{
			ILogger logger =
				loggerResolver?.GetLogger<QueuePool<T>>();

			var queue = new Queue<T>();

			PerformInitialAllocation<T>(
				queue,
				initialAllocationCommand,
				logger);

			return new QueuePool<T>(
				queue,
				additionalAllocationCommand,
				logger);
		}

		private static void PerformInitialAllocation<T>(
			Queue<T> queue,
			AllocationCommand<T> initialAllocationCommand,
			ILogger logger)
		{
			int initialAmount = -1;

			switch (initialAllocationCommand.Descriptor.Rule)
			{
				case EAllocationAmountRule.ZERO:
					initialAmount = 0;
					break;

				case EAllocationAmountRule.ADD_ONE:
					initialAmount = 1;
					break;

				case EAllocationAmountRule.ADD_PREDEFINED_AMOUNT:
					initialAmount = initialAllocationCommand.Descriptor.Amount;
					break;

				default:
					throw new Exception(
						logger.TryFormatException(
							$"[QueuePoolFactory] INVALID INITIAL ALLOCATION COMMAND RULE: {initialAllocationCommand.Descriptor.Rule.ToString()}"));
			}

			for (int i = 0; i < initialAmount; i++)
			{
				var newElement = initialAllocationCommand.AllocationDelegate();

				initialAllocationCommand.AllocationCallback?.OnAllocated(newElement);

				queue.Enqueue(
					newElement);
			}
		}

		#endregion

		#region Concurrent queue pool

		public static ConcurrentQueuePool<T> BuildConcurrentQueuePool<T>(
			AllocationCommand<T> initialAllocationCommand,
			AllocationCommand<T> additionalAllocationCommand,
			ILoggerResolver loggerResolver)
		{
			ILogger logger =
				loggerResolver?.GetLogger<ConcurrentQueuePool<T>>();

			var queue = new Queue<T>();

			PerformInitialAllocation<T>(
				queue,
				initialAllocationCommand,
				logger);

			return new ConcurrentQueuePool<T>(
				queue,
				additionalAllocationCommand,
				logger);
		}

		#endregion

		#region Async queue pool

		public static async Task<AsyncQueuePool<T>> BuildAsyncQueuePool<T>(
			AsyncAllocationCommand<T> initialAllocationCommand,
			AsyncAllocationCommand<T> additionalAllocationCommand,
			ILoggerResolver loggerResolver,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			ILogger logger =
				loggerResolver?.GetLogger<AsyncQueuePool<T>>();

			var queue = new Queue<T>();

			await PerformInitialAllocation<T>(
				queue,
				initialAllocationCommand,
				logger,
				
				asyncContext);

			return new AsyncQueuePool<T>(
				queue,
				additionalAllocationCommand,
				logger);
		}

		private static async Task PerformInitialAllocation<T>(
			Queue<T> queue,
			AsyncAllocationCommand<T> initialAllocationCommand,
			ILogger logger,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			int initialAmount = -1;

			switch (initialAllocationCommand.Descriptor.Rule)
			{
				case EAllocationAmountRule.ZERO:
					initialAmount = 0;
					break;

				case EAllocationAmountRule.ADD_ONE:
					initialAmount = 1;
					break;

				case EAllocationAmountRule.ADD_PREDEFINED_AMOUNT:
					initialAmount = initialAllocationCommand.Descriptor.Amount;
					break;

				default:
					throw new Exception(
						logger.TryFormatException(
							$"[QueuePoolFactory] INVALID INITIAL ALLOCATION COMMAND RULE: {initialAllocationCommand.Descriptor.Rule.ToString()}"));
			}

			for (int i = 0; i < initialAmount; i++)
			{
				var newElement = await initialAllocationCommand.AllocationDelegate();

				await initialAllocationCommand.AllocationCallback?.OnAllocated(
					newElement,
					asyncContext);

				queue.Enqueue(
					newElement);
			}
		}

		#endregion

		#region Queue managed pool

		public static QueueManagedPool<T> BuildQueueManagedPool<T>(
			AllocationCommand<T> initialAllocationCommand,
			AllocationCommand<T> additionalAllocationCommand,
			ILoggerResolver loggerResolver,

			MetadataAllocationDescriptor[] metadataAllocationDescriptors = null,
			IAllocationCallback<IPoolElementFacade<T>> facadeAllocationCallback = null)
		{
			ILogger logger =
				loggerResolver?.GetLogger<QueueManagedPool<T>>();

			var queue = new Queue<IPoolElementFacade<T>>();

			Func<IPoolElementFacade<T>> facadeAllocationDelegate =
				() => ObjectPoolAllocationFactory.BuildPoolElementFacade<T>(
					metadataAllocationDescriptors);

			AllocationCommand<IPoolElementFacade<T>> initialFacadeAllocationCommand =
				ObjectPoolAllocationCommandFactory.BuildPoolElementFacadeAllocationCommand(
					initialAllocationCommand.Descriptor,
					facadeAllocationDelegate,
					facadeAllocationCallback);

			AllocationCommand<IPoolElementFacade<T>> additionalFacadeAllocationCommand =
				ObjectPoolAllocationCommandFactory.BuildPoolElementFacadeAllocationCommand(
					additionalAllocationCommand.Descriptor,
					facadeAllocationDelegate,
					facadeAllocationCallback);

			PerformInitialAllocation<T>(
				queue,
				initialFacadeAllocationCommand,
				initialAllocationCommand,
				logger);

			return new QueueManagedPool<T>(
				queue,
				additionalFacadeAllocationCommand,
				additionalAllocationCommand,
				logger);
		}

		private static void PerformInitialAllocation<T>(
			Queue<IPoolElementFacade<T>> queue,
			AllocationCommand<IPoolElementFacade<T>> facadeAllocationCommand,
			AllocationCommand<T> valueAllocationCommand,
			ILogger logger)
		{
			int initialAmount = -1;

			switch (facadeAllocationCommand.Descriptor.Rule)
			{
				case EAllocationAmountRule.ZERO:
					initialAmount = 0;
					break;

				case EAllocationAmountRule.ADD_ONE:
					initialAmount = 1;
					break;

				case EAllocationAmountRule.ADD_PREDEFINED_AMOUNT:
					initialAmount = facadeAllocationCommand.Descriptor.Amount;
					break;

				default:
					throw new Exception(
						logger.TryFormatException(
							$"[QueuePoolFactory] INVALID INITIAL ALLOCATION COMMAND RULE: {facadeAllocationCommand.Descriptor.Rule.ToString()}"));
			}

			for (int i = 0; i < initialAmount; i++)
			{
				var newElementFacade = facadeAllocationCommand.AllocationDelegate();

				//MOVING IT AFTER THE VALUE ALLOCATION BECAUSE SOME WRAPPER PUSH LOGIC MAY DEPEND ON THE VALUE
				//facadeAllocationCommand.AllocationCallback?.OnAllocated(newElementFacade);

				var newElementValue = valueAllocationCommand.AllocationDelegate();

				valueAllocationCommand.AllocationCallback?.OnAllocated(
					newElementValue);

				newElementFacade.Value = newElementValue;

				//THIS SHOULD BE SET BEFORE ALLOCATION CALLBACK TO ENSURE THAT ELEMENTS ALREADY PRESENT ARE NOT PUSHED TWICE
				newElementFacade.Status = EPoolElementStatus.PUSHED;

				facadeAllocationCommand.AllocationCallback?.OnAllocated(newElementFacade);

				queue.Enqueue(
					newElementFacade);
			}
		}

		#endregion

		#region Concurrent queue managed pool

		public static ConcurrentQueueManagedPool<T> BuildConcurrentQueueManagedPool<T>(
			AllocationCommand<T> initialAllocationCommand,
			AllocationCommand<T> additionalAllocationCommand,
			ILoggerResolver loggerResolver,

			MetadataAllocationDescriptor[] metadataAllocationDescriptors = null,
			IAllocationCallback<IPoolElementFacade<T>> facadeAllocationCallback = null)
		{
			ILogger logger =
				loggerResolver?.GetLogger<ConcurrentQueueManagedPool<T>>();

			var queue = new Queue<IPoolElementFacade<T>>();

			Func<IPoolElementFacade<T>> facadeAllocationDelegate =
				() => ObjectPoolAllocationFactory.BuildPoolElementFacade<T>(
					metadataAllocationDescriptors);

			AllocationCommand<IPoolElementFacade<T>> initialFacadeAllocationCommand =
				ObjectPoolAllocationCommandFactory.BuildPoolElementFacadeAllocationCommand(
					initialAllocationCommand.Descriptor,
					facadeAllocationDelegate,
					facadeAllocationCallback);

			AllocationCommand<IPoolElementFacade<T>> additionalFacadeAllocationCommand =
				ObjectPoolAllocationCommandFactory.BuildPoolElementFacadeAllocationCommand(
					additionalAllocationCommand.Descriptor,
					facadeAllocationDelegate,
					facadeAllocationCallback);

			PerformInitialAllocation<T>(
				queue,
				initialFacadeAllocationCommand,
				initialAllocationCommand,
				logger);

			return new ConcurrentQueueManagedPool<T>(
				queue,
				additionalFacadeAllocationCommand,
				additionalAllocationCommand,
				logger);
		}

		#endregion

		#region Appendable queue managed pool

		public static AppendableQueueManagedPool<T> BuildAppendableQueueManagedPool<T>(
			AllocationCommand<T> initialAllocationCommand,
			AllocationCommand<T> additionalAllocationCommand,
			ILoggerResolver loggerResolver,

			MetadataAllocationDescriptor[] metadataAllocationDescriptors = null,
			IAllocationCallback<IPoolElementFacade<T>> facadeAllocationCallback = null)
		{
			ILogger logger =
				loggerResolver?.GetLogger<AppendableQueueManagedPool<T>>();

			var queue = new Queue<IPoolElementFacade<T>>();

			Func<IPoolElementFacade<T>> facadeAllocationDelegate =
				() => ObjectPoolAllocationFactory.BuildPoolElementFacade<T>(
					metadataAllocationDescriptors);

			AllocationCommand<IPoolElementFacade<T>> initialFacadeAllocationCommand =
				ObjectPoolAllocationCommandFactory.BuildPoolElementFacadeAllocationCommand(
					initialAllocationCommand.Descriptor,
					facadeAllocationDelegate,
					facadeAllocationCallback);

			AllocationCommand<IPoolElementFacade<T>> additionalFacadeAllocationCommand =
				ObjectPoolAllocationCommandFactory.BuildPoolElementFacadeAllocationCommand(
					additionalAllocationCommand.Descriptor,
					facadeAllocationDelegate,
					facadeAllocationCallback);

			AllocationCommand<T> nullValueAllocationCommand =
				new AllocationCommand<T>
				{
					Descriptor = new AllocationCommandDescriptor
					{
						Rule = EAllocationAmountRule.ADD_ONE,

						Amount = 1
					},

					AllocationDelegate = AllocationFactory.NullAllocationDelegate<T>
				};

			AllocationCommand<IPoolElementFacade<T>> appendFacadeAllocationCommand =
				ObjectPoolAllocationCommandFactory.BuildPoolElementFacadeAllocationCommand(
					nullValueAllocationCommand.Descriptor,
					facadeAllocationDelegate,
					facadeAllocationCallback);

			PerformInitialAllocation<T>(
				queue,
				initialFacadeAllocationCommand,
				initialAllocationCommand,
				logger);

			return new AppendableQueueManagedPool<T>(
				queue,
				additionalFacadeAllocationCommand,
				additionalAllocationCommand,

				appendFacadeAllocationCommand,
				nullValueAllocationCommand,

				logger);
		}

		#endregion

		#region Concurrent appendable queue managed pool

		public static ConcurrentAppendableQueueManagedPool<T> BuildConcurrentAppendableQueueManagedPool<T>(
			AllocationCommand<T> initialAllocationCommand,
			AllocationCommand<T> additionalAllocationCommand,
			ILoggerResolver loggerResolver,

			MetadataAllocationDescriptor[] metadataAllocationDescriptors = null,
			IAllocationCallback<IPoolElementFacade<T>> facadeAllocationCallback = null)
		{
			ILogger logger =
				loggerResolver?.GetLogger<ConcurrentAppendableQueueManagedPool<T>>();

			var queue = new Queue<IPoolElementFacade<T>>();

			Func<IPoolElementFacade<T>> facadeAllocationDelegate =
				() => ObjectPoolAllocationFactory.BuildPoolElementFacade<T>(
					metadataAllocationDescriptors);

			AllocationCommand<IPoolElementFacade<T>> initialFacadeAllocationCommand =
				ObjectPoolAllocationCommandFactory.BuildPoolElementFacadeAllocationCommand(
					initialAllocationCommand.Descriptor,
					facadeAllocationDelegate,
					facadeAllocationCallback);

			AllocationCommand<IPoolElementFacade<T>> additionalFacadeAllocationCommand =
				ObjectPoolAllocationCommandFactory.BuildPoolElementFacadeAllocationCommand(
					additionalAllocationCommand.Descriptor,
					facadeAllocationDelegate,
					facadeAllocationCallback);

			AllocationCommand<T> nullValueAllocationCommand =
				new AllocationCommand<T>
				{
					Descriptor = new AllocationCommandDescriptor
					{
						Rule = EAllocationAmountRule.ADD_ONE,

						Amount = 1
					},

					AllocationDelegate = AllocationFactory.NullAllocationDelegate<T>
				};

			AllocationCommand<IPoolElementFacade<T>> appendFacadeAllocationCommand =
				ObjectPoolAllocationCommandFactory.BuildPoolElementFacadeAllocationCommand(
					nullValueAllocationCommand.Descriptor,
					facadeAllocationDelegate,
					facadeAllocationCallback);

			PerformInitialAllocation<T>(
				queue,
				initialFacadeAllocationCommand,
				initialAllocationCommand,
				logger);

			return new ConcurrentAppendableQueueManagedPool<T>(
				queue,
				additionalFacadeAllocationCommand,
				additionalAllocationCommand,

				appendFacadeAllocationCommand,
				nullValueAllocationCommand,

				logger);
		}

		#endregion

		#endregion

		#region Resize

		public static int ResizeQueuePool<T>(
			Queue<T> queue,
			int currentCapacity,
			AllocationCommand<T> allocationCommand,
			ILogger logger)
		{
			int addedCapacity = -1;

			switch (allocationCommand.Descriptor.Rule)
			{
				case EAllocationAmountRule.ADD_ONE:
					addedCapacity = 1;
					break;

				case EAllocationAmountRule.DOUBLE_AMOUNT:
					addedCapacity = currentCapacity * 2;
					break;

				case EAllocationAmountRule.ADD_PREDEFINED_AMOUNT:
					addedCapacity = allocationCommand.Descriptor.Amount;
					break;

				default:
					throw new Exception(
						logger.TryFormatException(
							$"[QueuePoolFactory] INVALID RESIZE ALLOCATION COMMAND RULE FOR STACK: {allocationCommand.Descriptor.Rule.ToString()}"));
			}

			for (int i = 0; i < addedCapacity; i++)
			{
				var newElement = allocationCommand.AllocationDelegate();

				allocationCommand.AllocationCallback?.OnAllocated(newElement);

				queue.Enqueue(
					newElement);
			}

			return currentCapacity + addedCapacity;
		}

		public static async Task<int> ResizeAsyncQueuePool<T>(
			Queue<T> queue,
			int currentCapacity,
			AsyncAllocationCommand<T> allocationCommand,
			ILogger logger,

			//Async tail
			AsyncExecutionContext asyncContext)
		{
			int addedCapacity = -1;

			switch (allocationCommand.Descriptor.Rule)
			{
				case EAllocationAmountRule.ADD_ONE:
					addedCapacity = 1;
					break;

				case EAllocationAmountRule.DOUBLE_AMOUNT:
					addedCapacity = currentCapacity * 2;
					break;

				case EAllocationAmountRule.ADD_PREDEFINED_AMOUNT:
					addedCapacity = allocationCommand.Descriptor.Amount;
					break;

				default:
					throw new Exception(
						logger.TryFormatException(
							$"[QueuePoolFactory] INVALID RESIZE ALLOCATION COMMAND RULE FOR STACK: {allocationCommand.Descriptor.Rule.ToString()}"));
			}

			for (int i = 0; i < addedCapacity; i++)
			{
				var newElement = await allocationCommand.AllocationDelegate();

				await allocationCommand.AllocationCallback?.OnAllocated(
					newElement,
					asyncContext);

				queue.Enqueue(
					newElement);
			}

			return currentCapacity + addedCapacity;
		}

		public static int ResizeQueueManagedPool<T>(
			Queue<IPoolElementFacade<T>> queue,
			int currentCapacity,
			AllocationCommand<IPoolElementFacade<T>> facadeAllocationCommand,
			AllocationCommand<T> valueAllocationCommand,
			ILogger logger)
		{
			int addedCapacity = -1;

			switch (facadeAllocationCommand.Descriptor.Rule)
			{
				case EAllocationAmountRule.ADD_ONE:
					addedCapacity = 1;
					break;

				case EAllocationAmountRule.DOUBLE_AMOUNT:
					addedCapacity = currentCapacity * 2;
					break;

				case EAllocationAmountRule.ADD_PREDEFINED_AMOUNT:
					addedCapacity = facadeAllocationCommand.Descriptor.Amount;
					break;

				default:
					throw new Exception(
						logger.TryFormatException(
							$"[QueuePoolFactory] INVALID RESIZE ALLOCATION COMMAND RULE FOR STACK: {facadeAllocationCommand.Descriptor.Rule.ToString()}"));
			}

			for (int i = 0; i < addedCapacity; i++)
			{
				var newElement = facadeAllocationCommand.AllocationDelegate();

				//MOVING IT AFTER THE VALUE ALLOCATION BECAUSE SOME WRAPPER PUSH LOGIC MAY DEPEND ON THE VALUE
				//facadeAllocationCommand.AllocationCallback?.OnAllocated(newElement);

				var newElementValue = valueAllocationCommand.AllocationDelegate();

				valueAllocationCommand.AllocationCallback?.OnAllocated(
					newElementValue);

				newElement.Value = newElementValue;

				//THIS SHOULD BE SET BEFORE ALLOCATION CALLBACK TO ENSURE THAT ELEMENTS ALREADY PRESENT ARE NOT PUSHED TWICE
				newElement.Status = EPoolElementStatus.PUSHED;

				facadeAllocationCommand.AllocationCallback?.OnAllocated(newElement);

				queue.Enqueue(
					newElement);
			}

			return currentCapacity + addedCapacity;
		}

		#endregion
	}
}