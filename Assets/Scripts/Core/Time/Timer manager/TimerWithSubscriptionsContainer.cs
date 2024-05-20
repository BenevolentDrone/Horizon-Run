using HereticalSolutions.Delegates.Subscriptions;

namespace HereticalSolutions.Time
{
    public class TimerWithSubscriptionsContainer
    {
        public IRuntimeTimer Timer;

        public SubscriptionSingleArgGeneric<float> TickSubscription;

        public SubscriptionSingleArgGeneric<IRuntimeTimer> StartTimerSubscription;
        
        public SubscriptionSingleArgGeneric<IRuntimeTimer> FinishTimerSubscription;
    }
}