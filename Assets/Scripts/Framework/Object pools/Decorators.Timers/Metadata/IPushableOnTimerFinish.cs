using HereticalSolutions.Delegates.Subscriptions;

using HereticalSolutions.Time;

namespace HereticalSolutions.Pools
{
	public interface IPushableOnTimerFinish
	{
		float Duration { get; }

		SubscriptionSingleArgGeneric<IRuntimeTimer> PushSubscription { get; }

		ushort TimerHandle { get; set; }
	}
}