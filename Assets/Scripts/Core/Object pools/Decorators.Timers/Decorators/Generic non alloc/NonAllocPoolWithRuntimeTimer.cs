using System;

using HereticalSolutions.Pools.Arguments;

using HereticalSolutions.Time;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Pools.Decorators
{
	public class NonAllocPoolWithRuntimeTimer<T> : ANonAllocDecoratorPool<T>
	{
		private ITimerManager timerManager;
		
		public NonAllocPoolWithRuntimeTimer(
			INonAllocDecoratedPool<T> innerPool,
			ITimerManager timerManager,
			ILogger logger = null)
			: base(
				innerPool,
				logger)
		{
			this.timerManager = timerManager;
		}
		
		protected override void OnAfterPop(
			IPoolElement<T> instance,
			IPoolDecoratorArgument[] args)
		{
			if (!instance.Metadata.Has<IContainsRuntimeTimer>())
				throw new Exception(
					logger.TryFormat<NonAllocPoolWithRuntimeTimer<T>>(
						"INVALID INSTANCE"));

			//Get metadata
			var metadata = instance.Metadata.Get<IContainsRuntimeTimer>();

			var metadataWithPushSubscription = metadata as IPushOnTimerFinish;


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
					out var timerID,
					out timer);

				metadata.RuntimeTimer = timer;

				metadataWithPushSubscription.TimerID = timerID;

				timer.OnFinish.Subscribe(
					metadataWithPushSubscription.PushSubscription);
			}
			else
			{
				timer = metadata.RuntimeTimer;
			}


			if (timer == null)
				throw new Exception(
					logger.TryFormat<NonAllocPoolWithRuntimeTimer<T>>(
						"INVALID TIMER"));
			

			timer.Reset(duration);

			timer.Start();
		}

		/// <summary>
		/// Callback method called before an object is pushed back into the pool.
		/// </summary>
		/// <param name="instance">The instance that will be pushed back into the pool.</param>
		protected override void OnBeforePush(IPoolElement<T> instance)
		{
			if (!instance.Metadata.Has<IContainsRuntimeTimer>())
				throw new Exception(
					logger.TryFormat<NonAllocPoolWithRuntimeTimer<T>>(
						"INVALID INSTANCE"));

			var metadata = instance.Metadata.Get<IContainsRuntimeTimer>();

			var metadataWithPushSubscription = metadata as IPushOnTimerFinish;


			var timer = metadata.RuntimeTimer;

			if (timer != null)
			{
				timer.Reset();


				if (metadataWithPushSubscription != null)
				{
					if (metadataWithPushSubscription.PushSubscription.Active)
						timer.OnFinish.Unsubscribe(metadataWithPushSubscription.PushSubscription);

					timerManager.TryDestroyTimer(
						metadataWithPushSubscription.TimerID);

					metadata.RuntimeTimer = null;

					metadataWithPushSubscription.TimerID = -1;
				}
			}
		}
	}
}