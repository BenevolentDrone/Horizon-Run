using System;

using HereticalSolutions.Time;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Decorators
{
	public class ManagedPoolWithRuntimeTimer<T>
		: ADecoratorManagedPool<T>
	{
		private ITimerManager timerManager;
		
		public ManagedPoolWithRuntimeTimer(
			IManagedPool<T> innerPool,
			ITimerManager timerManager,
			ILogger logger)
			: base(
				innerPool,
				logger)
		{
			this.timerManager = timerManager;
		}
		
		protected override void OnAfterPop(
			IPoolElementFacade<T> instance,
			IPoolPopArgument[] args)
		{
			IPoolElementFacadeWithMetadata<T> instanceWithMetadata =
				instance as IPoolElementFacadeWithMetadata<T>;

			if (instanceWithMetadata == null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"POOL ELEMENT FACADE HAS NO METADATA"));
			}
			
			if (!instanceWithMetadata.Metadata.Has<IContainsRuntimeTimer>())
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"POOL ELEMENT FACADE HAS NO TIMER METADATA"));

			//Get metadata

			var metadata = instanceWithMetadata.Metadata.Get<IContainsRuntimeTimer>();

			var metadataWithPushSubscription = metadata as IPushableOnTimerFinish;


			//Calculate duration

			float duration = 0f;

			if (metadataWithPushSubscription != null)
				duration = metadataWithPushSubscription.Duration;
			else if (metadata.RuntimeTimer != null)
				duration = metadata.RuntimeTimer.CurrentDuration;

			if (args.TryGetArgument<DurationArgument>(out var arg))
			{
				duration = arg.Duration;
			}


			//Early return

			if (duration < 0f)
				return;


			//Get the timer

			IRuntimeTimer timer;

			if (metadataWithPushSubscription != null)
			{
				timerManager.CreateTimer(
					out var timerHandle,
					out timer);
				
				metadata.RuntimeTimer = timer;

				metadataWithPushSubscription.TimerHandle = timerHandle;

				timer.OnFinish.Subscribe(
					metadataWithPushSubscription.PushSubscription);
			}
			else
			{
				timer = metadata.RuntimeTimer;
			}


			if (timer == null)
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"INVALID TIMER"));
			

			timer.Reset(duration);

			timer.Start();
		}

		protected override void OnBeforePush(IPoolElementFacade<T> instance)
		{
			IPoolElementFacadeWithMetadata<T> instanceWithMetadata =
				instance as IPoolElementFacadeWithMetadata<T>;

			if (instanceWithMetadata == null)
			{
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"POOL ELEMENT FACADE HAS NO METADATA"));
			}
			
			if (!instanceWithMetadata.Metadata.Has<IContainsRuntimeTimer>())
				throw new Exception(
					logger.TryFormatException(
						GetType(),
						"POOL ELEMENT FACADE HAS NO TIMER METADATA"));

			var metadata = instanceWithMetadata.Metadata.Get<IContainsRuntimeTimer>();

			var metadataWithPushSubscription = metadata as IPushableOnTimerFinish;


			var timer = metadata.RuntimeTimer;

			if (timer != null)
			{
				timer.Reset();

				if (metadataWithPushSubscription != null)
				{
					if (metadataWithPushSubscription.PushSubscription.Active)
						timer.OnFinish.Unsubscribe(metadataWithPushSubscription.PushSubscription);

					timerManager.TryDestroyTimer(
						metadataWithPushSubscription.TimerHandle);

					metadata.RuntimeTimer = null;

					metadataWithPushSubscription.TimerHandle = 0;
				}
			}
		}
	}
}