using HereticalSolutions.Time;
using HereticalSolutions.Time.Factories;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.AllocationCallbacks
{
	public class SetRuntimeTimerCallback<T> : IAllocationCallback<T>
	{
		/// <summary>
		/// Gets or sets the ID of the runtime timer.
		/// </summary>
		public string ID { get; set; }

		/// <summary>
		/// Gets or sets the default duration for the runtime timer.
		/// </summary>
		public float DefaultDuration { get; set; }

		private ILoggerResolver loggerResolver;

		public SetRuntimeTimerCallback(
			string id = TimerConsts.ANONYMOUS_TIMER_ID,
			float defaultDuration = 0f,
			ILoggerResolver loggerResolver = null)
		{
			ID = id;

			DefaultDuration = defaultDuration;

			this.loggerResolver = loggerResolver;
		}

		public void OnAllocated(IPoolElement<T> currentElement)
		{
			//SUPPLY AND MERGE POOLS DO NOT PRODUCE ELEMENTS WITH VALUES
			//if (currentElement.Value == null)
			//    return;

			var metadata = (RuntimeTimerMetadata)
				currentElement.Metadata.Get<IContainsRuntimeTimer>();

			// Set the runtime timer
			var timer = TimeFactory.BuildRuntimeTimer(
				ID,
				DefaultDuration,
				loggerResolver);

			metadata.RuntimeTimer = timer;
		}
	}
}