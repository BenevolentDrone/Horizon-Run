using System;

using HereticalSolutions.Allocations;

using HereticalSolutions.Time;
using HereticalSolutions.Time.Factories;

using HereticalSolutions.Logging;
using ILogger = HereticalSolutions.Logging.ILogger;

namespace HereticalSolutions.Pools.AllocationCallbacks
{
	public class SetRuntimeTimerCallback<T>
		: IAllocationCallback<IPoolElementFacade<T>>
	{
		private readonly ILoggerResolver loggerResolver;

		private readonly ILogger logger;

		public string ID { get; set; }

		public float DefaultDuration { get; set; }

		public SetRuntimeTimerCallback(
			ILoggerResolver loggerResolver,
			ILogger logger,

			string id = TimerDecoratorConsts.POOL_ELEMENT_METADATA_TIMER_ID,
			float defaultDuration = 0f)
		{
			ID = id;

			DefaultDuration = defaultDuration;

			this.loggerResolver = loggerResolver;
			
			this.logger = logger;
		}

		public void OnAllocated(
			IPoolElementFacade<T> poolElementFacade)
		{
			IPoolElementFacadeWithMetadata<T> facadeWithMetadata =
				poolElementFacade as IPoolElementFacadeWithMetadata<T>;

			if (facadeWithMetadata == null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"POOL ELEMENT FACADE HAS NO METADATA"));
			}
			
			var metadata = (RuntimeTimerMetadata)
				facadeWithMetadata.Metadata.Get<IContainsRuntimeTimer>();

			if (metadata == null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"POOL ELEMENT FACADE HAS NO TIMER METADATA"));
			}
			
			// Set the runtime timer
			var timer = TimerFactory.BuildRuntimeTimer(
				ID,
				DefaultDuration,
				loggerResolver);

			metadata.RuntimeTimer = timer;
		}
	}
}