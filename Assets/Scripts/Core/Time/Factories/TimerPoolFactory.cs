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
		public const int INITIAL_TIMERS_AMOUNT = 100;

		public const int ADDITIONAL_TIMERS_AMOUNT = 100;

		public static INonAllocDecoratedPool<TimerWithSubscriptionsContainer> BuildRuntimeTimersPool(
			ISynchronizationProvider provider,
			ILoggerResolver loggerResolver = null)
		{
			#region Builders

			// Create a builder for resizable pools.
			var resizablePoolBuilder = new ResizablePoolBuilder<TimerWithSubscriptionsContainer>(
				loggerResolver,
				loggerResolver?.GetLogger<ResizablePoolBuilder<TimerWithSubscriptionsContainer>>());

			#endregion

			#region Callbacks

			// Create a push to decorated pool callback.
			PushToDecoratedPoolCallback<TimerWithSubscriptionsContainer> pushCallback =
				PoolsFactory.BuildPushToDecoratedPoolCallback<TimerWithSubscriptionsContainer>(
					PoolsFactory.BuildDeferredCallbackQueue<TimerWithSubscriptionsContainer>());

			#endregion

			#region Metadata descriptor builders

			// Create an array of metadata descriptor builder functions.
			var metadataDescriptorBuilders = new Func<MetadataAllocationDescriptor>[]
			{
				PoolsFactory.BuildIndexedMetadataDescriptor
			};

			#endregion

			// Create a value allocation delegate.
			Func<TimerWithSubscriptionsContainer> valueAllocationDelegate =
				() => 
				{
					var result = TimeFactory.BuildRuntimeTimerWithSubscriptionsContainer(
						provider,
						loggerResolver: loggerResolver);

					result.Timer.Repeat = false;
			
					result.Timer.Accumulate = false;
			
					result.Timer.FlushTimeElapsedOnRepeat = false;
					
					return result;
				};

			#region Allocation callbacks initialization

			// Create an array of allocation callbacks.
			var callbacks = new IAllocationCallback<TimerWithSubscriptionsContainer>[]
			{
				pushCallback
			};

			#endregion

			// Initialize the resizable pool builder
			resizablePoolBuilder.Initialize(
				valueAllocationDelegate,
				true,

				metadataDescriptorBuilders,

				new AllocationCommandDescriptor
				{
					Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

					Amount = INITIAL_TIMERS_AMOUNT
				},
				new AllocationCommandDescriptor
				{
					Rule = EAllocationAmountRule.ADD_PREDEFINED_AMOUNT,

					Amount = ADDITIONAL_TIMERS_AMOUNT
				
				},

				callbacks);

			// Build the resizable pool.
			var resizablePool = resizablePoolBuilder.BuildResizablePool();

			// Set the root of the push callback.
			pushCallback.Root = resizablePool;

			return resizablePool;
		}
	}
}