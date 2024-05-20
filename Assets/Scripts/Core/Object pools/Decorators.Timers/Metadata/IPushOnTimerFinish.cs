using HereticalSolutions.Delegates.Subscriptions;
using HereticalSolutions.Time;

namespace HereticalSolutions.Pools
{
	public interface IPushOnTimerFinish
	{
		float Duration { get; }

		SubscriptionSingleArgGeneric<IRuntimeTimer> PushSubscription { get; }

		int TimerID { get; set; }
	}
}