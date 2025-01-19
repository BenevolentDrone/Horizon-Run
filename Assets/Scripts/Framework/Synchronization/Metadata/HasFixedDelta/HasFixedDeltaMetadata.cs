using System;

using HereticalSolutions.Delegates;
using HereticalSolutions.Delegates.Factories;

using HereticalSolutions.Time;

using HereticalSolutions.LifetimeManagement;

using HereticalSolutions.Logging;

namespace HereticalSolutions.Synchronization
{
	public class HasFixedDeltaMetadata<TDelta>
		: IHasFixedDelta<TDelta>,
		  IPublisherDependencyRecipient<TDelta>,
		  ICleanuppable,
		  IDisposable
	{
		private readonly IRuntimeTimer fixedDeltaTimer;

		private readonly ITickable fixedDeltaTimerAsTickable;

		private readonly Func<TDelta, float> deltaToFloatDelegate;

		private readonly INonAllocSubscription synchronizeFixedScubscription;

		private IPublisherSingleArgGeneric<TDelta> broadcasterAsPublisher;

		public HasFixedDeltaMetadata(
			TDelta fixedDelta,
			IRuntimeTimer fixedDeltaTimer,
			ITickable fixedDeltaTimerAsTickable,
			Func<TDelta, float> deltaToFloatDelegate,
			ILoggerResolver loggerResolver)
		{
			FixedDelta = fixedDelta;

			this.fixedDeltaTimer = fixedDeltaTimer;

			this.fixedDeltaTimerAsTickable = fixedDeltaTimerAsTickable;

			this.deltaToFloatDelegate = deltaToFloatDelegate;


			synchronizeFixedScubscription = DelegateWrapperFactory.BuildSubscriptionSingleArgGeneric<IRuntimeTimer>(
				SynchronizeFixed,
				loggerResolver);

			fixedDeltaTimer.Repeat = true;

			fixedDeltaTimer.FlushTimeElapsedOnRepeat = false;

			fixedDeltaTimer.FireRepeatCallbackOnFinish = false;

			fixedDeltaTimer.Reset(
				deltaToFloatDelegate(
					FixedDelta));

			fixedDeltaTimer.Start();

			fixedDeltaTimer.OnFinishRepeated.Subscribe(
				(INonAllocSubscriptionHandler<
					INonAllocSubscribable,
					IInvokableSingleArgGeneric<IRuntimeTimer>>)
					synchronizeFixedScubscription);
		}

		#region IHasFixedDelta

		public TDelta FixedDelta { get; private set; }

		public void SetFixedDelta(TDelta fixedDelta)
		{
			FixedDelta = fixedDelta;

			fixedDeltaTimer.Reset(
				deltaToFloatDelegate(
					FixedDelta));

			fixedDeltaTimer.Start();
		}

		public void Tick(TDelta delta)
		{
			fixedDeltaTimerAsTickable.Tick(
				deltaToFloatDelegate(
					delta));
		}

		#endregion

		#region IPublisherDependencyRecipient

		public IPublisherSingleArgGeneric<TDelta> BroadcasterAsPublisher
		{
			set
			{
				broadcasterAsPublisher = value;
			}
		}

		#endregion

		private void SynchronizeFixed(IRuntimeTimer timer)
		{
			broadcasterAsPublisher?.Publish(FixedDelta);
		}

		#region ICleanUppable

		public void Cleanup()
		{
			if (broadcasterAsPublisher is ICleanuppable)
				(broadcasterAsPublisher as ICleanuppable).Cleanup();

			if (synchronizeFixedScubscription is ICleanuppable)
				(synchronizeFixedScubscription as ICleanuppable).Cleanup();

			if (fixedDeltaTimer is ICleanuppable)
				(fixedDeltaTimer as ICleanuppable).Cleanup();
		}

		#endregion

		#region IDisposable

		public void Dispose()
		{
			if (broadcasterAsPublisher is IDisposable)
				(broadcasterAsPublisher as IDisposable).Dispose();

			if (synchronizeFixedScubscription is IDisposable)
				(synchronizeFixedScubscription as IDisposable).Dispose();

			if (fixedDeltaTimer is IDisposable)
				(fixedDeltaTimer as IDisposable).Dispose();
		}

		#endregion
	}
}