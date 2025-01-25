using System;

using HereticalSolutions.Allocations;

using HereticalSolutions.Pools;
using HereticalSolutions.Pools.Factories;
using HereticalSolutions.Pools.AllocationCallbacks;

using HereticalSolutions.Metadata.Allocations;

using HereticalSolutions.Synchronization;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Time.Factories
{
	public static class TimerPoolFactory
	{
		#region Factory settings

		#region Timer pool

		public const int DEFAULT_TIMER_POOL_SIZE = 32;

		public static AllocationCommandDescriptor TimerPoolInitialAllocationDescriptor =
			new AllocationCommandDescriptor
			{
				Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

				Amount = DEFAULT_TIMER_POOL_SIZE
			};

		public static AllocationCommandDescriptor TimerPoolAdditionalAllocationDescriptor =
			new AllocationCommandDescriptor
			{
				Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

				Amount = DEFAULT_TIMER_POOL_SIZE
			};

		#endregion

		#endregion

		public static IManagedPool<TimerWithSubscriptionsContainer> BuildRuntimeTimerPool(
			ISynchronizationProvider provider,
			ILoggerResolver loggerResolver)
		{
			return BuildRuntimeTimerPool(
				provider,
				TimerPoolInitialAllocationDescriptor,
				TimerPoolAdditionalAllocationDescriptor,
				loggerResolver);
		}

		public static IManagedPool<TimerWithSubscriptionsContainer> BuildRuntimeTimerPool(
			ISynchronizationProvider provider,
			AllocationCommandDescriptor initialAllocationDescriptor,
			AllocationCommandDescriptor additionalAllocationDescriptor,
			ILoggerResolver loggerResolver)
		{
			#region Builders

			var managedPoolBuilder = new ManagedPoolBuilder<TimerWithSubscriptionsContainer>(
				loggerResolver,
				loggerResolver?.GetLogger<ManagedPoolBuilder<TimerWithSubscriptionsContainer>>());

			#endregion

			#region Callbacks

			// Create a push to decorated pool callback
			PushToManagedPoolWhenAvailableCallback<TimerWithSubscriptionsContainer> pushCallback =
				ObjectPoolAllocationCallbacksFactory.BuildPushToManagedPoolWhenAvailableCallback<TimerWithSubscriptionsContainer>();

			#endregion

			#region Metadata descriptor builders

			// Create an array of metadata descriptor builder functions
			var metadataDescriptorBuilders = new Func<MetadataAllocationDescriptor>[]
			{
				//ObjectPoolMetadataFactory.BuildIndexedMetadataDescriptor
			};

			#endregion

			// Create a value allocation delegate
			Func<TimerWithSubscriptionsContainer> valueAllocationDelegate =
				() => 
				{
					var result = TimerFactory.BuildRuntimeTimerWithSubscriptionsContainer(
						provider,
						loggerResolver: loggerResolver);

					result.Timer.Repeat = false;
			
					result.Timer.Accumulate = false;
			
					result.Timer.FlushTimeElapsedOnRepeat = false;

					result.Timer.FireRepeatCallbackOnFinish = true;
					
					return result;
				};

			#region Allocation callbacks initialization

			// Create an array of allocation callbacks	
			var facadeAllocationCallbacks =
				new IAllocationCallback<IPoolElementFacade<TimerWithSubscriptionsContainer>>[]
				{
					pushCallback
				};

			#endregion

			// Initialize the resizable pool builder
			managedPoolBuilder.Initialize(
				valueAllocationDelegate,

				metadataDescriptorBuilders,

				initialAllocationDescriptor,
				additionalAllocationDescriptor,

				facadeAllocationCallbacks,
				null);

			// Build the resizable pool
			var resizablePool = managedPoolBuilder.BuildLinkedListManagedPool();

			// Set the root of the push callback
			pushCallback.TargetPool = resizablePool;

			return resizablePool;
		}
	}
}