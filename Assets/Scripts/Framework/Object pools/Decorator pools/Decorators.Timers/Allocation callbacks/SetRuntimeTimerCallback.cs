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
		public string ID { get; set; }

		public float DefaultDuration { get; set; }

		private ILoggerResolver loggerResolver;

		private ILogger logger;

		public SetRuntimeTimerCallback(
			string id = TimerDecoratorConsts.POOL_ELEMENT_METADATA_TIMER_ID,
			float defaultDuration = 0f,
			ILoggerResolver loggerResolver)
		{
			ID = id;

			DefaultDuration = defaultDuration;

			this.loggerResolver = loggerResolver;
			
			logger = this.loggerResolver?.GetLogger<SetRuntimeTimerCallback<T>>();
		}

		public void OnAllocated(IPoolElementFacade<T> poolElementFacade)
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